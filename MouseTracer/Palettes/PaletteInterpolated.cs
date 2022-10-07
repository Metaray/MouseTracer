using System;
using System.Drawing;

namespace MouseTracer.Palettes
{
	public class PaletteInterpolated : ColorPalette
	{
		private const int alpha = 110;

		private readonly Color[] colors;

		private readonly bool useGc;

		public PaletteInterpolated(Color[] colors, bool useGc = false)
		{
			if (colors.Length < 2)
			{
				throw new ArgumentException("Need at least 2 colors to interpolate", nameof(colors));
			}

			this.colors = colors;
			this.useGc = useGc;
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

			var a = (Math.Atan2(dy, dx) / Math.PI + 1) / 2 * colors.Length;
			var f = a % 1.0;
			var i = ((int)a) % colors.Length;
			var c1 = colors[i];
			var c2 = colors[(i + 1) % colors.Length];

			return Color.FromArgb(
				alpha,
				LerpColor(c1.R, c2.R, f),
				LerpColor(c1.G, c2.G, f),
				LerpColor(c1.B, c2.B, f)
			);
		}

		private int LerpColor(double a, double b, double x)
		{
			return (int)Math.Round(
				useGc
				? Utils.LerpGc(a, b, x)
				: Utils.Lerp(a, b, x)
			);
		}
	}
}
