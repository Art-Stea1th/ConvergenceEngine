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

        private List<Frame> _frames = new List<Frame>();

        private int _actualFrameIndex = 0;
        private int _additionalFrameIndexOffset = -1;

        private Map _map = new Map();

        public int ActualFrameIndex {
            get => _actualFrameIndex;
            set => _actualFrameIndex = FixFrameIndex(value);
        }
        public int AdditionalFrameIndexOffset { get; set; }

        public IEnumerable<ISegment> ActualFrame => _frames[_actualFrameIndex];
        public IEnumerable<ISegment> AdditionalFrame => _frames[FixFrameIndex(_actualFrameIndex + _additionalFrameIndexOffset)];

        public IEnumerable<ISegment> Map => _map;


        public void HandleNextData(IEnumerable<Point> nextDepthLine) {

            var next = new Frame(nextDepthLine);

            next.SetPrev(_frames.LastOrDefault());
            _frames.Add(next);

            _actualFrameIndex = _frames.Count - 1;
            OnMapperUpdate?.Invoke();
        }

        private void EmplaceFrame(IEnumerable<Point> points) {
            _frames.Add(new Frame(points));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FixFrameIndex(int index) => index < 0 ? 0 : index >= _frames.Count ? _frames.Count - 1 : index;
    }
}