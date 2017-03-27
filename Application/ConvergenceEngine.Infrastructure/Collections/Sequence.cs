using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvergenceEngine.Infrastructure.Collections {

    public sealed class Sequence<T> {

        private List<SequenceNode<T>> _values;

        //private SequenceNode<T> _head;
        //private SequenceNode<T> _last;


        public Sequence() {
            _values = new List<SequenceNode<T>>();
        }

        public SequenceNode<T> AddLast(T item) {
            var result = new SequenceNode<T>(this, item);
            AddLast(result);
            return result;
        }

        public void AddLast(SequenceNode<T> node) {

            if (_values.Count < 1) {
                _values.Add(node);
            }

        }


    }
}
