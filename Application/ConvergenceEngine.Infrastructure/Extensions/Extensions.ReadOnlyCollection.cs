using System;
using System.Collections.Generic;
using System.Linq;

namespace ConvergenceEngine.Infrastructure.Extensions {

    public static partial class Extensions {

        public static (IEnumerable<T> left, IEnumerable<T> right) SplitBy<T>(this IReadOnlyCollection<T> sequence, int index) {

            if (sequence.Count < 3) { throw new InvalidOperationException(); }
            if (index == 0 || index == sequence.Count - 1) { throw new ArgumentException(); }
            if (index < 0 || index >= sequence.Count) { throw new ArgumentOutOfRangeException(); }

            return (
                left: sequence.TakeWhile((p, i) => i <= index),
                right: sequence.SkipWhile((p, i) => i < index));
        }
    }    
}