using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Infrastructure.Interfaces;

    public sealed class Mapper : IMapper {

        public event Action OnMapperUpdate;

        private List<Frame> frames = new List<Frame>();

        private int actualFrameIndex = 0;
        private int additionalFrameIndexOffset = -1;

        private Map map = new Map();

        public int ActualFrameIndex {
            get => actualFrameIndex;
            set => actualFrameIndex = FixFrameIndex(value);
        }
        public int AdditionalFrameIndexOffset { get; set; }

        public IFrame ActualFrame { get => frames[actualFrameIndex]; }
        public IFrame AdditionalFrame { get => frames[FixFrameIndex(actualFrameIndex + additionalFrameIndexOffset)]; }

        public IEnumerable<ISegment> Map { get => map; }


        public void HandleNextData(IEnumerable<Point> nextDepthLine) {

            var next = new Frame(nextDepthLine);

            next.SetPrev(frames.LastOrDefault());
            frames.Add(next);

            actualFrameIndex = frames.Count - 1;
            OnMapperUpdate?.Invoke();
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