using System;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace MouseTracer
{
    public class StatCollector : IDisposable
    {
        private bool running = false;

        private double traveledPx = 0.0;
        private uint leftClicks = 0;
        private uint rightClicks = 0;
        private readonly Stopwatch runTimeCounter = new Stopwatch();

        private bool hasPreviousPoint = false;
        private int prevX = 0;
        private int prevY = 0;
        private MouseButtons prevButtons;

        public StatCollector() { }

        public void Dispose()
        {
            SetRunning(false);
        }

        public void SetRunning(bool run)
        {
            if (running == run) return;
            
            if (run)
            {
                runTimeCounter.Start();
                hasPreviousPoint = false;
                Program.MouseHook.MouseAction += DoMouseEvent;
            }
            else
            {
                runTimeCounter.Stop();
				Program.MouseHook.MouseAction -= DoMouseEvent;
            }

            running = run;
        }

        private void DoMouseEvent(object sender, MouseStateEventArgs e)
        {
            if (hasPreviousPoint)
            {
                traveledPx += Math.Sqrt((e.X - prevX) * (e.X - prevX) + (e.Y - prevY) * (e.Y - prevY));

                var pressed = e.Buttons & ~prevButtons;

                if (pressed.HasFlag(MouseButtons.Left))
                {
                    leftClicks++;
                }

                if (pressed.HasFlag(MouseButtons.Right))
                {
                    rightClicks++;
                }
            }
            else
            {
                hasPreviousPoint = true;
            }

            prevX = e.X;
            prevY = e.Y;
            prevButtons = e.Buttons;
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
