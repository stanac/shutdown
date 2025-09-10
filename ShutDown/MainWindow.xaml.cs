using ShutDown.Data;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ShutDown.Utils;

namespace ShutDown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _exiting;

        public MainWindow()
        {
            InitializeComponent();
            
            PreventShutDownHelper.Run();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && args[1] == "/winstart" && SettingsData.Instance.CloseToTray)
            {
                Hide();
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            MouseDown += (s, me) =>
            {
                if (me.ChangedButton == MouseButton.Left)
                {
                    DragMove();
                }
            };

            var vm = (DataContext as MainViewModel);
            vm.CloseApp += (s, ec) => { Close(); };

            var icon = vm.TheTrayIcon;
            icon.OnExit = () => 
            {
                _exiting = true;
                Close();
                icon.Dispose();
                
            };
            icon.OnOpen = () => { Show(); Application.Current.MainWindow.Activate(); };
            icon.OnCancel = () => { vm.CancelShutDownCommand.Execute(null); };

            NewVersionLink.RequestNavigate += (sender, args) =>
            {
                Process.Start(args.Uri.ToString());
            };

            Closing += (sender, args) => HandleOnClosing(args);

            base.OnInitialized(e);
        }

        private void MinBtnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseBtnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HelpBtnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/stanac/shutdown/");
        }

        private void HandleOnClosing(CancelEventArgs args)
        {
            var vm = DataContext as MainViewModel;

            if (vm == null) return;

            if (SettingsData.Instance.CloseToTray && !_exiting)
            {
                Hide();
                if (vm.ShutDownInProgress)
                {
                    var message = vm.OperationName.Replace("in:", "").Trim() + " is still in progress.";
                    vm.TheTrayIcon.ShowNotification(message);
                }

                args.Cancel = true;
            }
            else
            {
                vm.TheTrayIcon.Dispose();
                args.Cancel = false;
            }
        }
    }
}
