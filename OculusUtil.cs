using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OculusTray
{
    public static class OculusUtil
    {
        private const string OculusServiceName = @"OVRService";
        private const string OculusBaseVarName = @"OculusBase";
        private const string OculusClientRelativePath = @"Support\oculus-client\OculusClient.exe";

        public static ServiceController FindOculusService()
        {
            var services = ServiceController.GetServices();
            return services.FirstOrDefault(svc => svc.ServiceName == OculusServiceName);
        }

        public static DirectoryInfo GetOculusBaseDirectory()
        {
            string oculusBasePath = Environment.GetEnvironmentVariable(OculusBaseVarName);
            if (oculusBasePath == null)
                return null;

            return Directory.Exists(oculusBasePath) ? new DirectoryInfo(oculusBasePath) : null;
        }

        public static FileInfo GetOculusClientPath()
        {
            var baseDir = GetOculusBaseDirectory();
            if (baseDir == null)
                return null;

            var clientPath = Path.Combine(baseDir.FullName, OculusClientRelativePath);
            return File.Exists(clientPath) ? new FileInfo(clientPath) : null;
        }
    }
}
