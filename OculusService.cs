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

        private ServiceControllerStatus? _temporaryStatus;

        public OculusService(ServiceController service)
        {
            _service = service;
        }

        public ServiceControllerStatus Status
        {
            get
            {
                if (_temporaryStatus.HasValue)
                    return _temporaryStatus.Value;

                _service.Refresh();
                return _service.Status;
            }
        }

        public void Start()
        {
            if (OculusUtil.IsElevated)
            {
                _service.Start();
                _service.WaitForStatus(ServiceControllerStatus.Running);
            }
            else
            {
                _temporaryStatus = ServiceControllerStatus.StartPending;
                OculusUtil.ElevateMe("start", true);
                _temporaryStatus = null;
            }
        }

        public void Stop()
        {
            if (OculusUtil.IsElevated)
            {
                _service.Stop();
                _service.WaitForStatus(ServiceControllerStatus.Stopped);
            }
            else
            {
                _temporaryStatus = ServiceControllerStatus.StopPending;
                OculusUtil.ElevateMe("stop", true);
                _temporaryStatus = null;
            }
        }

        public void Restart()
        {
            if (OculusUtil.IsElevated)
            {
                Stop();
                Start();
            }
            else
            {
                _temporaryStatus = ServiceControllerStatus.StartPending;
                OculusUtil.ElevateMe("restart", true);
                _temporaryStatus = null;
            }
        }
    }
}
