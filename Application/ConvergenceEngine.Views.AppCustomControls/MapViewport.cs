using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConvergenceEngine.Views.AppCustomControls {

    using Infrastructure.Interfaces;

    public class MapViewport : Control {

        public IEnumerable<ISegment> AllSegments {
            get { return (IEnumerable<ISegment>)GetValue(AllSegmentsProperty); }
            set { SetValue(AllSegmentsProperty, value); }
        }
        public IEnumerable<ISegment> CurrentSegments {
            get { return (IEnumerable<ISegment>)GetValue(CurrentSegmentsProperty); }
            set { SetValue(CurrentSegmentsProperty, value); }
        }
        public IEnumerable<INavigationInfo> CameraPath {
            get { return (IEnumerable<INavigationInfo>)GetValue(CameraPathProperty); }
            set { SetValue(CameraPathProperty, value); }
        }

        public static readonly DependencyProperty AllSegmentsProperty;
        public static readonly DependencyProperty CurrentSegmentsProperty;
        public static readonly DependencyProperty CameraPathProperty;

        static MapViewport() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MapViewport), new FrameworkPropertyMetadata(typeof(MapViewport)));
        }
    }
}
