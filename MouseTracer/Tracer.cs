using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MouseTracer
{
    class Tracer : IDisposable
    {
        private Graphics graph;
        public Bitmap Image { get; private set; }
        private Rectangle screenBounds;
        private ColorPalette palette;

        private const int HistLength = 4;
        private List<Point> mouseHistory;

        private bool running;

        public bool DrawClicks { get; set; }

        public Tracer(ColorPalette palette)
        {
            this.palette = palette;

            screenBounds = Utils.GetScreenSize();
            Image = new Bitmap(screenBounds.Width, screenBounds.Height);
            graph = Graphics.FromImage(Image);
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graph.Clear(palette.Background);

            running = false;
            mouseHistory = new List<Point>();
            DrawClicks = true;

            MouseHook.MouseAction += DoMouseEvent;
        }

        public void Dispose()
        {
            MouseHook.MouseAction -= DoMouseEvent;
            graph.Dispose();
            Image.Dispose();
        }

        private void DoMouseEvent(object sender, MouseEventArgs e)
        {
            if (!running)
            {
                return;
            }
            if (UpdateMousePos(e.X - screenBounds.X, e.Y - screenBounds.Y))
            {
                DrawMouseMove();
            }
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                DrawMouseClick();
            }
        }

        private void DrawMouseMove()
        {
            if (mouseHistory.Count >= 2)
            {
                Color c = palette.VectorColor(mouseHistory[0], mouseHistory[1]);
                graph.DrawLine(new Pen(c), mouseHistory[0], mouseHistory[1]);
            }
        }

        private void DrawMouseClick()
        {
            if (DrawClicks && mouseHistory.Count >= 2)
            {
                const int cw = 10; // click circle diameter
                Color c = palette.VectorColor(mouseHistory[0], mouseHistory[1]);
                graph.FillEllipse(new SolidBrush(c), mouseHistory[0].X - cw / 2, mouseHistory[0].Y - cw / 2, cw, cw);
            }
        }

        private bool UpdateMousePos(int x, int y)
        {
            Point pos = new Point(x, y);
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

        public void SetRunning(bool run)
        {
            if (run)
            {
                mouseHistory.Clear();
            }
            running = run;
        }
    }
}
