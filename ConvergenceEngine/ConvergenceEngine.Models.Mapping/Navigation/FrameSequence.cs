using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Navigation {

    using Extensions;

    public abstract class FrameSequence : IEnumerable<KeyValuePair<int, Frame>> {

        private SortedList<int, Frame> frames;

        internal int CurrentFrameIndex { get; private set; }
        public Frame CurrentFrame { get; private set; }
        public Frame PreviousFrame { get; private set; }

        internal FrameSequence() {
            ReInitializeData();
        }

        protected void ReInitializeData() {
            frames = new SortedList<int, Frame>();
            CurrentFrameIndex = 0;
            CurrentFrame = null;
            PreviousFrame = null;
        }

        public IEnumerable<Point> GetMapPoints() { // TMP

            Point prevPoint = new Point(0, 0);

            foreach (var item in frames) {
                foreach (var point in item.Value.PointsTransformed) {
                    if (point.DistanceTo(prevPoint) >= 5.0) {
                        yield return point;
                        prevPoint = point;
                    }
                }
            }
        }

        internal void NextFrameProceed(int index, Frame frame) { // TMP

            CurrentFrameIndex = index;
            CurrentFrame = frame;

            var prev = frames.LastOrDefault(f => f.Key < index);
            PreviousFrame = prev.Value;

            if (frames.ContainsKey(index)) {
                return;
            }

            if (prev.Value == null) {
                frame.SetPosition(new NavigationInfo());
            }
            else {
                var convergence = frame.ConvergenceTo(prev.Value);
                frame.SetPosition(prev.Value.AbsolutePosition + convergence);
                //Console.WriteLine($"{prev.Value.Absolute} | {convergence} | {frame.Absolute}");
            }
            frames.Add(index, frame);
        }

        IEnumerator<KeyValuePair<int, Frame>> IEnumerable<KeyValuePair<int, Frame>>.GetEnumerator() {
            return frames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return frames.GetEnumerator();
        }        
    }
}