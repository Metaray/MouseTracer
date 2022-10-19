using System;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace MouseTracer
{
    public abstract class MouseHook
    {
        private const int SendIntervalMs = 50;
        
        private const uint MaxQueuedEvents = 128;
        
        private readonly ConcurrentQueue<MouseEventArgs> hookEvents = new ConcurrentQueue<MouseEventArgs>();

        private readonly Timer sendTimer;

		public event EventHandler<MouseEventArgs> MouseAction;

		public MouseHook()
        {
            sendTimer = new Timer
            {
                Interval = SendIntervalMs
            };
            sendTimer.Tick += SendTimer_Tick;
        }

        private void SendTimer_Tick(object sender, EventArgs e)
        {
            while (hookEvents.TryDequeue(out var evt))
            {
                NotifyMouseAction(evt);
            }
        }

        protected virtual void NotifyMouseAction(MouseEventArgs args)
        {
            MouseAction?.Invoke(null, args);
        }

        public virtual void Start()
        {
            sendTimer.Start();
        }

        public virtual void Stop()
        {
            sendTimer.Stop();
        }

        protected void EnqueueNewEvent(MouseEventArgs eventArgs)
        {
            // Drop events if consumer thread stalled to prevent runaway memory growth
            // Max events >= Mouse polling frequency * Consumer polling interval
            if (hookEvents.Count < MaxQueuedEvents)
            {
                hookEvents.Enqueue(eventArgs);
            }
        }
	}
}
