using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.ViewModels.AppWindows {

    using Models.Mapping;
    using System.Windows.Media;

    internal sealed class MotionDataWindowViewModel : ViewModelBase {

        private Map map;

        private PointCollection absolutePositionX;
        private PointCollection absolutePositionY;
        private PointCollection absoluteAngle;

        private PointCollection relativePositionX;
        private PointCollection relativePositionY;
        private PointCollection relativeAngle;

        public PointCollection AbsolutePositionX {
            get { return absolutePositionX; }
            set { Set(ref absolutePositionX, value); }
        }
        public PointCollection AbsolutePositionY {
            get { return absolutePositionY; }
            set { Set(ref absolutePositionY, value); }
        }
        public PointCollection AbsoluteAngle {
            get { return absoluteAngle; }
            set { Set(ref absoluteAngle, value); }
        }

        public PointCollection RelativePositionX {
            get { return relativePositionX; }
            set { Set(ref relativePositionX, value); }
        }
        public PointCollection RelativePositionY {
            get { return relativePositionY; }
            set { Set(ref relativePositionY, value); }
        }
        public PointCollection RelativeAngle {
            get { return relativeAngle; }
            set { Set(ref relativeAngle, value); }
        }

        internal MotionDataWindowViewModel(Map map) {
            this.map = map;
            this.map.OnFrameUpdate += Update;
            Initialize();
        }

        private void Initialize() {
            Update();
        }

        public void Update() {

            AbsolutePositionX = new PointCollection();
            AbsolutePositionY = new PointCollection();
            AbsoluteAngle = new PointCollection();

            RelativePositionX = new PointCollection();
            RelativePositionY = new PointCollection();
            RelativeAngle = new PointCollection();

            foreach (var frame in map) {
                AbsolutePositionX.Add(new Point(frame.Key, frame.Value.Absolute.Direction.X));
                AbsolutePositionY.Add(new Point(frame.Key, frame.Value.Absolute.Direction.Y));
                AbsoluteAngle.Add(new Point(frame.Key, frame.Value.Absolute.Angle));

                //RelativePositionX.Add(new Point(frame.Key, frame.Value.RelativePosition.Direction.X));
                //RelativePositionY.Add(new Point(frame.Key, frame.Value.RelativePosition.Direction.Y));
                //RelativeAngle.Add(new Point(frame.Key, frame.Value.RelativePosition.Angle));
            }
        }
    }
}