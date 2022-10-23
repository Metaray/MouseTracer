using System;
using System.Drawing;
using System.Windows.Forms;

namespace MouseTracer
{
	public class MouseStateEventArgs : EventArgs
	{
		public readonly Point Position;

		public readonly MouseButtons Buttons;

		public int X => Position.X;

		public int Y => Position.Y;

		public MouseStateEventArgs(Point position, MouseButtons buttons)
		{
			Position = position;
			Buttons = buttons;
		}
	}
}
