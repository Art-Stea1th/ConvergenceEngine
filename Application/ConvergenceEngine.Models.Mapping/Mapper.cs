using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Infrastructure.Interfaces;
    using System.Runtime.CompilerServices;

    public sealed class Mapper : IMapper {

        public event Action OnMapperUpdate;

        private List<Frame> frames = new List<Frame>();

        private int actualFrameIndex = 0;
        private int additionalFrameIndexOffset = -1;

        private Map map = new Map();

        public int ActualFrameIndex {
            get { return actualFrameIndex; }
            set { actualFrameIndex = FixFrameIndex(value); }
        }
        public int AdditionalFrameIndexOffset { get; set; }

        public IFrame ActualFrame {
            get { return frames[actualFrameIndex]; }
        }
        public IFrame AdditionalFrame {
            get { return frames[FixFrameIndex(actualFrameIndex + additionalFrameIndexOffset)]; }
        }

        public IEnumerable<ISegment> Map { get { return map; } }


        public void HandleNextData(IEnumerable<Point> nextDepthLine) {

            var next = new Frame(nextDepthLine);

            next.SetOffsetBy(frames.LastOrDefault());
            frames.Add(next);

            actualFrameIndex = frames.Count - 1;
            OnMapperUpdate?.Invoke();
        }

        private void Recalculate() {
            if (frames.Count < 2) {
                return;
            }
            for (int i = 1; i < frames.Count; ++i) {
                frames[i].SetOffsetBy(frames[i - 1]);
            }
        }

        private void EmplaceFrame(IEnumerable<Point> points) {
            frames.Add(new Frame(points));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FixFrameIndex(int index) {
            return index < 0 ? 0 : index >= frames.Count ? frames.Count - 1 : index;
        }
    }    
}