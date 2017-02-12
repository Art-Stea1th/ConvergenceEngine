using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SLAM.Models.Mapping {

    internal class FrameSequence : IEnumerable<KeyValuePair<int, Frame>> {

        private SortedList<int, Frame> sequence;

        internal bool IsNullOrEmpty() {
            return sequence.IsNullOrEmpty();
        }

        internal FrameSequence(int capacity) {
            sequence = new SortedList<int, Frame>(capacity);
        }

        internal bool ContainsFrameIndex(int index) {
            return sequence.ContainsKey(index);
        }

        internal void AddFrame(int index, Frame frame) {
            sequence.Add(index, frame);
        }

        public void SetFramePositionAbsolute(int index, Frame frame) {

            var prev = sequence.LastOrDefault(f => f.Key < index).Value;
            var next = sequence.FirstOrDefault(f => f.Key > index).Value;

            if (prev != null) {
                frame.Location = prev.Location;
                frame.Direction = prev.Direction;

                frame.Segments.Difference(prev.Segments);

                //Console.WriteLine(Segment.AngleBetween(prev.Segments.First(), frame.Segments.First()));

                //var prevS = 
            }

            //Console.WriteLine(prev);
            //Console.WriteLine(next);
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