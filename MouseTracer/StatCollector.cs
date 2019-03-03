using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MouseTracer
{
    class StatCollector : IDisposable
    {
        private bool running = false;
        private bool hasPrevious = false;

        private double traveledPx = 0.0;
        private uint leftClicks = 0, rightClicks = 0;

        private int pvX = -1, pvY = -1;

        public StatCollector()
        {
            MouseHook.MouseAction += DoMouseEvent;
        }

        public void Dispose()
        {
            MouseHook.MouseAction -= DoMouseEvent;
        }

        public void SetRunning(bool state)
        {
            if (running != state)
            {
                hasPrevious = false;
            }
            running = state;
        }

        private void DoMouseEvent(object sender, MouseEventArgs e)
        {
            if (!running)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                leftClicks++;
            }
            else if (e.Button == MouseButtons.Right)
            {
                rightClicks++;
            }

            if (hasPrevious)
            {
                traveledPx += Math.Sqrt(Math.Pow(pvX - e.X, 2) + Math.Pow(pvY - e.Y, 2));
            }
            else
            {
                hasPrevious = true;
            }
            pvX = e.X;
            pvY = e.Y;
        }

        public void DisplayStats()
        {
            MessageBox.Show(
                $"Distance traveled: {(int)traveledPx}px\n" +
                $"Left clicks: {leftClicks}\n" +
                $"Right clicks: {rightClicks}",

                "Statistics"
            );
        }
    }
}
