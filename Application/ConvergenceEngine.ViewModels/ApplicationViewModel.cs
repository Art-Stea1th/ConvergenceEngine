using System.Collections.Generic;

namespace ConvergenceEngine.ViewModels {

    using Infrastructure;
    using Infrastructure.Interfaces;

    public abstract class ApplicationViewModel : ViewModelBase {

        private IDataProvider _dataProvider;
        private short[,] _fullFrame;

        private IMapper _mapper;
        private IEnumerable<ISegment> _mapSegments;
        private IEnumerable<ISegment> _actualSegments;

        public IDataProvider DataProvider {
            get => _dataProvider;
            protected set { SetNew(value); NotifyPropertyChanged(); }
        }
        public short[,] FullFrame {
            get => _fullFrame;
            protected set => Set(ref _fullFrame, value);
        }

        public IMapper Mapper {
            get => _mapper;
            protected set { SetNew(value); NotifyPropertyChanged(); }
        }
        public IEnumerable<ISegment> MapSegments {
            get => _mapSegments;
            internal set => Set(ref _mapSegments, value);
        }
        public IEnumerable<ISegment> ActualSegments {
            get => _actualSegments;
            internal set => Set(ref _actualSegments, value);
        }        

        public abstract double FpsCurrent { get; set; }
        public abstract bool ModelStarted { get; set; }
        public abstract int TotalFrames { get; set; }
        public abstract int CurrentFrame { get; set; }
        public abstract string StartStopResetButtonText { get; set; }

        private void SetNew(IDataProvider dataProvider) {
            if (dataProvider != null) {
                if (_mapper != null) {
                    dataProvider.OnNextDepthLineReady += _mapper.HandleNextData;
                }
                dataProvider.OnNextFullFrameReady += UpdateFullFrame;
                dataProvider.OnNextFullFrameReady += (f) => ++TotalFrames;
                dataProvider.OnStateChanged += OnDataProviderStateChanged;
            }
            _dataProvider = dataProvider;
        }

        private void SetNew(IMapper mapper) {
            if (_dataProvider != null) {
                if (_mapper != null) {
                    _dataProvider.OnNextDepthLineReady -= _mapper.HandleNextData;
                }
                if (mapper != null) { // 1
                    _dataProvider.OnNextDepthLineReady += mapper.HandleNextData;
                }
            }
            if (mapper != null) { // 2
                mapper.OnMapperUpdate += UpdateMapperData;
            }
            _mapper = mapper;
        }

        protected virtual void UpdateMapperData() {
            MapSegments = Mapper.Map;
            ActualSegments = Mapper.ActualFrame;
        }

        protected virtual void UpdateFullFrame(short[,] fullFrame) {
            FullFrame = fullFrame;
        }

        protected virtual void OnDataProviderStateChanged(DataProviderStates state) {
            if (state == DataProviderStates.Started) {
                ModelStarted = true;
            }
            else {
                ModelStarted = false;
                CurrentFrame = TotalFrames - 1;
            }
            UpdateStartStopResetButtonText();
        }

        protected abstract void UpdateStartStopResetButtonText();

        protected override void OnDispose() {
            DataProvider?.Dispose();
        }
    }
}