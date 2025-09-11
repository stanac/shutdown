using System.Windows.Controls;

namespace ShutDown.Views.Patterns
{
    /// <summary>
    /// Interaction logic for NewPatternControl.xaml
    /// </summary>
    public partial class NewPatternControl : UserControl
    {
        public NewPatternControl()
        {
            InitializeComponent();

            IsVisibleChanged += (_, e) =>
            {
                (DataContext as NewPatternViewModel)?.OnShown((bool)e.NewValue);
            };
        }
    }
}
