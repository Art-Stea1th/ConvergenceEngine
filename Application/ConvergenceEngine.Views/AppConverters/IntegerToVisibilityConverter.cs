using System.Windows;
using System.Windows.Data;

namespace ConvergenceEngine.Views.AppConverters {

    [ValueConversion(typeof(int), typeof(Visibility))]
    public sealed class IntegerToVisibilityConverter : BinaryLogicIntegerConverter<Visibility> {
        public IntegerToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed) { }
    }
}