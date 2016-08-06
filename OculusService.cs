using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OculusTray
{
    public class OculusService
    {
        private readonly ServiceController _service;

        public OculusService(ServiceController service)
        {
            _service = service;
        }

        public ServiceControllerStatus Status
        {
            get
            {
                _service.Refresh();
                return _service.Status;
            }
        }

        public void Start()
        {
            _service.Start();
            _service.WaitForStatus(ServiceControllerStatus.Running);
        }

        public void Stop()
        {
            _service.Stop();
            _service.WaitForStatus(ServiceControllerStatus.Stopped);
        }

        public void Restart()
        {
            Stop();
            Start();
        }
    }
}
