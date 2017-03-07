using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ConvergenceEngine.Views.AppConverters {

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if ((bool)value) {
                return Visibility.Visible;
            }
            if (parameter != null && (bool)parameter) {
                return Visibility.Collapsed;
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

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverterInverse : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if ((bool)value) {
                return Visibility.Hidden;
            }
            if (parameter != null && (bool)parameter) {
                return Visibility.Collapsed;
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