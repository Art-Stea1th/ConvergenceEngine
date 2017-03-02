using NUnit.Framework;
using System.Collections.Generic;
using System.Windows;


namespace ConvergenceEngine.Models.UnitTests.Mapping.Extensions.Ops {

    using Models.Mapping;
    using Models.Mapping.Collections;

    [TestFixture]
    public sealed partial class FrameTests {

        [Test]
        public void SegmentSequence_SegmentateInCtor_EqualExpected() {

            // Arrange
            SegmentSequence segmentSequence = new SegmentSequenceDerived(FakePointSequence); // Act

            var fsqe = FakeSegmentSequence.GetEnumerator();
            var sqe = segmentSequence.GetEnumerator();

            // Assert
            while (fsqe.MoveNext() && sqe.MoveNext()) {
                Segment expected = fsqe.Current;
                Segment actual = sqe.Current;

                Assert.AreEqual(expected.PointA, actual.PointA);
                Assert.AreEqual(expected.PointB, actual.PointB);
            }
        }

        private sealed class SegmentSequenceDerived : SegmentSequence {
            public SegmentSequenceDerived(IEnumerable<Point> points) : base(points) { }
        }

        private IEnumerable<Point> FakePointSequence { // octagon Points
            get {
                yield return new Point(-100.0, -300.0);
                yield return new Point(-300.0, -100.0);
                yield return new Point(-300.0, 100.0);
                yield return new Point(-100.0, 300.0);
                yield return new Point(100.0, 300.0);
                yield return new Point(300.0, 100.0);
                yield return new Point(300.0, -100.0);
                yield return new Point(100.0, -300.0);
                yield return new Point(-100.0, -300.0);
            }
        }

        private IEnumerable<Segment> FakeSegmentSequence { // octagon Segments
            get {
                yield return new Segment(new List<Point> { new Point(-100.0, -300.0), new Point(-300.0, -100.0) });
                yield return new Segment(new List<Point> { new Point(-300.0, -100.0), new Point(-300.0, 100.0) });
                yield return new Segment(new List<Point> { new Point(-300.0, 100.0), new Point(-100.0, 300.0) });
                yield return new Segment(new List<Point> { new Point(-100.0, 300.0), new Point(100.0, 300.0) });
                yield return new Segment(new List<Point> { new Point(100.0, 300.0), new Point(300.0, 100.0) });
                yield return new Segment(new List<Point> { new Point(300.0, 100.0), new Point(300.0, -100.0) });
                yield return new Segment(new List<Point> { new Point(300.0, -100.0), new Point(100.0, -300.0) });
                yield return new Segment(new List<Point> { new Point(100.0, -300.0), new Point(-100.0, -300.0) });
            }
        }
    }
}