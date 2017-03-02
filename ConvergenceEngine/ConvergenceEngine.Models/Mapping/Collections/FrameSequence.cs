using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Collections {

    using Extensions;

    public abstract class FrameSequence : IEnumerable<KeyValuePair<int, Frame>> {

        private SortedList<int, Frame> frames;
        private SortedList<int, Frame> buffer;

        private int currentFrameIndex;
        private KeyValuePair<int, Frame> currentFrame;
        private KeyValuePair<int, Frame> previousFrame;

        internal int CurrentFrameIndex { get { return currentFrameIndex; } }
        public Frame CurrentFrame { get { return currentFrame.Value; } }
        public Frame PreviousFrame { get { return previousFrame.Value; } }

        internal FrameSequence() {
            ReInitializeData();
        }

        protected void ReInitializeData() {
            frames = new SortedList<int, Frame>();
            currentFrameIndex = 0;
            currentFrame = new KeyValuePair<int, Frame>(CurrentFrameIndex, null);
            previousFrame = new KeyValuePair<int, Frame>(CurrentFrameIndex - 1, null);
        }

        public IEnumerable<Point> GetMapPoints() { // TMP, slow View

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

        internal void NextFrameProceed(int index, IEnumerable<Point> points) { // TMP

            currentFrameIndex = index;

            //previousFrame = frames.LastOrDefault(f => f.Key < index);
            //if (frames.ContainsKey(index)) {
            //    currentFrame = new KeyValuePair<int, Frame>(index, frames[index]);
            //    return;
            //}

            //currentFrame = new KeyValuePair<int, Frame>(index, new Frame(index, points));

            //var convergence = currentFrame.Value.ConvergenceTo(previousFrame.Value);

            currentFrame = new KeyValuePair<int, Frame>(index, new Frame(index, points));

            var prev = frames.LastOrDefault(f => f.Key < index);
            previousFrame = prev;

            if (frames.ContainsKey(index)) {
                return;
            }

            if (prev.Value == null) {
                currentFrame.Value.SetAbsoluteNavigationInfo(new NavigationInfo());
            }
            else {
                var convergence = currentFrame.Value.ConvergenceTo(previousFrame.Value);
                currentFrame.Value.SetAbsoluteNavigationInfo(prev.Value.Absolute + convergence);
                //Console.WriteLine($"{prev.Value.Absolute} | {convergence} | {frame.Absolute}");
            }
            frames.Add(index, currentFrame.Value);
        }

        IEnumerator<KeyValuePair<int, Frame>> IEnumerable<KeyValuePair<int, Frame>>.GetEnumerator() {
            return frames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return frames.GetEnumerator();
        }
    }
}