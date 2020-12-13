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
        private double saturation;

        public PaletteColorful(double saturation=1.0)
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
            int dx = to.X - from.X, dy = to.Y - from.Y;
            if (dx != 0 || dy != 0)
            {
                Color c = Utils.HsvToColor(Math.Atan2(dy, dx) / Math.PI / 2 + 0.5, saturation, 255);
                return Color.FromArgb(200, c.R, c.G, c.B);
            }
            else
            {
                return Color.Black;
            }
        }
    }

    public class PaletteInterpolated : ColorPalette
    {
        private Color[] colors;

        private bool useGc;

        public PaletteInterpolated(Color[] colors, bool useGc=false)
        {
            if (colors.Length < 2)
            {
                throw new ArgumentException("Need at least 2 colors to interpolate");
            }
            this.colors = colors;
            this.useGc = useGc;
        }

        public override Color Background => Color.FromArgb(0, 0, 0);

        public override Color VectorColor(Point from, Point to)
        {
            int dx = to.X - from.X, dy = to.Y - from.Y;
            if (dx != 0 || dy != 0)
            {
                double a = (Math.Atan2(dy, dx) / Math.PI + 1) / 2 * colors.Length;
                double f = a % 1.0;
                int i = ((int)a) % colors.Length;
                var c1 = colors[i];
                var c2 = colors[(i + 1) % colors.Length];
                if (useGc)
                {
                    return Color.FromArgb(
                        200,
                        (int)Utils.LerpGc(c1.R, c2.R, f),
                        (int)Utils.LerpGc(c1.G, c2.G, f),
                        (int)Utils.LerpGc(c1.B, c2.B, f)
                    );
                } 
                else
                {
                    return Color.FromArgb(
                        200,
                        (int)Utils.Lerp(c1.R, c2.R, f),
                        (int)Utils.Lerp(c1.G, c2.G, f),
                        (int)Utils.Lerp(c1.B, c2.B, f)
                    );
                }
            }
            else
            {
                return Color.Black;
            }
        }
    }
}
