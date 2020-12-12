using System;
using System.Drawing;

namespace MouseTracer
{
    public abstract class ColorPalette
    {
        public abstract Color Background { get; }

        public abstract Color VectorColor(Point from, Point to);
    }

    public class PaletteBlackWhite : ColorPalette
    {
        public override Color Background => Color.FromArgb(255, 255, 255);

        public override Color VectorColor(Point from, Point to) => Color.FromArgb(120, 0, 0, 0);
    }

    public class PaletteColorful : ColorPalette
    {
        public override Color Background => Color.FromArgb(0, 0, 0);

        public override Color VectorColor(Point from, Point to)
        {
            int dx = to.X - from.X, dy = to.Y - from.Y;
            if (dx != 0 || dy != 0)
            {
                Color c = Utils.HsvToColor(Math.Atan2(dy, dx) / Math.PI / 2 + 0.5, 1.0, 255);
                return Color.FromArgb(200, c.R, c.G, c.B);
            }
            else
            {
                return Color.Black;
            }
        }
    }
}
