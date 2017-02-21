using System;
using System.Globalization;
using System.Windows.Data;


namespace ConvergenceEngine.Views.Converters {

    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolInverseConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return !(bool)value;
        }
    }
}