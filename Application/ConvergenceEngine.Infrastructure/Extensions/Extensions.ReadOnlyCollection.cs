using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;


namespace ConvergenceEngine.Infrastructure.Extensions {

    public static partial class Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMaxBy<TSource, TComparable>(this IReadOnlyCollection<TSource> sequence,
            Func<TSource, TComparable> selector) where TComparable : IComparable {
            return sequence.MinOrMaxBy(selector, value => value > 0).Key;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfMinBy<TSource, TComparable>(this IReadOnlyCollection<TSource> sequence,
            Func<TSource, TComparable> selector) where TComparable : IComparable {
            return sequence.MinOrMaxBy(selector, value => value < 0).Key;
        }

        public static Tuple<IEnumerable<T>, IEnumerable<T>> SplitBy<T>(this IReadOnlyCollection<T> sequence, int index) {

            if (sequence.Count < 3) { throw new InvalidOperationException(); }
            if (index == 0 || index == sequence.Count - 1) { throw new ArgumentException(); }
            if (index < 0 || index >= sequence.Count) { throw new ArgumentOutOfRangeException(); }

            IEnumerable<T> left = sequence.TakeWhile((p, i) => i <= index);
            IEnumerable<T> right = sequence.SkipWhile((p, i) => i < index);

            return Tuple.Create(left, right);
        }
    }    
}