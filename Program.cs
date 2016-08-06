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

            // Double check that we're running with elevated privileges, otherwise we can't control any services
            if (!OculusUtil.IsElevated)
            {
                OculusUtil.ElevateMe();
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

        private static void StartTrayIcon()
        {
            if (!OculusUtil.IsElevated)
                return;

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
