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

		private readonly List<MouseState> movesHistory = new List<MouseState>();

        private MouseButtons prevButtons;

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
                movesHistory.Clear();
                prevButtons = MouseButtons.None;
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
                DoDrawMouseClick(e.Buttons & ~prevButtons);
            }

            prevButtons = e.Buttons;
        }

		private void UpdateMouseHistory(MouseStateEventArgs e)
		{
			var pos = e.Position;
			pos.X -= screenBounds.X;
			pos.Y -= screenBounds.Y;

			if (movesHistory.Count == 0 || movesHistory[0].Position != pos)
            {
                movesHistory.Insert(0, new MouseState(pos, e.Buttons));
                if (movesHistory.Count > HistLength)
                {
                    movesHistory.RemoveAt(HistLength);
                }
            }
		}

		private void DoDrawMouseMove()
        {
            if (movesHistory.Count < 2)
            {
                return;
            }

            var prev = movesHistory[1].Position;
			var cur = movesHistory[0].Position;
            if (prev == cur)
            {
                return;
            }

			var c = palette.VectorColor(prev, cur);
            float w = (movesHistory[0].Buttons != MouseButtons.None) && (movesHistory[1].Buttons != MouseButtons.None)
                ? 3.0f : 1.0f;

			using (var p = new Pen(c, w))
            {
                graph.DrawLine(p, prev, cur);
            }
        }

        private void DoDrawMouseClick(MouseButtons button)
        {
			const float CCD = 15; // click circle diameter

            if (button == MouseButtons.None || movesHistory.Count < 2)
            {
                return;
            }

			var prev = movesHistory[1].Position;
			var cur = movesHistory[0].Position;
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

        private class MouseState
        {
            public readonly Point Position;

            public readonly MouseButtons Buttons;

            public MouseState(Point position, MouseButtons buttons)
            {
                Position = position;
                Buttons = buttons;
            }
        }
    }
}
