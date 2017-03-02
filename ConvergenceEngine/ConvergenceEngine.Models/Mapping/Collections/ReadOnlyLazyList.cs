using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ConvergenceEngine.Models.Mapping.Collections {

    public abstract class ReadOnlyLazyList<T> : IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T> {

        private readonly Lazy<IReadOnlyList<T>> items;

        public T this[int index] { get { return items.Value[index]; } }
        public int Count { get { return items.Value.Count; } }

        internal ReadOnlyLazyList(Func<IEnumerable<T>> lazyInitializer) {
            items = new Lazy<IReadOnlyList<T>>(() => new List<T>(lazyInitializer.Invoke()));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() {
            return items.Value.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() {
            return items.Value.GetEnumerator();
        }
    }
}