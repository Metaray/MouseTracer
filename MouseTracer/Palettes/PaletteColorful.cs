using System;
using System.Drawing;

namespace MouseTracer.Palettes
{
	public class PaletteColorful : ColorPalette
	{
		private const int alpha = 110;

		private readonly double saturation;

		public PaletteColorful(double saturation = 1.0)
		{
			if (saturation < 0 || saturation > 1)
			{
				throw new ArgumentException("Saturation value outside [0; 1]");
			}

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

			var c = Utils.HsvToColor(Math.Atan2(dy, dx) / Math.PI / 2 + 0.5, saturation, 255);
			return Color.FromArgb(alpha, c.R, c.G, c.B);
		}
	}
}
