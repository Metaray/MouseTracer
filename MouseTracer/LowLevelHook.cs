using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Drawing;

namespace MouseTracer
{
	public class LowLevelHook : MouseHook
	{
		private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

		private Thread hookThread;

		private readonly LowLevelMouseProc hookProcPin; // Without Pin garbage collector destroys callback procedure

		private SafeHookHandle hookHandle;

		private uint lastMoveTimeMs = 0;

		private readonly uint moveEventDelayMs;

		private MouseButtons buttonsState;

		public LowLevelHook(uint minMoveDelay)
			: base()
		{
			if (Debugger.IsAttached)
			{
				Trace.TraceWarning("Low level hook will not start when debugger is present");
			}

			hookProcPin = HookCallback;
			moveEventDelayMs = minMoveDelay;
		}

		public override void Start()
		{
			base.Start();

			if (hookThread == null && !Debugger.IsAttached)
			{
				hookThread = new Thread(HookThreadLoop)
				{
					IsBackground = true,
					Priority = ThreadPriority.Highest
				};
				hookThread.Start();
			}
		}

		public override void Stop()
		{
			base.Stop();

			if (hookThread != null)
			{
				hookHandle.Dispose();
				hookThread.Abort();
				hookThread = null;
			}
		}

		private void HookThreadLoop()
		{
			hookHandle = SetWindowsHookEx(WH_MOUSE_LL, hookProcPin, IntPtr.Zero, 0);
			if (hookHandle.IsInvalid)
			{
				throw new System.ComponentModel.Win32Exception();
			}

			Application.Run(); // Start message pump
		}

		private void EnqueueNewEvent(MouseMessages message, MSLLHOOKSTRUCT data)
		{
			var pos = new Point(data.pt.x, data.pt.y);

			switch (message)
			{
				case MouseMessages.WM_MOUSEMOVE:
					break;

				case MouseMessages.WM_LBUTTONDOWN:
					buttonsState |= MouseButtons.Left;
					break;
				
				case MouseMessages.WM_RBUTTONDOWN:
                    buttonsState |= MouseButtons.Right;
					break;

				case MouseMessages.WM_LBUTTONUP:
                    buttonsState &= ~MouseButtons.Left;
					break;

				case MouseMessages.WM_RBUTTONUP:
                    buttonsState &= ~MouseButtons.Right;
					break;

				default:
					// Don't send events for other messages
					return;
			}

			EnqueueNewEvent(new MouseStateEventArgs(pos, buttonsState));
		}

		private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0)
			{
				var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
				var message = (MouseMessages)wParam;
				if (message == MouseMessages.WM_MOUSEMOVE)
				{
					if (hookStruct.time - lastMoveTimeMs >= moveEventDelayMs)
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

		#region P/Invoke

		private class SafeHookHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			private SafeHookHandle() : base(true) { }

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

		#endregion
	}
}
