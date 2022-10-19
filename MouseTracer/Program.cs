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

            DebuggingHook dbgHook = null;
            if (Debugger.IsAttached)
            {
                MouseHook = dbgHook = new DebuggingHook();
            }
            else
            {
                MouseHook = new LowLevelHook();
            }
            
            var window = new MainWindow();

            if (dbgHook != null)
            {
                dbgHook.TrackedWindow = window;
            }

			MouseHook.Start();
			Application.Run(window);
            MouseHook.Stop();
        }
    }
}
