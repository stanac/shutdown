using HandyControl.Controls;
using ShutDown.Data;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ShutDown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBorderless
    {
        public MainWindow()
        {
            InitializeComponent();
            
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && args[1] == "/winstart" && Settings.Instance.CloseToTray)
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
                icon.Dispose();
                Close();
            };
            icon.OnOpen = () => { Show(); };
            icon.OnCancel = () => { vm.CancelShutDownCommand.Execute(null); };

            base.OnInitialized(e);
        }

        private void HelpBtnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("http://stanac.github.io/shutdown/");
        }

        private void WindowBorderless_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = (DataContext as MainViewModel);
            if (Settings.Instance.CloseToTray)
            {
                Hide();
                if (vm.ShutDownInProgress)
                {
                    var message = vm.OperationName.Replace("in:", "").Trim() + " is still in progress.";
                    vm.TheTrayIcon.ShowNotification(message);
                }
            }
            else
            {
                vm.TheTrayIcon.Dispose();
                Environment.Exit(0);
            }
        }
    }
}
