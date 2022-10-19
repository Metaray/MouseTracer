using System;
using System.Windows.Forms;

namespace MouseTracer
{
	public class DebuggingHook : MouseHook
	{
		private readonly Timer pollTimer;

		public Form TrackedWindow;

		public DebuggingHook()
			: base()
		{
			pollTimer = new Timer
			{
				Interval = 10
			};
			pollTimer.Tick += PollTimer_Tick;
		}

		public override void Start()
		{
			base.Start();
			pollTimer.Start();

			if (TrackedWindow != null)
			{
				TrackedWindow.MouseClick += TrackedWindow_MouseClick;
			}
		}

		public override void Stop()
		{
			base.Stop();
			pollTimer.Stop();

			if (TrackedWindow != null)
			{
				TrackedWindow.MouseClick -= TrackedWindow_MouseClick;
			}
		}

		private void PollTimer_Tick(object sender, EventArgs e)
		{
			EnqueueNewEvent(new MouseEventArgs(MouseButtons.None, 0, Cursor.Position.X, Cursor.Position.Y, 0));
		}

		private void TrackedWindow_MouseClick(object sender, MouseEventArgs e)
		{
			EnqueueNewEvent(new MouseEventArgs(e.Button, 1, Cursor.Position.X, Cursor.Position.Y, 0));
		}
	}
}
