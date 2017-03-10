using System.Windows;
using System.Windows.Data;

namespace ConvergenceEngine.Views.AppConverters {

    [ValueConversion(typeof(object), typeof(Visibility))]
    public sealed class ObjectToVisibilityConverter : BinaryLogicObjectConverter<Visibility> {
        public ObjectToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed) { }
    }
}