using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;


namespace ConvergenceEngine.Models.Mapping.Extensions.Iterable {

    internal static partial class Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence) {
            return sequence.IsNull() || sequence.IsEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this IEnumerable<T> sequence) {
            return sequence.Count() < 1;
        }

        public static IEnumerable<TResult> Sequential<TSource, TAnother, TResult>(
            this IEnumerable<TSource> sourceSequence, IEnumerable<TAnother> anotherSequence, Func<TSource, TAnother, TResult> selector) {

            var ssEnumerator = sourceSequence.GetEnumerator();
            var asEnumerator = anotherSequence.GetEnumerator();

            while (ssEnumerator.MoveNext() && asEnumerator.MoveNext()) {
                yield return selector.Invoke(ssEnumerator.Current, asEnumerator.Current);
            }
        }
    }    
}