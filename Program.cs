using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OculusTray.Properties;

namespace OculusTray
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string[] cmdArgs = Environment.GetCommandLineArgs();
            if (cmdArgs.Length > 1)
            {
                RunCommand(cmdArgs);
                return;
            }

            // We need to run in user interactive mode to be able to show a tray icon
            if (!Environment.UserInteractive)
            {
                Environment.ExitCode = 2;
                return;
            }

            StartTrayIcon();
        }

        private static void RunCommand(string[] args)
        {
            if (args.Length < 2)
                return;

            if (!OculusUtil.IsElevated)
                return;

            var service = OculusUtil.FindOculusService();
            if (service == null)
            {
                Error(Resources.Error_VR_Service_Not_Found);
                return;
            }

            var oculusService = new OculusService(service);

            switch (args[1].ToLowerInvariant())
            {
                case "start":
                    oculusService.Start();
                    break;
                case "stop":
                    oculusService.Stop();
                    break;
                case "restart":
                    oculusService.Restart();
                    break;
            }
        }

        private static void StartTrayIcon()
        {
            var service = OculusUtil.FindOculusService();
            if (service == null)
            {
                Error(Resources.Error_VR_Service_Not_Found);
                return;
            }

            var clientPath = OculusUtil.GetOculusClientPath();
            if (clientPath == null)
            {
                Error(Resources.Error_Oculus_Client_Not_Found);
                return;
            }
            
            using (new OculusTrayIcon(new OculusService(service), clientPath))
            {
                Application.Run();
            }
        }

        private static void Error(string errorMessage)
        {
            if (Environment.UserInteractive)
                MessageBox.Show(null, errorMessage, Resources.Oculus_VR_Service, MessageBoxButtons.OK, MessageBoxIcon.Error);

            Environment.ExitCode = 1;
        }
    }
}
