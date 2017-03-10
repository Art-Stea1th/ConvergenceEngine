using System.Windows;
using System.Windows.Data;

namespace ConvergenceEngine.Views.AppConverters {

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BooleanToVisibilityConverter : BinaryLogicBooleanConverter<Visibility> {
        public BooleanToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed) { }
    }
}