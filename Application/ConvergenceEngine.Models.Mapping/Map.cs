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

    public sealed class Map : IMap { // quick hardcode not optimized impl.

        public event Action<IMapData> OnMapUpdate;

        private const double AllowedDivergencePercent = 3.0; // using Segmenter
        private const double MaxDistancePercent = 5.0;       // using Selector & Determinator
        private const double MaxAngleDegrees = 3.0;          // using Selector & Determinator

        private int uid = 0;
        private int NextUniqueId { get { return uid++; } }

        public List<MultiPointSegment> Segments { get; private set; }
        public IEnumerable<Segment> PreviousSegments { get; private set; }
        public IEnumerable<Segment> CurrentSegments { get; private set; }
        public List<NavigationInfo> CameraPath { get; private set; }

        public Map() {
            Segments = new List<MultiPointSegment>();
            CameraPath = new List<NavigationInfo>();
        }

        public void HandleNextData(IEnumerable<Point> points) {

            CurrentSegments = points.Segmentate(AllowedDivergencePercent);

            if (CameraPath.IsEmpty()) {
                InitializeWithFirstData(CurrentSegments);
                return;
            }

            // --- Step A ---

            NavigationInfo prevNavInfo = CameraPath.Last();
            ApplyTransformToSegments(CurrentSegments, prevNavInfo);

            SetIdentifiersForNearest(CurrentSegments, PreviousSegments, MaxDistancePercent, MaxAngleDegrees);

            var nearestPairsToPrev = GetPairsWithEqualsId(CurrentSegments, PreviousSegments);
            NavigationInfo convergence = nearestPairsToPrev.ComputeConvergence(MaxDistancePercent, MaxAngleDegrees);

            ApplyTransformToSegments(CurrentSegments, convergence);

            // --- Step B ---

            var nearestCurrToPrev = nearestPairsToPrev.Select(sp => new MultiPointSegment(sp.Item1));

            SetIdentifiersForNearest(nearestCurrToPrev, Segments, MaxDistancePercent, MaxAngleDegrees);

            var nearestPairsToMap = GetPairsWithEqualsId(nearestCurrToPrev, Segments);
            NavigationInfo convergenceToMap = nearestPairsToMap.ComputeConvergence(MaxDistancePercent, MaxAngleDegrees);

            ApplyTransformToSegments(CurrentSegments, convergenceToMap);
            ApplyTransformToSegments(nearestCurrToPrev, convergenceToMap);

            Segments = GetMergedSegmentsCollection(Segments, nearestCurrToPrev);

            PreviousSegments = CurrentSegments;
            NavigationInfo nextNavInfo = prevNavInfo + convergence + convergenceToMap;
            CameraPath.Add(nextNavInfo);

            OnMapUpdate?.Invoke(new MapData(Segments, CurrentSegments, CameraPath));
        }

        private List<MultiPointSegment> GetMergedSegmentsCollection(IEnumerable<ISegment> toCurrent, IEnumerable<ISegment> fromAnother) {
            var result = new List<MultiPointSegment>();

            foreach (var c in toCurrent) {
                MultiPointSegment next = null;
                if (ExistsById(fromAnother, c)) {
                    var another = fromAnother.Where(s => s.Id == c.Id).First();
                    if (c.Length > another.Length) {
                        next = new MultiPointSegment(c);
                        next.Id = c.Id;
                        result.Add(next);
                    }
                    else {
                        next = new MultiPointSegment(another);
                        next.Id = another.Id;
                        result.Add(next);
                    }
                }
                else {
                    next = new MultiPointSegment(c);
                    next.Id = c.Id;
                    result.Add(next);
                }
                
            }
            return result;
        }

        private bool ExistsById(IEnumerable<ISegment> segments, ISegment segment) {
            foreach (var s in segments) {
                if (s.Id == segment.Id) {
                    return true;
                }
            }
            return false;
        }

        private void SetIdentifiersForNearest(IEnumerable<Segment> toCurrent, IEnumerable<ISegment> fromAnother,
            double maxDistancePercent, double maxAngleDegrees) {
            foreach (var segment in toCurrent) {
                int? nextId = fromAnother.SelectNearestTo(segment, maxDistancePercent, maxAngleDegrees)?.Id;
                if (nextId != null) {
                    segment.Id = nextId;
                }
                else {
                    segment.Id = NextUniqueId;
                }
            }
        }

        private IEnumerable<Tuple<ISegment, ISegment>> GetPairsWithEqualsId(IEnumerable<ISegment> current, IEnumerable<ISegment> another) {
            foreach (var c in current) {
                foreach (var a in another) {
                    if (c.Id == a.Id) {
                        yield return new Tuple<ISegment, ISegment>(c, a);
                    }
                }
            }
        }

        private void InitializeWithFirstData(IEnumerable<Segment> segments) {
            foreach (var segment in segments) {
                segment.Id = NextUniqueId;
            }
            PreviousSegments = segments;
            CameraPath.Add(new NavigationInfo(0.0, 0.0, 0.0));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyTransformToSegments(IEnumerable<ISegment> segments, NavigationInfo navigationInfo) {
            foreach (var segment in segments) {
                segment.ApplyTransform(navigationInfo.X, navigationInfo.Y, navigationInfo.A);
            }
        }
    }
}