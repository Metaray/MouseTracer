using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseTracer
{
	public class PollingHook : MouseHook
	{
		private readonly Timer pollTimer;

		private Point prevPos = new Point(-1234, -1234);

		private MouseButtons prevButtons;

		public PollingHook(int intervalMs)
		{
			if (intervalMs <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(intervalMs), "Interval must be positive");
			}

			pollTimer = new Timer
			{
				Interval = intervalMs
			};
			pollTimer.Tick += PollTimer_Tick;
		}

		public override void Start()
		{
			base.Start();
			pollTimer.Start();
		}

		public override void Stop()
		{
			base.Stop();
			pollTimer.Stop();
		}

		private void PollTimer_Tick(object sender, EventArgs e)
		{
			var pos = Cursor.Position;

			var buttons = MouseButtons.None;
			if (IsKeyPressed(Keys.LButton))
				buttons |= MouseButtons.Left;
			if (IsKeyPressed(Keys.RButton))
				buttons |= MouseButtons.Right;
			if (IsKeyPressed(Keys.MButton))
				buttons |= MouseButtons.Middle;

			var pressed = (~prevButtons) & buttons;

			if (pos != prevPos || pressed != MouseButtons.None)
			{
				NotifyMouseAction(new MouseEventArgs(pressed, pressed != MouseButtons.None ? 1 : 0, pos.X, pos.Y, 0));
			}

			prevPos = pos;
			prevButtons = buttons;
		}

		#region P/Invoke

		private bool IsKeyPressed(Keys key) => (GetKeyState((int)key) & 0x8000) != 0;

		[DllImport("USER32.dll")]
		private static extern short GetKeyState(int nVirtKey);

		#endregion
	}
}
