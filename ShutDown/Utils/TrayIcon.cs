using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Threading;
using ShutDown.Data;

namespace ShutDown.Utils
{
    public class TrayIcon : IDisposable
    {
        private NotifyIcon _ni;
        private Stream _iconStream;
        private bool _isShutDownInProgress;
        private DispatcherTimer _timer;
        private string _currentIcon;

        private System.Drawing.Icon _greenIcon;
        private System.Drawing.Icon _redIcon;
        private System.Drawing.Icon _orangeIcon;
        
        private const string IconOrangeName = "icon.ico";
        private const string IconRedName = "icon_red.ico";
        private const string IconGreenName = "icon_green.ico";

        public TrayIcon()
        {
            _orangeIcon = GetIcon(IconOrangeName);
            _redIcon = GetIcon(IconRedName);
            _greenIcon = GetIcon(IconGreenName);

            _ni = new NotifyIcon();
            _ni.Visible = true;
            _ni.Icon = SettingsData.Instance.PreventShutDown ? _greenIcon : _redIcon;
            _ni.BalloonTipTitle = "Shut down";
            _ni.BalloonTipText = "Shut down is in progress";
            _ni.Text = "Shut Down";
            SetContextMenu(false);
            _isShutDownInProgress = false;
            _ni.Click += (s, e) =>
            {
                OnOpen?.Invoke();
            };

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            _timer.Tick += (s, e) =>
            {
                OnTimerTick();
            };
            _timer.Start();

            SetShutDownInProgress(false);
        }

        public Action OnExit { get; set; }
        public Action OnCancel { get; set; }
        public Action OnOpen { get; set; }

        public void ShowNotification(string text)
        {
            _ni.BalloonTipText = text;
            _ni.ShowBalloonTip(5 * 1000);
        }

        public void SetShutDownInProgress(bool isShutDownInProgress)
        {
            _isShutDownInProgress = isShutDownInProgress;
            _ni.Icon = SettingsData.Instance.PreventShutDown ? _greenIcon : _orangeIcon;
            
            SetContextMenu(isShutDownInProgress);
        }

        private System.Drawing.Icon GetIcon(string iconName)
        {
            if (_iconStream != null)
            {
                _iconStream.Dispose();
            }
            _iconStream = System.Windows.Application.GetResourceStream(new Uri(iconName, UriKind.Relative)).Stream;

            return new System.Drawing.Icon(_iconStream);
        }
        
        private void SetContextMenu(bool isShutDownInProgress)
        {
            var cm = new ContextMenu();

            var openMi = new MenuItem();
            openMi.Text = "Open";
            openMi.Click += (s, e) => { OnOpen?.Invoke(); };
            cm.MenuItems.Add(openMi);

            if (isShutDownInProgress)
            {
                var cancelMi = new MenuItem();
                cancelMi.Text = "Cancel delayed operation";
                cancelMi.Click += (s, e) => { OnCancel?.Invoke(); };
                cm.MenuItems.Add(1, cancelMi);
            }
            
            var closeMi = new MenuItem();
            closeMi.Text = "Exit";
            closeMi.Click += (s, e) => { OnExit?.Invoke(); };
            cm.MenuItems.Add(closeMi);

            if (_ni.ContextMenu != null)
            {
                _ni.ContextMenu.Dispose();
            }
            _ni.ContextMenu = cm;
        }

        private void OnTimerTick()
        {
            if (_isShutDownInProgress && SettingsData.Instance.BlinkTrayIcon)
            {
                if (IsCurrentIconRed)
                {
                    if (SettingsData.Instance.PreventShutDown) SetGreenIcon();
                    else SetOrangeIcon();
                }
                else
                {
                    SetRedIcon();
                }
            }
            else
            {
                if (SettingsData.Instance.PreventShutDown)
                {
                    SetGreenIcon();
                }
                else
                {
                    SetOrangeIcon();
                }
            }
        }

        private bool IsCurrentIconRed => _currentIcon == IconRedName;

        private void SetGreenIcon()
        {
            if (_currentIcon != IconGreenName)
            {
                _ni.Icon = _greenIcon;
                _currentIcon = IconGreenName;
            }
        }

        private void SetRedIcon()
        {
            if (_currentIcon != IconRedName)
            {
                _ni.Icon = _redIcon;
                _currentIcon = IconRedName;
            }
        }

        private void SetOrangeIcon()
        {
            if (_currentIcon != IconOrangeName)
            {
                _ni.Icon = _orangeIcon;
                _currentIcon = IconOrangeName;
            }
        }

        public void Dispose()
        {
            _ni.Visible = false;
            _ni.Dispose();
            _iconStream.Dispose();
        }
    }
}
