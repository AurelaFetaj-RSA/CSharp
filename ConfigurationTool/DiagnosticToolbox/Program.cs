using DiagnosticTool;
using DiagnosticToolbox.Properties;
using RSACommon.GraphicsForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiagnosticToolbox
{
    internal static class Program
    {
        static SplashScreen SplashScreen;
        static DiagnosticToolForm MainForm;

        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SplashScreen = new SplashScreen(Settings.Default.SplashScreenFilepath, 300, 200);
            var splashThread = new Thread(new ThreadStart(
                () => Application.Run(SplashScreen)));
            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.Start();

            //Create and Show Main Form
            MainForm = new DiagnosticToolForm(SplashScreen);
            MainForm.Load += MainForm_LoadCompleted;
            Application.Run(MainForm);
        }

        private static void MainForm_LoadCompleted(object sender, EventArgs e)
        {
            if (SplashScreen != null && !SplashScreen.Disposing && !SplashScreen.IsDisposed)
                SplashScreen.Invoke(new Action(() => SplashScreen.Close()));

            MainForm.TopMost = true;
            MainForm.Activate();
            MainForm.TopMost = false;

            MainForm.StartUpdateGuiTask();
        }

    }
}
