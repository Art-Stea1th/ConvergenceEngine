using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SLAM.ViewModels.Converters {

    public class IntToBoolConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int)value != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}