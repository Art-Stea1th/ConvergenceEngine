using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;


namespace ConvergenceEngine.Infrastructure.Extensions {

    public static partial class Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence) {
            return sequence == null || sequence.Count() < 1;
        }

        public static double AverageWeighted<TSource>(this IEnumerable<TSource> sequence,
            Func<TSource, double> valueSelector, Func<TSource, double> weightSelector) {
            var weightsSum = sequence.Sum(w => weightSelector.Invoke(w));
            return sequence.Select(s => valueSelector(s) * weightSelector(s) / weightsSum).Sum();
        }

        public static IEnumerable<TResult> Sequential<TSource, TAnother, TResult>(this IEnumerable<TSource> sourceSequence,
            IEnumerable<TAnother> anotherSequence, Func<TSource, TAnother, TResult> selector) {

            var ssEnumerator = sourceSequence.GetEnumerator();
            var asEnumerator = anotherSequence.GetEnumerator();

            while (ssEnumerator.MoveNext() && asEnumerator.MoveNext()) {
                yield return selector(ssEnumerator.Current, asEnumerator.Current);
            }
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource MaxBy<TSource, TComparable>(this IEnumerable<TSource> sequence,
           Func<TSource, TComparable> selector) where TComparable : IComparable {
            return sequence.MinOrMaxBy(selector, value => value > 0).Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource MinBy<TSource, TComparable>(this IEnumerable<TSource> sequence,
            Func<TSource, TComparable> selector) where TComparable : IComparable {
            return sequence.MinOrMaxBy(selector, value => value < 0).Value;
        }

        private static KeyValuePair<int, TSource> MinOrMaxBy<TSource, TComparable>(this IEnumerable<TSource> sequence,
            Func<TSource, TComparable> selector, Predicate<int> lessOrMoreThanZero) where TComparable : IComparable {

            var enumerator = sequence.GetEnumerator();
            enumerator.MoveNext();

            var minOrMaxIndex = 0;
            var minOrMaxSource = enumerator.Current;
            var minOrMaxComparable = selector(minOrMaxSource);

            for (int i = 1; enumerator.MoveNext(); ++i) {
                var currentSource = enumerator.Current;
                var currentComparable = selector(currentSource);
                if (lessOrMoreThanZero(currentComparable.CompareTo(minOrMaxComparable))) {
                    minOrMaxComparable = currentComparable;
                    minOrMaxSource = currentSource;
                    minOrMaxIndex = i;
                }
            }
            return new KeyValuePair<int, TSource>(minOrMaxIndex, minOrMaxSource);
        }
    }    
}