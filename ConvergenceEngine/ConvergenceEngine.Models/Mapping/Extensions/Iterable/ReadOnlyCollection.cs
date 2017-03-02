using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;


namespace ConvergenceEngine.Models.Mapping.Extensions.Iterable {

    internal static partial class Extensions {

        public static Tuple<IEnumerable<T>, IEnumerable<T>> SplitBy<T>(this IReadOnlyCollection<T> sequence, int index) {

            if (sequence.Count < 3) { throw new InvalidOperationException(); }
            if (index == 0 || index == sequence.Count - 1) { throw new ArgumentException(); }
            if (index < 0 || index >= sequence.Count) { throw new ArgumentOutOfRangeException(); }

            IEnumerable<T> left = sequence.TakeWhile((p, i) => i <= index);
            IEnumerable<T> right = sequence.SkipWhile((p, i) => i < index);

            return new Tuple<IEnumerable<T>, IEnumerable<T>>(left, right);
        }

    }    
}