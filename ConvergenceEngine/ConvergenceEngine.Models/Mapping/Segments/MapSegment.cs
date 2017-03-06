using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Extensions.Ops;

    public sealed class MapSegment : MultiPointSegment {

        private List<MultiPointSegment> nearestSegments;
        private Segment approximated;

        public readonly int Id;

        public override Point PointA { get { return approximated.PointA; } }
        public override Point PointB { get { return approximated.PointB; } }

        internal MapSegment(int id, MultiPointSegment segment) : base(segment) {
            Id = id;
            nearestSegments = new List<MultiPointSegment> { segment };
            approximated = new Segment(segment.ApproximateSorted());
            UpdatePoints();
        }

        public override void ApplyTransform(double offsetX, double offsetY, double angle, bool rotatePrepend = true) {
            foreach (var segment in nearestSegments) {
                segment.ApplyTransform(offsetX, offsetY, angle, rotatePrepend);
            }
            approximated.ApplyTransform(offsetX, offsetY, angle, rotatePrepend);
        }


        //  При добавлении очередного сегмента в класс MapSegment необходимо сравнить его длину с аппроксимированным,
        //  и если она больше - пересчитать позицию существующих по новым, иначе - скорректировать новый по аппроксимированному.
        //  
        //  Для оптимизации - периодически "мержить" существующие в один.
        internal void Append(MultiPointSegment segment) {

            nearestSegments.Add(segment);
            RecalculateApproximated();

            // update points collection / async?
            UpdatePoints();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RecalculateApproximated() {
            approximated = new Segment(nearestSegments.SelectMany(p => p)
                .OrderByLine(approximated.PointA, approximated.PointB).ApproximateSorted());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePoints() {
            points = new List<Point>(nearestSegments.SelectMany(p => p).OrderByLine(PointA, PointB).ThinOutSorted());
        }
    }
}