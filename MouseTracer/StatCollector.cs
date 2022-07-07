using System;
using System.Text;
using System.Windows.Forms;

namespace MouseTracer
{
    public class StatCollector : IDisposable
    {
        private bool running = false;

        private double traveledPx = 0.0;
        private uint leftClicks = 0;
        private uint rightClicks = 0;
        private System.Diagnostics.Stopwatch runTimeCounter = new System.Diagnostics.Stopwatch();

        private bool hasPreviousPoint = false;
        private int prevX = 0;
        private int prevY = 0;

        public StatCollector()
        {
            MouseHook.MouseAction += DoMouseEvent;
        }

        public void Dispose()
        {
            MouseHook.MouseAction -= DoMouseEvent;
        }

        public void SetRunning(bool run)
        {
            if (running != run)
            {
                hasPreviousPoint = false;
            }

            if (run)
            {
                runTimeCounter.Start();
            }
            else
            {
                runTimeCounter.Stop();
            }

            running = run;
        }

        private void DoMouseEvent(object sender, MouseEventArgs e)
        {
            if (!running) return;

            if (e.Button.HasFlag(MouseButtons.Left))
            {
                leftClicks++;
            }
            if (e.Button.HasFlag(MouseButtons.Right))
            {
                rightClicks++;
            }

            if (hasPreviousPoint)
            {
                traveledPx += Math.Sqrt((e.X - prevX) * (e.X - prevX) + (e.Y - prevY) * (e.Y - prevY));
            }
            else
            {
                hasPreviousPoint = true;
            }
            prevX = e.X;
            prevY = e.Y;
        }

        public TimeSpan TimeTracing => runTimeCounter.Elapsed;

        public void DisplayStats()
        {
            var msg = new StringBuilder();
            
            msg.AppendLine($"Time spent tracing: {TimeTracing:h\\:mm\\:ss}");

            msg.AppendLine($"Distance traveled: {(int)traveledPx}px ({traveledPx / Utils.GetPxPerCm():F1} cm)");

            msg.AppendLine($"Left clicks: {leftClicks}");

            msg.Append($"Right clicks: {rightClicks}");

            MessageBox.Show(msg.ToString(), "Statistics");
        }
    }
}
