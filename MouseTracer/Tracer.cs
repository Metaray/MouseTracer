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
        private readonly Graphics graph;
        private readonly Rectangle screenBounds;
        private readonly ColorPalette palette;

        private const int HistLength = 4;
        private readonly List<Point> mouseHistory = new List<Point>();

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
				Program.MouseHook.MouseAction += DoMouseEvent;
            }
            else
            {
				Program.MouseHook.MouseAction -= DoMouseEvent;
            }

            running = run;
        }

        private void DoMouseEvent(object sender, MouseStateEventArgs e)
        {
            UpdateMouseHistory(e);

            if (DrawMouseMove)
            {
                DoDrawMouseMove();
            }

            if (DrawClicks)
            {
                DoDrawMouseClick(e.Pressed);
            }
        }

		private void UpdateMouseHistory(MouseStateEventArgs e)
		{
			var pos = e.Position;
			pos.X -= screenBounds.X;
			pos.Y -= screenBounds.Y;

			if (mouseHistory.Count > 0 && mouseHistory[0] == pos)
			{
				return;
			}

			mouseHistory.Insert(0, pos);
			if (mouseHistory.Count > HistLength)
			{
				mouseHistory.RemoveAt(HistLength);
			}
		}

		private void DoDrawMouseMove()
        {
            if (mouseHistory.Count < 2)
            {
                return;
            }

            var prev = mouseHistory[1];
			var cur = mouseHistory[0];
            if (prev == cur)
            {
                return;
            }

			var c = palette.VectorColor(prev, cur);
            using (var p = new Pen(c))
            {
                graph.DrawLine(p, prev, cur);
            }
        }

        private void DoDrawMouseClick(MouseButtons button)
        {
			const float CCD = 15; // click circle diameter

            if (button == MouseButtons.None || mouseHistory.Count < 2)
            {
                return;
            }

			var prev = mouseHistory[1];
			var cur = mouseHistory[0];
			var c = palette.VectorColor(prev, cur);

            if (button.HasFlag(MouseButtons.Left))
            {
                using (var b = new SolidBrush(c))
                {
                    graph.FillEllipse(b, cur.X - CCD / 2, cur.Y - CCD / 2, CCD, CCD);
                }
            }

			if (button.HasFlag(MouseButtons.Right))
			{
                using (var p = new Pen(c))
                {
                    graph.DrawEllipse(p, cur.X - CCD / 2, cur.Y - CCD / 2, CCD, CCD);
                }
            }

			if ((button & ~(MouseButtons.Left | MouseButtons.Right)) != MouseButtons.None)
			{
				using (var p = new Pen(c))
				{
                    graph.DrawRectangle(p, cur.X - CCD / 2, cur.Y - CCD / 2, CCD, CCD);
				}
			}
        }
    }
}
