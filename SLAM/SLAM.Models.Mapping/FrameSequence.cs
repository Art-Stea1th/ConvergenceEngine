using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SLAM.Models.Mapping {

    using Extensions;

    internal class FrameSequence : IEnumerable<KeyValuePair<int, Frame>> {

        private SortedList<int, Frame> sequence;

        internal FrameSequence(int capacity) {
            sequence = new SortedList<int, Frame>(capacity);
        }

        internal bool ContainsFrameIndex(int index) {
            return sequence.ContainsKey(index);
        }

        internal void AddFrame(int index, Frame frame) {
            sequence.Add(index, frame);
        }     

        #region IEnumerable

        public IEnumerator<KeyValuePair<int, Frame>> GetEnumerator() {
            return sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion
    }
}