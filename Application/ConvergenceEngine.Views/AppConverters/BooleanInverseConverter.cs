using System.Windows.Data;


namespace ConvergenceEngine.Views.AppConverters {

    [ValueConversion(typeof(bool), typeof(bool))]
    public sealed class BooleanInverseConverter : BinaryLogicBooleanConverter<bool> {
        public BooleanInverseConverter() : base(false, true) { }
    }
}