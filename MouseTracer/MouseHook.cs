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
        private static LowLevelMouseProc hookProcPin = HookCallback; // Without Pin garbage collector destroys callback procedure
        private static SafeHookHandle hookHandle;

        public static uint MoveEventDelayMs = 10;
        private static uint lastMoveTimeMs = 0;
        
        private const int SendIntervalMs = 50;
        private const uint MaxQueuedEvents = 128;
        private static ConcurrentQueue<CallbackData> hookEvents;

        public static event EventHandler<MouseEventArgs> MouseAction = delegate { };
        private static System.Windows.Forms.Timer sendTimer;

        static MouseHook()
        {
            hookEvents = new ConcurrentQueue<CallbackData>();

            sendTimer = new System.Windows.Forms.Timer();
            sendTimer.Interval = SendIntervalMs;
            sendTimer.Tick += SendTimer_Tick;
        }

        private static void SendTimer_Tick(object sender, EventArgs e)
        {
            // TODO: make proper polling solution for debugging
            if (Debugger.IsAttached)
            {
                MouseAction(null, new MouseEventArgs(MouseButtons.None, 0, Cursor.Position.X, Cursor.Position.Y, 0));
                return;
            }

            while (hookEvents.TryDequeue(out var evt))
            {
                if (evt.Message == MouseMessages.WM_MOUSEMOVE)
                {
                    MouseAction(null, new MouseEventArgs(MouseButtons.None, 0, evt.Data.pt.x, evt.Data.pt.y, 0));
                }
                else if (evt.Message == MouseMessages.WM_LBUTTONDOWN)
                {
                    MouseAction(null, new MouseEventArgs(MouseButtons.Left, 1, evt.Data.pt.x, evt.Data.pt.y, 0));
                }
                else if (evt.Message == MouseMessages.WM_RBUTTONDOWN)
                {
                    MouseAction(null, new MouseEventArgs(MouseButtons.Right, 1, evt.Data.pt.x, evt.Data.pt.y, 0));
                }
            }
        }

        public static void Start()
        {
            sendTimer.Start();
            if (hookThread == null && !Debugger.IsAttached)
            {
                hookThread = new Thread(HookThreadLoop);
                hookThread.IsBackground = true;
                hookThread.Priority = ThreadPriority.Highest;
                hookThread.Start();
            }
        }

        public static void Stop()
        {
            sendTimer.Stop();
            if (hookThread != null)
            {
                hookHandle?.Dispose();
                hookHandle = null;
                hookThread.Abort();
                hookThread = null;
            }
        }

        private static void HookThreadLoop()
        {
            hookHandle = SetHook(hookProcPin);
            Application.Run(); // Start message pump
        }

        private static SafeHookHandle SetHook(LowLevelMouseProc hookProc)
        {
            SafeHookHandle hook = SetWindowsHookEx(WH_MOUSE_LL, hookProc, IntPtr.Zero, 0);
            if (hook.IsInvalid)
                throw new System.ComponentModel.Win32Exception();
            return hook;
        }

        private static void EnqueueNewEvent(MouseMessages message, MSLLHOOKSTRUCT data)
        {
            // Drop events if consumer thread stalled to prevent runaway memory growth
            // Max events >= Mouse polling frequency * Consumer polling interval
            if (hookEvents.Count < MaxQueuedEvents)
            {
                hookEvents.Enqueue(new CallbackData() { Message = message, Data = data });
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                var message = (MouseMessages)wParam;
                if (message == MouseMessages.WM_MOUSEMOVE)
                {
                    if (hookStruct.time - lastMoveTimeMs >= MoveEventDelayMs)
                    {
                        EnqueueNewEvent(message, hookStruct);
                        lastMoveTimeMs = hookStruct.time;
                    }
                } 
                else
                {
                    EnqueueNewEvent(message, hookStruct);
                }
            }
            return CallNextHookEx(hookHandle, nCode, wParam, lParam);
        }

        private struct CallbackData
        {
            public MouseMessages Message;
            public MSLLHOOKSTRUCT Data;
        }

        private class SafeHookHandle : SafeHandleZeroOrMinusOneIsInvalid
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
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_MOUSEWHEEL = 0x020A,
            WM_MOUSEHWHEEL = 0x020E,
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
        private static extern SafeHookHandle SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(SafeHookHandle hhk, int nCode, IntPtr wParam, IntPtr lParam);
    }
}
