using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OculusTray.Properties;

namespace OculusTray
{
    public class OculusTrayIcon: IDisposable
    {
        private readonly OculusService _oculusService;
        private readonly FileInfo _oculusClientPath;
        private readonly NotifyIcon _notifyIcon;

        private bool _disposed;

        private ToolStripMenuItem _menuStart;
        private ToolStripMenuItem _menuStop;
        private ToolStripMenuItem _menuRestart;

        public OculusTrayIcon(OculusService oculusService, FileInfo oculusClientPath)
        {
            _oculusService = oculusService;
            _oculusClientPath = oculusClientPath;

            _notifyIcon = new NotifyIcon
            {
                ContextMenuStrip = CreateContextMenu(),
                Text = Resources.Oculus_VR_Service,
                Visible = true,
            };

            _notifyIcon.DoubleClick += ToggleService;

            UpdateStatus();
            PollServiceStatus();
        }
        
        private ContextMenuStrip CreateContextMenu()
        {
            var elevateImage = OculusUtil.IsElevated ? (Image)null : SystemIcons.Shield.ToBitmap();

            var menu = new ContextMenuStrip();
            menu.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(Resources.Menu_OpenClient, null, OnOpenOculusClient),
                new ToolStripSeparator(),
                _menuStart = new ToolStripMenuItem(Resources.Menu_Start, elevateImage, OnStart),
                _menuStop = new ToolStripMenuItem(Resources.Menu_Stop, elevateImage, OnStop),
                _menuRestart = new ToolStripMenuItem(Resources.Menu_Restart, elevateImage, OnRestart),
                new ToolStripSeparator(),
                new ToolStripMenuItem(Resources.Menu_Exit, null, OnExit),
            });

            _menuStart.Font = new Font(_menuStart.Font, FontStyle.Bold);
            _menuStop.Font = new Font(_menuStop.Font, FontStyle.Bold);

            return menu;
        }

        private void OnOpenOculusClient(object sender, EventArgs args)
        {
            Process.Start(_oculusClientPath.FullName);
        }

        private void OnStart(object sender, EventArgs args)
        {
            SetStarting();
            _oculusService.Start();
            UpdateStatus();
        }

        private void OnStop(object sender, EventArgs args)
        {
            SetStopping();
            _oculusService.Stop();
            UpdateStatus();
        }

        private void OnRestart(object sender, EventArgs args)
        {
            SetStopping();
            _oculusService.Restart();
            UpdateStatus();
        }
        
        private void OnExit(object sender, EventArgs args)
        {
            Application.Exit();
        }

        private void ToggleService(object sender, EventArgs args)
        {
            switch (_oculusService.Status)
            {
                case ServiceControllerStatus.Running:
                    OnStop(sender, args);
                    break;
                case ServiceControllerStatus.Stopped:
                    OnStart(sender, args);
                    break;
            }
        }

        private void UpdateStatus()
        {
            switch (_oculusService.Status)
            {
                case ServiceControllerStatus.Running:
                    SetRunning();
                    break;
                case ServiceControllerStatus.Stopped:
                case ServiceControllerStatus.Paused:
                    SetStopped();
                    break;
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.ContinuePending:
                    SetStarting();
                    break;
                case ServiceControllerStatus.StopPending:
                case ServiceControllerStatus.PausePending:
                    SetStopping();
                    break;
                default:
                    SetUnknown();
                    break;
            }
        }
        
        private void SetRunning()
        {
            _notifyIcon.Icon = Resources.Running;
            _notifyIcon.Text = $"{Resources.Oculus_VR_Service} {Resources.Status_Running}";

            _menuStart.Visible = false;
            _menuStop.Visible = _menuRestart.Visible = true;
        }

        private void SetStopped()
        {
            _notifyIcon.Icon = Resources.Stopped;
            _notifyIcon.Text = $"{Resources.Oculus_VR_Service} {Resources.Status_Stopped}";

            _menuStart.Visible = true;
            _menuStop.Visible = _menuRestart.Visible = false;
        }

        private void SetStarting()
        {
            _notifyIcon.Icon = Resources.Pending;
            _notifyIcon.Text = $"{Resources.Oculus_VR_Service} {Resources.Status_Starting}";

            _menuStart.Visible = _menuStop.Visible = _menuRestart.Visible = false;
        }

        private void SetStopping()
        {
            _notifyIcon.Icon = Resources.Pending;
            _notifyIcon.Text = $"{Resources.Oculus_VR_Service} {Resources.Status_Stopping}";

            _menuStart.Visible = _menuStop.Visible = _menuRestart.Visible = false;
        }

        private void SetUnknown()
        {
            _notifyIcon.Icon = Resources.Unknown;
            _notifyIcon.Text = Resources.Oculus_VR_Service;

            _menuStart.Visible = _menuStop.Visible = _menuRestart.Visible = true;
        }

        private async void PollServiceStatus()
        {
            while (!_disposed)
            {
                await Task.Delay(500);
                UpdateStatus();
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _notifyIcon.Dispose();
        }
    }
}
