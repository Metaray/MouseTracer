﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MouseTracer.Palettes;
using System.Diagnostics;

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

        private double fadeTravelCounter = 0;

        private Stopwatch noMovementTimer = new Stopwatch();

        private bool running = false;

        public bool FadeOverTime { get; set; } = true;

        public bool DrawClicks { get; set; } = true;

        public bool DrawMouseMove { get; set; } = true;

        public bool DrawMouseStops { get; set; } = true;

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
                noMovementTimer.Restart();
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

            if (FadeOverTime)
            {
                DoFadeImage();
            }

            if (DrawMouseMove)
            {
                DoDrawMouseMove();
            }

            if (DrawClicks)
            {
                DoDrawMouseClick();
            }

            if (DrawMouseStops)
            {
                DoDrawMouseStops();
            }
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

        private void DoDrawMouseStops()
        {
            const float MinNoMoveTime = 5.0f;
            const float HalfRadiusTime = 180.0f;
            const float MinRadius = 10.0f;
            const float MaxRadius = 500.0f;

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

            var delta = noMovementTimer.ElapsedMilliseconds / 1000.0f;
            noMovementTimer.Restart();

            if (delta < MinNoMoveTime)
            {
                return;
            }

            var t = delta - MinNoMoveTime;
            //var f = 1 - 1 / (1 + t / HalfRadiusTime);
            var f = 1 - Math.Exp(-t / (HalfRadiusTime / Math.Log(2)));
            var r = MinRadius + (float)f * (MaxRadius - MinRadius);
            var c = palette.VectorColor(prev, cur);

            using (var p = new Pen(c))
            {
                graph.DrawEllipse(p, cur.X - r, cur.Y - r, r * 2, r * 2);
            }
        }

        private void DoDrawMouseClick()
        {
			const float CCD = 15; // click circle diameter

            if (movesHistory.Count < 2)
            {
                return;
            }

            var buttons = movesHistory[0].Buttons & ~movesHistory[1].Buttons;

            if (buttons == MouseButtons.None)
            {
                return;
            }

            var prev = movesHistory[1].Position;
			var cur = movesHistory[0].Position;
			var c = palette.VectorColor(prev, cur);

            if (buttons.HasFlag(MouseButtons.Left))
            {
                var r = CCD / 2;
				using (var b = new SolidBrush(c))
                {
                    graph.FillEllipse(b, cur.X - r, cur.Y - r, r * 2, r * 2);
                }
            }

			if (buttons.HasFlag(MouseButtons.Right))
			{
                var w = 2;
                var r = CCD / 2 - w;
                using (var p = new Pen(c, w))
                {
                    graph.DrawEllipse(p, cur.X - r, cur.Y - r, r * 2, r * 2);
                }
            }

			if ((buttons & ~(MouseButtons.Left | MouseButtons.Right)) != MouseButtons.None)
			{
                var w = 2;
                var r = CCD / 2 - w;
                using (var p = new Pen(c, w))
				{
                    graph.DrawRectangle(p, cur.X - r, cur.Y - r, r * 2, r * 2);
				}
			}
        }

        private void DoFadeImage()
        {
            if (movesHistory.Count < 2)
            {
                return;
            }

            const double FadeTriggerDistMult = 200;
            const double FadeoutStrength = 0.15;

            var p1 = movesHistory[1].Position;
            var p2 = movesHistory[0].Position;
            fadeTravelCounter += Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
            if (fadeTravelCounter < Math.Min(screenBounds.Width, screenBounds.Height) * FadeTriggerDistMult)
            {
                return;
            }
            fadeTravelCounter = 0;

            var c = palette.Background;
            using (var b = new SolidBrush(Color.FromArgb((int)(255 * FadeoutStrength), c.R, c.G, c.B)))
            {
                graph.FillRectangle(b, 0, 0, screenBounds.Width, screenBounds.Height);
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
