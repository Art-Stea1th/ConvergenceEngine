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

        private const double AllowedDivergencePercent = 3.0; // using Segmenter
        private const double MaxDistancePercent = 5.0;       // using Selector & Determinator
        private const double MaxAngleDegrees = 3.0;          // using Selector & Determinator

        // All Map Points
        // All Map Segments

        // Current Segments points (Frame)
        // Current Segments (Frame)

        // Current Tracked Segments
        // Points of Current Tracked Segments

        // Camera Path

        private IEnumerable<ISegment> previous;
        private IEnumerable<ISegment> segments;

        protected List<MapSegment> MapSegments { get; private set; }


        public List<NavigationInfo> CameraPath { get; private set; }


        internal MapBase() {
            ClearData();
        }

        protected void ClearData() {
            MapSegments = new List<MapSegment>();
            CameraPath = new List<NavigationInfo>();
        }

        //      --- Step A ---
        //  
        //  Применить последнее NavigationInfo к новым сегментам.
        //  
        //  Найти среди новых сегментов близкие к предыдущим сегментам.
        //  Рассчитать новое NavigationInfo по предыдущим сегментам.
        //  
        //  Применить новое NavigationInfo к новым сегментам и к близким к предыдущим.
        //  
        //      --- Step B ---
        //  
        //  Среди сегментов, близких к предыдущим, найти сегменты близкие к сегментам карты
        //  и присвоить им Id из соответствующих сегментов карты.
        //  Уточнить новое NavigationInfo по сегментам карты и перезаписать его.
        //  
        //  Заново применить новое NavigationInfo к новым сегментам и к близким к предыдущим.
        //  
        //  Добавить все близкие к предыдущим в текущую коллекцию сегментов карты, присвоив новые Id сегментам без Id.
        //  
        //      --- Final Step ---
        //  
        //  Заменить предыдущие на новые.

        protected void NextFrameDataProceed(IEnumerable<Point> points) {

            return;

            var nextSegments = points.Segmentate(AllowedDivergencePercent);
            
            if (CameraPath.IsEmpty()) {
                InitializeWithFirstData(nextSegments);
                return;
            }

            // --- Step A ---

            NavigationInfo prevNavInfo = CameraPath.Last();
            ApplyTransformToSegments(nextSegments, prevNavInfo);

            var nearestNextAndPrev = nextSegments.SelectNearestTo(previous, MaxDistancePercent, MaxAngleDegrees);
            NavigationInfo convergence = nearestNextAndPrev.ComputeConvergence(MaxDistancePercent, MaxAngleDegrees);

            ApplyTransformToSegments(nextSegments, convergence);

            // --- Step B ---

            var nearestNextToPrev = nearestNextAndPrev.Select(sp => sp.Item1);
            ApplyTransformToSegments(nearestNextToPrev, convergence);



            NavigationInfo nextNavInfo = prevNavInfo + convergence;
        }

        private void InitializeWithFirstData(IEnumerable<MultiPointSegment> segments) {
            previous = segments;
            CameraPath.Add(new NavigationInfo(0.0, 0.0, 0.0));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyTransformToSegments(IEnumerable<ISegment> segments, NavigationInfo navigationInfo) {
            foreach (var segment in segments) {
                segment.ApplyTransform(navigationInfo.Offset.X, navigationInfo.Offset.Y, navigationInfo.Angle);
            }
        }
    }
}