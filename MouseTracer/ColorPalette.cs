using System;
using System.Collections.Generic;
using System.Drawing;

namespace MouseTracer
{
    abstract class ColorPalette
    {
        public Color background;

        public abstract Color DirMod(Point a, Point b);
    }

    class PaletteBlackWhite : ColorPalette
    {
        public PaletteBlackWhite()
        {
            this.background = Color.FromArgb(255, 255, 255);
        }

        public override Color DirMod(Point a, Point b)
        {
            return Color.FromArgb(120, 0, 0, 0);
        }
    }

    class PaletteColorful : ColorPalette
    {
        public PaletteColorful()
        {
            this.background = Color.FromArgb(0, 0, 0);
        }

        public override Color DirMod(Point a, Point b)
        {
            int dx = b.X - a.X, dy = b.Y - a.Y;
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
