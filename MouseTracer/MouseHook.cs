using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;

namespace MouseTracer
{
    public static class MouseHook
    {
        public delegate void MouseEventHandler(object sender, MouseEventArgs e);
        public static event MouseEventHandler MouseAction = delegate { };

        private static Timer sendTimer;
        private static Queue<MouseEventArgs> events;

        public static void HookSetup()
        {
            events = new Queue<MouseEventArgs>();
            sendTimer = new Timer();
            sendTimer.Interval = 1;
            sendTimer.Start();
            sendTimer.Tick += SendTimer_Tick;
        }

        private static void SendTimer_Tick(object sender, EventArgs e)
        {
            while (events.Count > 0)
            {
                MouseAction(null, events.Dequeue());
            }
        }

        public static void Start()
        {
            hookHandle = SetHook(_proc);
        }
        public static void Stop()
        {
            hookHandle.Dispose();
            hookHandle = null;
        }

        private static LowLevelMouseProc _proc = HookCallback;
        private static SafeHookHandle hookHandle;
        private static uint lastMoveTime = 0;
        public static uint moveLimit = 10;

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

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
          int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                switch ((MouseMessages)wParam)
                {
                    case MouseMessages.WM_LBUTTONDOWN:
                        {
                            events.Enqueue(new MouseEventArgs(MouseButtons.Left, 1, hookStruct.pt.x, hookStruct.pt.y, 0));
                            break;
                        }
                    case MouseMessages.WM_RBUTTONDOWN:
                        {
                            events.Enqueue(new MouseEventArgs(MouseButtons.Right, 1, hookStruct.pt.x, hookStruct.pt.y, 0));
                            break;
                        }
                    case MouseMessages.WM_MOUSEMOVE:
                        {
                            if (hookStruct.time - lastMoveTime >= moveLimit)
                            {
                                events.Enqueue(new MouseEventArgs(MouseButtons.None, 0, hookStruct.pt.x, hookStruct.pt.y, 0));
                                lastMoveTime = hookStruct.time;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            return CallNextHookEx(hookHandle, nCode, wParam, lParam);
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

        internal class SafeHookHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeHookHandle()
                : base(true)
            {
            }

            override protected bool ReleaseHandle()
            {
                return UnhookWindowsHookEx(handle);
            }
        }
    }
}
