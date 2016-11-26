using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SLAM.ViewModels.Converters {

    public class BoolToVisibilityConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if ((bool)value) {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            switch ((Visibility)value) {
                case Visibility.Visible:
                    return true;
                case Visibility.Hidden:
                case Visibility.Collapsed:
                default:
                    return false;
            }
        }
    }

    public class BoolToVisibilityConverterInverse : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if ((bool)value) {
                return Visibility.Hidden;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            switch ((Visibility)value) {
                case Visibility.Visible:
                    return false;
                case Visibility.Hidden:
                case Visibility.Collapsed:
                default:
                    return true;
            }
        }
    }
}