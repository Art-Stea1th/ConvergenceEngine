using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace SLAM.Views.Converters {

    public class IntToVisibilityConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int)value == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class IntToVisibilityConverterInverse : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int)value == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}