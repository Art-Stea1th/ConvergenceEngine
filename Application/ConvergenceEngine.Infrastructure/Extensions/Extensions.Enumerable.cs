using System;
using System.Collections.Generic;
using System.Linq;

namespace ConvergenceEngine.Infrastructure.Extensions {

    public static partial class Extensions {

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence) => sequence == null || sequence.Count() < 1;

        public static TSource MaxBy<TSource, TComparable>(this IEnumerable<TSource> sequence,
            Func<TSource, TComparable> selector) where TComparable : IComparable {
            return sequence.MinOrMaxBy(selector, value => value > 0);
        }

        public static TSource MinBy<TSource, TComparable>(this IEnumerable<TSource> sequence,
            Func<TSource, TComparable> selector) where TComparable : IComparable {
            return sequence.MinOrMaxBy(selector, value => value < 0);
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

        private static TSource MinOrMaxBy<TSource, TComparable>(this IEnumerable<TSource> sequence,
            Func<TSource, TComparable> selector, Predicate<int> lessOrMoreThanZero) where TComparable : IComparable {

            var enumerator = sequence.GetEnumerator();
            enumerator.MoveNext();

            var minOrMaxSource = enumerator.Current;
            var minOrMaxComparable = selector(minOrMaxSource);

            for (int i = 1; enumerator.MoveNext(); ++i) {
                var currentSource = enumerator.Current;
                var currentComparable = selector(currentSource);
                if (lessOrMoreThanZero(currentComparable.CompareTo(minOrMaxComparable))) {
                    minOrMaxComparable = currentComparable;
                    minOrMaxSource = currentSource;
                }
            }
            return minOrMaxSource;
        }
    }    
}