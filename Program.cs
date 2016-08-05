using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OculusTray.Properties;

namespace OculusTray
{
    static class Program
    {
        private static bool IsElevated => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Double check that we're running with elevated privileges, otherwise we can't control any services
            if (!IsElevated)
            {
                string executable = Process.GetCurrentProcess().MainModule.FileName;
                var startInfo = new ProcessStartInfo(executable) { Verb = @"runas" };
                Process.Start(startInfo);
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
            if (!IsElevated)
                return;

            var oculusService = OculusUtil.FindOculusService();
            if (oculusService == null)
            {
                Error(Resources.Error_VR_Service_Not_Found);
                return;
            }

            var oculusClientPath = OculusUtil.GetOculusClientPath();
            if (oculusClientPath == null)
            {
                Error(Resources.Error_Oculus_Client_Not_Found);
                return;
            }
            
            using (new OculusTrayIcon(oculusService, oculusClientPath))
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
