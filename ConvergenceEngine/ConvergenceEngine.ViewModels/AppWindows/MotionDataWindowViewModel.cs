using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.ViewModels.AppWindows {

    using Models.Mapping;

    internal sealed class MotionDataWindowViewModel : ViewModelBase {

        private Map map;

        private IEnumerable<Point> absolutePositionX;
        private IEnumerable<Point> absolutePositionY;
        private IEnumerable<Point> absoluteAngle;

        private IEnumerable<Point> relativePositionX;
        private IEnumerable<Point> relativePositionY;
        private IEnumerable<Point> relativeAngle;

        public IEnumerable<Point> AbsolutePositionX {
            get { return absolutePositionX; }
            set { Set(ref absolutePositionX, value); }
        }

        public IEnumerable<Point> AbsolutePositionY {
            get { return absolutePositionY; }
            set { Set(ref absolutePositionY, value); }
        }

        public IEnumerable<Point> AbsoluteAngle {
            get { return absoluteAngle; }
            set { Set(ref absoluteAngle, value); }
        }

        internal MotionDataWindowViewModel(Map map) {
            this.map = map;
            this.map.OnFrameUpdate += Update;
            Initialize();
        }

        private void Initialize() {
            absolutePositionX = null;
        }

        public void Update() {

            var absolute = map.Select(f => new Tuple<Point, Point, Point>(
                new Point(f.Key, f.Value.AbsolutePosition.Direction.X),
                new Point(f.Key, f.Value.AbsolutePosition.Direction.Y),
                new Point(f.Key, f.Value.AbsolutePosition.Angle)));

            var relative = map.Select(f => new Tuple<Point, Point, Point>(
                new Point(f.Key, f.Value.RelativePosition.Direction.X),
                new Point(f.Key, f.Value.RelativePosition.Direction.Y),
                new Point(f.Key, f.Value.RelativePosition.Angle)));

            AbsolutePositionX = absolute.Select(ni => ni.Item1);
            AbsolutePositionY = absolute.Select(ni => ni.Item2);
            AbsoluteAngle = absolute.Select(ni => ni.Item3);
        }
    }
}