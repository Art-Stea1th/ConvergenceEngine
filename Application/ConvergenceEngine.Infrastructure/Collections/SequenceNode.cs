using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvergenceEngine.Infrastructure.Collections {

    public sealed class SequenceNode<T> {

        public Sequence<T> Sequence { get; internal set; }
        public SequenceNode<T> Next { get; internal set; }
        public SequenceNode<T> Prev { get; internal set; }

        public T Value { get; set; }

        public SequenceNode(T value) {
            Value = value;
        }

        internal SequenceNode(Sequence<T> sequence, T value) {
            Sequence = sequence;
            Value = value;
        }

        internal void Invalidate() {
            Sequence = null;
            Next = null;
            Prev = null;
        }
    }
}