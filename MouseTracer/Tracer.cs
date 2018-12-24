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

        private bool running;
        private const int histLength = 4;
        private List<Point> mouseHistory;

        private bool drawClicks = true;
        public bool DrawClicks
        {
            get { return drawClicks; }
            set { drawClicks = value; }
        }

        public Tracer(ColorPalette palette)
        {
            this.palette = palette;

            MouseHook.MouseAction += DoMouseEvent;

            screenBounds = Utils.GetScreenSize();
            image = new Bitmap(screenBounds.Width, screenBounds.Height);
            graph = Graphics.FromImage(image);
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graph.Clear(palette.background);

            running = false;
            mouseHistory = new List<Point>();
        }

        private void DoMouseEvent(object sender, MouseEventArgs e)
        {
            if (!running) return;
            UpdateMousePos(e.X, e.Y);
            if (e.Button == MouseButtons.None)
            {
                DrawMouseMove();
            }
            else if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
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
            if (drawClicks && mouseHistory.Count >= 2)
            {
                const int cw = 10;
                Color c = palette.DirMod(mouseHistory[0], mouseHistory[1]);
                graph.DrawLine(new Pen(c), mouseHistory[0], mouseHistory[1]);
                graph.FillEllipse(new SolidBrush(c), mouseHistory[0].X - cw / 2, mouseHistory[0].Y - cw / 2, cw, cw);
            }
        }

        private void UpdateMousePos(int x, int y)
        {
            Point np = new Point(x, y);
            if (mouseHistory.Count < histLength)
            {
                mouseHistory.Insert(0, np);
            }
            else
            {
                if (np == mouseHistory[0]) return;
                for (int i = histLength-2; i >= 0; --i)
                    mouseHistory[i+1] = mouseHistory[i];
                mouseHistory[0] = np;
            }
        }

        public void SetRunning(bool state)
        {
            if (state == true)
                mouseHistory.Clear();
            running = state;
        }
    }
}
