using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Infrastructure.Interfaces;
    using Segments;

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

        public IEnumerable<ISegment> ActualFrame
            => _frames[_actualFrameIndex].ActualSegments;

        public IEnumerable<ISegment> AdditionalFrame
            => _frames[FixFrameIndex(_actualFrameIndex + _additionalFrameIndexOffset)].ActualSegments;

        public IEnumerable<ISegment> Map => new List<Segment>(_map);


        public void HandleNextData(IEnumerable<Point> nextDepthLine) {

            var prev = _frames.LastOrDefault();
            var next = new Frame(nextDepthLine);

            SetFramesAbsolute(prev, next);
            _frames.Add(next);
            _map.AddSegments(next.ActualSegmentsNearestOnly);

            _actualFrameIndex = _frames.Count - 1;
            OnMapperUpdate?.Invoke();
        }

        private void SetFramesAbsolute(Frame prev, Frame next) {

            if (prev == null) {
                return;
            }
            prev.SetNext(next);
            next.SetPrev(prev);

            var nextAverageRelative = (-(NavigationInfo)prev.RelativeByNext + (NavigationInfo)next.RelativeByPrev) / 2.0;

            next.Absolute = (NavigationInfo)prev.Absolute + nextAverageRelative;
        }

        private int FixFrameIndex(int index) => index < 0 ? 0 : index >= _frames.Count ? _frames.Count - 1 : index;
    }
}