using System;
using System.Drawing;

namespace MouseTracer.Palettes
{
	public class PaletteColorful : ColorPalette
	{
		private const int alpha = 110;

		private readonly double saturation;

		private readonly bool symmetric;

		public PaletteColorful(bool symmetric = true, double saturation = 1.0)
		{
			if (saturation < 0 || saturation > 1)
			{
				throw new ArgumentException("Saturation value outside [0; 1]");
			}

			this.symmetric = symmetric;
			this.saturation = saturation;
		}

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
			if (symmetric)
			{
				a *= 2;
			}
			var c = Utils.HsvToColor(a + 0.5, saturation, 255);
			return Color.FromArgb(alpha, c.R, c.G, c.B);
		}
	}
}
