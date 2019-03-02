using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MouseTracer
{
    class Tracer
    {
        public Graphics graph;
        public Bitmap image;
        private Rectangle screenBounds;
        private ColorPalette palette;

        private const int HistLength = 4;
        private List<Point> mouseHistory;

        private bool running;

        public bool DrawClicks { get; set; }

        public Tracer(ColorPalette palette)
        {
            this.palette = palette;

            // TODO: unsubscribe from MouseHook on destruct
            MouseHook.MouseAction += DoMouseEvent;

            screenBounds = Utils.GetScreenSize();
            image = new Bitmap(screenBounds.Width, screenBounds.Height);
            graph = Graphics.FromImage(image);
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graph.Clear(palette.background);

            running = false;
            mouseHistory = new List<Point>();
            DrawClicks = true;
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
                Color c = palette.DirMod(mouseHistory[0], mouseHistory[1]);
                graph.DrawLine(new Pen(c), mouseHistory[0], mouseHistory[1]);
            }
        }

        private void DrawMouseClick()
        {
            if (DrawClicks && mouseHistory.Count >= 2)
            {
                const int cw = 10;
                Color c = palette.DirMod(mouseHistory[0], mouseHistory[1]);
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
