using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SLAM.Models.Mapping {

    using Extensions;
    using IO.DataExtractors;

    internal class FrameSequence : IEnumerable<KeyValuePair<int, Frame>> {

        private MiddleLineFrameExtractor extractor;

        private SortedList<int, Frame> sequence;

        internal IEnumerable<Point> GetMapPoints() { // ??

            Point prevPoint = new Point(0, 0);

            foreach (var item in sequence) {
                foreach (var point in item.Value.PointsTransformed) {
                    if (point.DistanceTo(prevPoint) >= 3) {
                        yield return point;
                        prevPoint = point;
                    }
                }
            }
        }


        internal FrameSequence(int capacity, MiddleLineFrameExtractor extractor) {
            sequence = new SortedList<int, Frame>(capacity);
            this.extractor = extractor;
        }

        internal Frame AppendData(int index, byte[] data) {

            if (sequence.ContainsKey(index)) {
                return sequence[index];
            }

            var middleLine = extractor.ExtractMiddleLine(data);
            Frame frame = new Frame(middleLine);

            var prev = sequence.LastOrDefault(f => f.Key < index);
            var next = sequence.FirstOrDefault(f => f.Key > index);

            if (prev.Value == null) {
                frame.SetPosition(new Navigation.NavigationInfo());
            }
            else {
                var difference = frame.GetDifferenceTo(prev.Value);
                frame.SetPosition(prev.Value.Absolute + difference);
            }

            if (next.Value == null) {
                sequence.Add(index, frame);
                return frame;
            }
            else {
                var newNextPosition = next.Value.GetDifferenceTo(frame);
                var offset = newNextPosition - next.Value.Absolute;

                foreach (var item in sequence) {
                    if (item.Key > index) {
                        item.Value.SetPosition(item.Value.Absolute + offset);
                    }
                }
            }
            sequence.Add(index, frame);
            return frame;
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