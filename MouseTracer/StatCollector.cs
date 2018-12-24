using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MouseTracer
{
    class StatCollector
    {
        private bool running = false;
        private double distance = 0.0f;
        private uint lclicks = 0, rclicks = 0;
        private int prevx = -1, prevy = -1;

        public StatCollector()
        {
            MouseHook.MouseAction += DoMouseEvent;
        }

        public void SetRunning(bool state)
        {
            if (running != state)
            {
                prevx = -1;
                prevy = -1;
            }
            running = state;
        }

        private void DoMouseEvent(object sender, MouseEventArgs e)
        {
            if (!running) return;
            if (prevx != -1)
            {
                distance += Math.Sqrt(Math.Pow(prevx - e.X, 2) + Math.Pow(prevy - e.Y, 2));
            }
            if (e.Button == MouseButtons.Left) ++lclicks;
            if (e.Button == MouseButtons.Right) ++rclicks;
            prevx = e.X;
            prevy = e.Y;
        }

        public void DisplayStats()
        {
            MessageBox.Show(String.Format(
                "Distance traveled: {0}px\nLeft clicks: {1}\nRight clicks: {2}",
                (int)distance,
                lclicks,
                rclicks)
                , "Statistics");
        }
    }
}
