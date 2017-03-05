using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions;
    using Extensions.Iterable;
    using Extensions.Ops;
    using Segments;

    public class MapBase {

        private const double SegmenterAllowedDivergencePercent = 3.0;

        private const double SelectorMaxDistancePercent = 5.0;
        private const double SelectorMaxAngleDegrees = 3.0;

        // All Map Points
        // All Map Segments

        // Current Segments points (Frame)
        // Current Segments (Frame)

        // Current Tracked Segments
        // Points of Current Tracked Segments

        // Camera Path

        private List<MultiPointsSegment> prevSegments;
        protected List<MapSegment> MapSegments { get; private set; }


        public List<NavigationInfo> CameraPath { get; private set; }


        internal MapBase() {
            ClearData();
        }

        protected void ClearData() {
            prevSegments = new List<MultiPointsSegment>();
            MapSegments = new List<MapSegment>();
            CameraPath = new List<NavigationInfo>();
        }

        protected void NextFrameDataProceed(IEnumerable<Point> points) {

            // A.

            var nextSegments = new List<MultiPointsSegment>(points.Segmentate(SegmenterAllowedDivergencePercent));
            if (prevSegments.IsEmpty()) {
                prevSegments = nextSegments;
                return;
            }



            // B.
            
            // C.

            // D.
        }

        
    }
}