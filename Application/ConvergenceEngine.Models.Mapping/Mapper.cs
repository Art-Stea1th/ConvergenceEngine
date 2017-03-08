using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions;
    using Infrastructure.Extensions;
    using Infrastructure.Interfaces;
    using Segments;

    public sealed class Mapper : IMapper {

        public event Action<IMap> OnMapUpdate;

        public void HandleNextData(IEnumerable<Point> nextDepthLine) {
            //throw new NotImplementedException();
        }

        private const double AllowedDivergencePercent = 3.0; // using Segmenter
        private const double MaxDistancePercent = 5.0;       // using Selector & Determinator
        private const double MaxAngleDegrees = 3.0;          // using Selector & Determinator

        public Point Size { get; private set; }

        public IEnumerable<ISegment> PreviousSegments { get; private set; }
        public IEnumerable<ISegment> CurrentSegments { get; private set; }
        public List<MapSegment> Segments { get; private set; }
        public List<NavigationInfo> CameraPath { get; private set; }


        public Mapper() {
            ClearData();
        }

        internal void ClearData() {
            Segments = new List<MapSegment>();
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

        internal void NextFrameDataProceed(IEnumerable<Point> points) {


            CurrentSegments = points.Segmentate(AllowedDivergencePercent);

            if (CameraPath.IsEmpty()) {
                InitializeWithFirstData(CurrentSegments);
                return;
            }

            // --- Step A ---

            NavigationInfo prevNavInfo = CameraPath.Last();
            ApplyTransformToSegments(CurrentSegments, prevNavInfo);

            var nearestNextAndPrev = CurrentSegments.SelectNearestTo(PreviousSegments, MaxDistancePercent, MaxAngleDegrees);
            NavigationInfo convergence = nearestNextAndPrev.ComputeConvergence(MaxDistancePercent, MaxAngleDegrees);

            ApplyTransformToSegments(CurrentSegments, convergence);

            // --- Step B ---

            var nearestNextToPrev = nearestNextAndPrev.Select(sp => sp.Item1);
            var nearestNextToPrevToMap = CurrentSegments.SelectNearestTo(Segments, MaxDistancePercent, MaxAngleDegrees);
            NavigationInfo convergenceNew = nearestNextToPrevToMap.ComputeConvergence(MaxDistancePercent, MaxAngleDegrees);


            //ApplyTransformToSegments(nearestNextToPrev, convergence);


            // --- Final Step ---

            PreviousSegments = CurrentSegments;
            NavigationInfo nextNavInfo = prevNavInfo + convergence;
            CameraPath.Add(nextNavInfo);
            UpdateMapSize();
        }

        private void InitializeWithFirstData(IEnumerable<ISegment> segments) {
            PreviousSegments = segments;
            CameraPath.Add(new NavigationInfo(0.0, 0.0, 0.0));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyTransformToSegments(IEnumerable<ISegment> segments, NavigationInfo navigationInfo) {
            foreach (var segment in segments) {
                segment.ApplyTransform(navigationInfo.X, navigationInfo.Y, navigationInfo.A);
            }
        }

        private void UpdateMapSize() {
            if (Segments.IsNullOrEmpty()) {
                if (CurrentSegments.IsNullOrEmpty()) {
                    Size = new Point(0.0, 0.0);
                    return;
                }
                Size = GetMapSizeBy(CurrentSegments);
                return;
            }
            Size = GetMapSizeBy(Segments);
        }

        private Point GetMapSizeBy(IEnumerable<ISegment> segments) {
            double minX = double.PositiveInfinity, minY = minX;
            double maxX = double.NegativeInfinity, maxY = maxX;
            foreach (var segment in CurrentSegments) {
                foreach (var point in segment) {
                    if (point.X < minX) { minX = point.X; }
                    else
                    if (point.X > maxX) { maxX = point.X; }

                    if (point.Y < minY) { minY = point.Y; }
                    else
                    if (point.Y > maxY) { maxY = point.Y; }
                }
            }
            return new Point(maxX - minX, maxY - minY);
        }
    }
}