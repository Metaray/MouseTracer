using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MouseTracer.Palettes;

namespace MouseTracer
{
    public class Tracer : IDisposable
    {
        public Bitmap Image { get; private set; }
        private Graphics graph;
        private Rectangle screenBounds;
        private ColorPalette palette;

        private const int HistLength = 4;
        private List<Point> mouseHistory = new List<Point>();

        private bool running = false;

        public bool DrawClicks { get; set; } = true;
        public bool DrawMouseMove { get; set; } = true;

        public Tracer(ColorPalette palette)
        {
            this.palette = palette;

            screenBounds = Utils.GetScreenSize();
            Image = new Bitmap(screenBounds.Width, screenBounds.Height);
            
            graph = Graphics.FromImage(Image);
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graph.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            graph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graph.Clear(palette.Background);
        }

        public void Dispose()
        {
            SetRunning(false);
            graph.Dispose();
            Image.Dispose();
        }

        public void SetRunning(bool run)
        {
            if (run == running) return;

            if (run)
            {
                mouseHistory.Clear();
                MouseHook.MouseAction += DoMouseEvent;
            }
            else
            {
                MouseHook.MouseAction -= DoMouseEvent;
            }

            running = run;
        }

        private void DoMouseEvent(object sender, MouseEventArgs e)
        {
            if (UpdateMousePos(e.X - screenBounds.X, e.Y - screenBounds.Y))
            {
                DoDrawMouseMove();
            }

            if (e.Button != MouseButtons.None)
            {
                DoDrawMouseClick(e.Button);
            }
        }

        private void DoDrawMouseMove()
        {
            if (DrawMouseMove && mouseHistory.Count >= 2)
            {
                var c = palette.VectorColor(mouseHistory[1], mouseHistory[0]);
                using (var p = new Pen(c))
                {
                    graph.DrawLine(p, mouseHistory[1], mouseHistory[0]);
                }
            }
        }

        private void DoDrawMouseClick(MouseButtons button)
        {
            if (DrawClicks && mouseHistory.Count >= 2)
            {
                const float cw = 15; // click circle diameter
                var c = palette.VectorColor(mouseHistory[1], mouseHistory[0]);
                if (button == MouseButtons.Left)
                {
                    using (var b = new SolidBrush(c))
                    {
                        graph.FillEllipse(b, mouseHistory[0].X - cw / 2, mouseHistory[0].Y - cw / 2, cw, cw);
                    }
                }
                else
                {
                    using (var p = new Pen(c))
                    {
                        graph.DrawEllipse(p, mouseHistory[0].X - cw / 2, mouseHistory[0].Y - cw / 2, cw, cw);
                    }
                }
            }
        }

        private bool UpdateMousePos(int x, int y)
        {
            var pos = new Point(x, y);
            
            if (mouseHistory.Count > 0 && pos == mouseHistory[0])
            {
                return false;
            }

            mouseHistory.Insert(0, pos);
            if (mouseHistory.Count > HistLength)
            {
                mouseHistory.RemoveAt(HistLength);
            }

            return true;
        }
    }
}
