using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SLAM.Views.Converters {

    [ValueConversion(typeof(int), typeof(bool))]
    public class IntToBoolConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int)value != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}