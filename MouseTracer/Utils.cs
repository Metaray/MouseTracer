using System;
using System.Drawing;
using System.Windows.Forms;

namespace MouseTracer
{
    public static class Utils
    {
        public static Rectangle GetScreenSize()
        {
            int minx = 0, maxx = 0, miny = 0, maxy = 0;
            foreach(Screen scr in Screen.AllScreens)
            {
                minx = Math.Min(minx, scr.Bounds.X);
                maxx = Math.Max(maxx, scr.Bounds.X + scr.Bounds.Width);
                miny = Math.Min(miny, scr.Bounds.Y);
                maxy = Math.Max(maxy, scr.Bounds.Y + scr.Bounds.Height);
            }
            return new Rectangle(minx, miny, maxx - minx, maxy - miny);
        }

        public static Color HsvToColor(double h, double s, double v)
        {
            if (s == 0.0)
                return Color.FromArgb((int)v, (int)v, (int)v);
            int i = (int)(h * 6.0);
            double f = (h * 6.0) - i;
            double p = v * (1.0 - s);
            double q = v * (1.0 - s * f);
            double t = v * (1.0 - s * (1.0 - f));
            i = i % 6;
            if (i == 0)
                return Color.FromArgb((int)v, (int)t, (int)p);
            else if (i == 1)
                return Color.FromArgb((int)q, (int)v, (int)p);
            else if (i == 2)
                return Color.FromArgb((int)p, (int)v, (int)t);
            else if (i == 3)
                return Color.FromArgb((int)p, (int)q, (int)v);
            else if (i == 4)
                return Color.FromArgb((int)t, (int)p, (int)v);
            else
                return Color.FromArgb((int)v, (int)p, (int)q);
        }

        public static double Lerp(double a, double b, double x)
        {
            return a + x * (b - a);
        }

        public static double LerpGc(double a, double b, double x)
        {
            const double GC = 2.2;
            return Math.Pow(Lerp(Math.Pow(a, GC), Math.Pow(b, GC), x), 1 / GC);
        }
    }
}
