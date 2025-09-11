using ShutDown.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ShutDown.Converters
{
    public class OperationToColorConverter : IValueConverter
    {
        private static readonly Brush _selected = Brushes.DodgerBlue;
        private static readonly Brush _notSelected = Brushes.Silver;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableProperty<ShutDownOperation> opv)
            {
                value = opv.Value;
            }

            var op = (ShutDownOperation)value;
            if (op.ToString().ToLower() == (parameter as string).ToLower())
            {
                return _selected;
            }
            return _notSelected;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
