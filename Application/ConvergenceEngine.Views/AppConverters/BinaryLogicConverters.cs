using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace ConvergenceEngine.Views.AppConverters {

    public abstract class BinaryLogicIntegerConverter<TTarget> : BinaryLogicConverter<int, TTarget> {

        public TTarget NonZero { get { return nonZeroTarget; } set { nonZeroTarget = value; } }
        public TTarget Zero { get { return zeroTarget; } set { zeroTarget = value; } }

        public BinaryLogicIntegerConverter(TTarget nonZeroTarget, TTarget zeroTarget)
            : base(1, 0, nonZeroTarget, zeroTarget) { }

    }

    public abstract class BinaryLogicBooleanConverter<TTarget> : BinaryLogicConverter<bool, TTarget> {

        public TTarget True { get { return nonZeroTarget; } set { nonZeroTarget = value; } }
        public TTarget False { get { return zeroTarget; } set { zeroTarget = value; } }

        public BinaryLogicBooleanConverter(TTarget trueValue, TTarget falseValue)
            : base(true, false, trueValue, falseValue) { }
    }

    public abstract class BinaryLogicObjectConverter<TTarget> : BinaryLogicConverter<object, TTarget> {

        public TTarget NotNull { get { return nonZeroTarget; } set { nonZeroTarget = value; } }
        public TTarget Null { get { return zeroTarget; } set { zeroTarget = value; } }

        public BinaryLogicObjectConverter(TTarget notNullValue, TTarget nullValue)
            : base(null, null, notNullValue, nullValue) { }
    }

    public abstract class BinaryLogicConverter<TSource, TTarget> : IValueConverter {

        protected TSource nonZeroSource;
        protected TSource zeroSource;

        protected TTarget nonZeroTarget;
        protected TTarget zeroTarget;

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is TSource && !EqualityComparer<TSource>.Default.Equals((TSource)value, zeroSource)
                ? nonZeroTarget : zeroTarget;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is TTarget && EqualityComparer<TTarget>.Default.Equals((TTarget)value, nonZeroTarget)
                ? nonZeroSource : zeroSource;
        }

        protected BinaryLogicConverter(TSource nonZeroSource, TSource zeroSource, TTarget nonZeroTarget, TTarget zeroTarget) {
            this.zeroSource = zeroSource; this.nonZeroSource = nonZeroSource;
            this.zeroTarget = zeroTarget; this.nonZeroTarget = nonZeroTarget;
        }
    }    
}