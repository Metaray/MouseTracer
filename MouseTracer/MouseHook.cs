using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Concurrent;
using Microsoft.Win32.SafeHandles;
using System.Threading;

namespace MouseTracer
{
    public static class MouseHook
    {
        private static Thread hookThread;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Without Pin garbage collector destroys callback procedure
        private static LowLevelMouseProc hookProcPin = HookCallback;
        private static SafeHookHandle hookHandle;

        // Times in milliseconds
        private static uint lastMoveTime = 0;
        public static uint MoveEventDelay = 10;

        public delegate void MouseEventHandler(object sender, MouseEventArgs e);
        public static event MouseEventHandler MouseAction = delegate { };

        private static System.Windows.Forms.Timer sendTimer;
        private static ConcurrentQueue<MouseEventArgs> events;

        static MouseHook()
        {
            events = new ConcurrentQueue<MouseEventArgs>();

            sendTimer = new System.Windows.Forms.Timer();
            sendTimer.Interval = 10;
            sendTimer.Start();
            sendTimer.Tick += SendTimer_Tick;
        }

        private static void HookThreadLoop()
        {
            hookHandle = SetHook(hookProcPin);
            Application.Run();
        }

        private static void SendTimer_Tick(object sender, EventArgs e)
        {
            MouseEventArgs args;
            while (events.TryDequeue(out args))
            {
                MouseAction(null, args);
            }
        }

        public static void Start()
        {
            hookThread = new Thread(HookThreadLoop);
            hookThread.IsBackground = true;
            hookThread.Start();
        }

        public static void Stop()
        {
            hookHandle.Dispose();
            hookHandle = null;
        }

        private static SafeHookHandle SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                SafeHookHandle hook = SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle("user32"), 0);
                if (hook.IsInvalid)
                    throw new System.ComponentModel.Win32Exception();
                return hook;
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                switch ((MouseMessages)wParam)
                {
                case MouseMessages.WM_LBUTTONDOWN:
                    events.Enqueue(new MouseEventArgs(MouseButtons.Left, 1, hookStruct.pt.x, hookStruct.pt.y, 0));
                    break;

                case MouseMessages.WM_RBUTTONDOWN:
                    events.Enqueue(new MouseEventArgs(MouseButtons.Right, 1, hookStruct.pt.x, hookStruct.pt.y, 0));
                    break;

                case MouseMessages.WM_MOUSEMOVE:
                    if (hookStruct.time - lastMoveTime >= MoveEventDelay)
                    {
                        events.Enqueue(new MouseEventArgs(MouseButtons.None, 0, hookStruct.pt.x, hookStruct.pt.y, 0));
                        lastMoveTime = hookStruct.time;
                    }
                    break;

                default:
                    break;
                }
            }
            return CallNextHookEx(hookHandle, nCode, wParam, lParam);
        }

        internal class SafeHookHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeHookHandle() : base(true) {}

            override protected bool ReleaseHandle()
            {
                return UnhookWindowsHookEx(handle);
            }
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern SafeHookHandle SetWindowsHookEx(int idHook,
          LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(SafeHookHandle hhk, int nCode,
          IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
