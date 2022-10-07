using System;
using System.Drawing;

namespace MouseTracer.Palettes
{
	public class PaletteBlackWhite : ColorPalette
	{
		public override Color Background => Color.FromArgb(255, 255, 255);

		public override Color VectorColor(Point from, Point to) => Color.FromArgb(120, 0, 0, 0);
	}
}
