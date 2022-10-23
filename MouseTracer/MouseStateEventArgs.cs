using System;
using System.Drawing;
using System.Windows.Forms;

namespace MouseTracer
{
	public class MouseStateEventArgs : EventArgs
	{
		public readonly Point Position;

		public readonly MouseButtons Buttons;

		public readonly Point PreviousPosition;

		public readonly MouseButtons PreviousButtons;

		public MouseButtons Pressed => Buttons & ~PreviousButtons;

		public int X => Position.X;

		public int Y => Position.Y;

		public MouseStateEventArgs(Point position, MouseButtons buttons, Point previousPosition, MouseButtons previousButtons)
		{
			Position = position;
			Buttons = buttons;
			PreviousPosition = previousPosition;
			PreviousButtons = previousButtons;
		}
	}
}
