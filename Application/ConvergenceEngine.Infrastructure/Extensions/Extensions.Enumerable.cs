using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;


namespace ConvergenceEngine.Infrastructure.Extensions {

    public static partial class Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence) => sequence == null || sequence.Count() < 1;

        public static double AverageWeighted<TSource>(
            this IEnumerable<TSource> sequence, Func<TSource, double> valueSelector, Func<TSource, double> weightSelector) {
            double weightsSum = sequence.Sum(w => weightSelector.Invoke(w));
            return sequence.Select(s => valueSelector(s) * weightSelector(s) / weightsSum).Sum();
        }

        public static IEnumerable<TResult> Sequential<TSource, TAnother, TResult>(
            this IEnumerable<TSource> sourceSequence, IEnumerable<TAnother> anotherSequence,
            Func<TSource, TAnother, TResult> selector) {

            var ssEnumerator = sourceSequence.GetEnumerator();
            var asEnumerator = anotherSequence.GetEnumerator();

            while (ssEnumerator.MoveNext() && asEnumerator.MoveNext()) {
                yield return selector(ssEnumerator.Current, asEnumerator.Current);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource MaxBy<TSource, TComparable>(
            this IEnumerable<TSource> sequence, Func<TSource, TComparable> selector) where TComparable : IComparable {
            return sequence.MinOrMaxBy(selector, value => value > 0).element;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource MinBy<TSource, TComparable>(
            this IEnumerable<TSource> sequence, Func<TSource, TComparable> selector) where TComparable : IComparable {
            return sequence.MinOrMaxBy(selector, value => value < 0).element;
        }

        private static (int index, TSource element) MinOrMaxBy<TSource, TComparable>(this IEnumerable<TSource> sequence,
            Func<TSource, TComparable> selector, Predicate<int> lessOrMoreThanZero) where TComparable : IComparable {

            var enumerator = sequence.GetEnumerator();
            enumerator.MoveNext();

            int minOrMaxIndex = 0;
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
            return (index: minOrMaxIndex, element: minOrMaxSource);
        }

        private static IEnumerable<(int index, TSource element)> AllIndexes<TSource>(
            this IEnumerable<TSource> sequence, Func<TSource, bool> predicate) {

            var enumerator = sequence.GetEnumerator();

            var res = sequence.Select((e, i) => (index: i, element: e)).Where(e => );

            for (int i = 0; enumerator.MoveNext(); ++i) {
                var current = enumerator.Current;
                if (predicate(current)) {
                    yield return (index: i, element: current);
                }
            }
        }
    }    
}