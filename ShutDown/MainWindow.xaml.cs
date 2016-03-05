using ShutDown.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShutDown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
            (DataContext as MainViewModel).CloseApp += (s, ec) => { Close(); };
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

        private void AddPatternClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Coming up in v1.1", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void HelpBtnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("http://google.com");
        }
    }
}
