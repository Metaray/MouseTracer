using System;
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

            MouseHook = new LowLevelHook();
            
            MouseHook.Start();
            Application.Run(new MainWindow());
            MouseHook.Stop();
        }
    }
}
