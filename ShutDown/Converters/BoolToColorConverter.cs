using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ShutDown.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        private static readonly Brush _selected = Brushes.DodgerBlue;
        private static readonly Brush _notSelected = Brushes.Silver;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? _selected : _notSelected;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
