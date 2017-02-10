using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SLAM.Models.Mapping {

    using IO.DataExtractors;
    using IO.Readers;
    using FrameSequence = SortedList<int, Frame>;

    public sealed class Map {

        private DataProvider dataProvider;
        private byte[] currentFrameBuffer;

        private LinearFrameExtractor linearExtractor;
        private ColoredFrameExtractor coloredExtractor;

        private FrameSequence frameSequence;
        private Frame currentFrame;

        public event Action OnFrameUpdate;

        public byte[] Buffer { get { return coloredExtractor.ExtractColored(Color.FromArgb(255, 0, 128, 192), Color.FromArgb(255, 0, 0, 30)); } }
        public IEnumerable<Point> MapPoints { get { return frameSequence?.SelectMany(f => f.Value.Points); } }
        public IEnumerable<Point> FramePoints { get { return currentFrame?.Points; } }
        public IEnumerable<IEnumerable<Point>> FrameSegments { get { return currentFrame?.GetFrameSegments(); } }

        internal Map(DataProvider dataProvider) {
            this.dataProvider = dataProvider;
            Initialize();
        }

        private void Initialize() {
            linearExtractor = new LinearFrameExtractor(dataProvider);
            coloredExtractor = new ColoredFrameExtractor(dataProvider);
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
            OnFrameUpdate?.Invoke();
        }
    }
}