using System;
using System.Drawing;

namespace MouseTracer.Palettes
{
    public abstract class ColorPalette
    {
        public abstract Color Background { get; }

        public abstract Color VectorColor(Point from, Point to);
    }
}
