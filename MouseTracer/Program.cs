using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MouseTracer
{
    internal static class Program
    {
        public static MouseHook MouseHook;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Debugger.IsAttached)
            {
                MouseHook = new PollingHook(16);
            }
            else
            {
                MouseHook = new LowLevelHook(5);
            }
            
			MouseHook.Start();
			Application.Run(new MainWindow());
            MouseHook.Stop();
        }
    }
}
