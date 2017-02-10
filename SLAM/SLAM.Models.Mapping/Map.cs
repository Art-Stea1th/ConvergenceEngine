using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SLAM.Models.Mapping {

    using IO.DataExtractors;
    using IO.Readers;
    using FrameSequence = SortedList<int, Frame>;

    public sealed class Map {

        private DataProvider dataProvider;
        private byte[] currentFrameBuffer;

        private LinearFrameExtractor linearExtractor;

        private FrameSequence frameSequence;
        private Frame currentFrame;

        public byte[] Buffer { get { return currentFrameBuffer; } }
        public IEnumerable<Point> MapPoints { get { return frameSequence.SelectMany(f => f.Value.Points); } }
        public IEnumerable<Point> FramePoints { get { return currentFrame.Points; } }
        public IEnumerable<IEnumerable<Point>> FrameSegments { get { return currentFrame.GetFrameSegments(); } }

        internal Map(DataProvider dataProvider) {
            this.dataProvider = dataProvider;
            Initialize();
        }

        private void Initialize() {
            linearExtractor = new LinearFrameExtractor(dataProvider);
            dataProvider.OnNextFrameReady += Update;
        }

        private void Update() {

            dataProvider.GetNextRawFrameTo(out currentFrameBuffer);

            if (frameSequence.IsNullOrEmpty()) {
                frameSequence = new FrameSequence(dataProvider.TotalFrames);
            }

            if (!frameSequence.ContainsKey(dataProvider.FrameIndex)) {
                currentFrame = new Frame(linearExtractor.ExtractMiddleLine(currentFrameBuffer));
                frameSequence.Add(dataProvider.FrameIndex, currentFrame);
            }
            else {
                currentFrame = frameSequence.Single(f => f.Key == dataProvider.FrameIndex).Value;
            }
        }
    }
}