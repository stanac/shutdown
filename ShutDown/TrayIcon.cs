using ShutDown.Data;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Threading;

namespace ShutDown
{
    public class TrayIcon : IDisposable
    {
        private NotifyIcon _ni;
        private Stream _iconStream;
        private bool _isShutDownInProgress;
        private DispatcherTimer _timer;
        private string _currentIcon;

        private const string IconPlainName = "icon.ico";
        private const string IconRedName = "icon_red.ico";
        private const string IconGreenName = "icon_green.ico";

        public TrayIcon()
        {
            _ni = new NotifyIcon();
            _ni.Visible = true;
            _ni.Icon = PreventShutDownHelper.PreventShutDown ? GetIcon(IconGreenName) : GetIcon(IconRedName);
            _ni.BalloonTipTitle = "Shut down";
            _ni.BalloonTipText = "Shut down is in progress";
            _ni.Text = "Shut Down";
            SetContextMenu(false);
            _isShutDownInProgress = false;
            _ni.DoubleClick += (s, e) =>
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
            _ni.Icon = GetIcon(IconPlainName);
            
            SetContextMenu(isShutDownInProgress);
        }

        private System.Drawing.Icon GetIcon(string iconName)
        {
            _currentIcon = iconName;
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
            if (_isShutDownInProgress && Settings.Instance.BlinkTrayIcon)
            {
                if (_currentIcon != IconRedName) _ni.Icon = GetIcon(IconRedName);
                else if (PreventShutDownHelper.PreventShutDown) _ni.Icon = GetIcon(IconGreenName);
                else _ni.Icon = GetIcon(IconPlainName);
            }
            else
            {
                if (PreventShutDownHelper.PreventShutDown)
                {
                    if (_currentIcon != IconRedName) _ni.Icon = GetIcon(IconGreenName);
                }
                else
                {
                    if (_currentIcon != IconPlainName) _ni.Icon = GetIcon(IconPlainName);
                }
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
