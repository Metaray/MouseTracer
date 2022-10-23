using System;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace MouseTracer
{
    public abstract class MouseHook
    {
        private const int SendIntervalMs = 50;
        
        private const uint MaxQueuedEvents = 128;
        
        private readonly ConcurrentQueue<MouseStateEventArgs> hookEvents = new ConcurrentQueue<MouseStateEventArgs>();

        private readonly Timer sendTimer;

        private bool initialized;

		public event EventHandler<MouseStateEventArgs> MouseAction;

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
                // Skip first event to prevent sending invalid previous state data
                if (initialized)
                {
                    NotifyMouseAction(evt);
                }
                else
                {
                    initialized = true;
                }
            }
        }

        private void NotifyMouseAction(MouseStateEventArgs args)
        {
            MouseAction?.Invoke(this, args);
        }

        public virtual void Start()
        {
            initialized = false;
            sendTimer.Start();
        }

        public virtual void Stop()
        {
            sendTimer.Stop();
        }

        // Asynchronous, thread safe
        protected void EnqueueNewEvent(MouseStateEventArgs eventArgs)
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
