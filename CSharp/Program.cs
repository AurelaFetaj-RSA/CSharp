using ProductionLaunch.Properties;
using RSACommon;
using RSACommon.GraphicsForm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductionLaunch
{
    static class Program
    {
        static SplashScreen SplashScreen;
        static FormApp MainForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LidorSystems.IntegralUI.Containers.TabControl.License(LidorLicenseKey.Key);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //check previous instance
            // get the name of our process
            string proc = Process.GetCurrentProcess().ProcessName;
            // get the list of all processes by that name
            Process[] processes = Process.GetProcessesByName(proc);
            // if there is more than one process...
            if (processes.Length > 1)
            {
                MessageBox.Show("Application already runs!");
                //exit application
                Environment.Exit(0);
            }
            else
            {

            }

            SplashScreen = new SplashScreen(Settings.Default.SplashScreenFilepath, 500, 232);
            var splashThread = new Thread(new ThreadStart(
                () => Application.Run(SplashScreen)));
            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.Start();

            //Create and Show Main Form
            MainForm = new FormApp(SplashScreen);
            MainForm.Load += MainForm_LoadCompleted;
            Application.Run(MainForm);
            Environment.Exit(0);
        }



        private static void MainForm_LoadCompleted(object sender, EventArgs e)
        {
            if (SplashScreen != null && !SplashScreen.Disposing && !SplashScreen.IsDisposed)
                SplashScreen.Invoke(new Action(() => SplashScreen.Close()));

            Thread.Sleep(2000);

            MainForm.TopMost = true;
            MainForm.Activate();
            MainForm.TopMost = false;

            MainForm.StartUpdateTask();
        }
    }
}
