using System;
using System.Drawing;

namespace MouseTracer.Palettes
{
	public class PaletteColorful : ColorPalette
	{
		public bool Symmetric { get; set; } = true;

		public double Saturation { get; set; } = 1.0;

		public double ColorOffset { get; set; } = 0.5;

		private const int Alpha = 110;

		public override Color Background => Color.FromArgb(0, 0, 0);

		public override Color VectorColor(Point from, Point to)
		{
			var dx = to.X - from.X;
			var dy = to.Y - from.Y;

			if (dx == 0 && dy == 0)
			{
				return Color.Black;
			}

			var a = Math.Atan2(dy, dx) / (Math.PI * 2);
			if (Symmetric)
			{
				a *= 2;
			}
			var c = Utils.HsvToColor(a + ColorOffset, Saturation, 255);
			return Color.FromArgb(Alpha, c.R, c.G, c.B);
		}
	}
}
