using System.Collections.Generic;

namespace ConvergenceEngine.ViewModels {

    using Infrastructure;
    using Infrastructure.Interfaces;

    public abstract class ApplicationViewModel : ViewModelBase {

        private IDataProvider dataProvider;
        private IMapper mapper;
        private IMap map;
        private short[,] fullFrame;

        private string startStopResetButtonText;

        protected IDataProvider DataProvider {
            get { return dataProvider; }
            set { SetNew(value); }
        }

        protected IMapper Mapper {
            get { return mapper; }
            set { SetNew(value); }
        }

        public IMap Map {
            get { return map; }
            set { Set(ref map, value); }
        }

        public short[,] FullFrame {
            get { return fullFrame; }
            set { Set(ref fullFrame, value); }
        }

        public string StartStopResetButtonText {
            get { return startStopResetButtonText; }
            set { Set(ref startStopResetButtonText, value); }
        }

        protected ApplicationViewModel() {
            UpdateStartStopResetButtonText(null);
        }

        private void SetNew(IDataProvider dataProvider) {
            if (dataProvider != null) {
                if (mapper != null) {
                    dataProvider.OnNextDepthLineReady += mapper.HandleNextData;
                }
                dataProvider.OnNextFullFrameReady += UpdateFullFrame;
                dataProvider.OnStateChanged += OnDataProviderStateChanged;
            }
            this.dataProvider = dataProvider;
        }

        private void SetNew(IMapper mapper) {
            if (dataProvider != null) {
                if (this.mapper != null) {
                    dataProvider.OnNextDepthLineReady -= this.mapper.HandleNextData;
                }
                if (mapper != null) { // 1
                    dataProvider.OnNextDepthLineReady += mapper.HandleNextData;
                }
            }
            if (mapper != null) { // 2
                mapper.OnMapUpdate += UpdateMap;
            }
            this.mapper = mapper;
        }

        protected virtual void UpdateMap(IMap map) {
            Map = map;
        }

        protected virtual void UpdateFullFrame(short[,] fullFrame) {
            FullFrame = fullFrame;
        }

        protected virtual void OnDataProviderStateChanged(DataProviderStates state) {
            UpdateStartStopResetButtonText(state);
        }

        protected void UpdateStartStopResetButtonText(DataProviderStates? state) {
            if (Mapper == null) {
                StartStopResetButtonText = "START";
                return;
            }
            if (state == DataProviderStates.Started) {
                StartStopResetButtonText = "STOP";
                return;
            }
            StartStopResetButtonText = "RESET";
        }
    }
}