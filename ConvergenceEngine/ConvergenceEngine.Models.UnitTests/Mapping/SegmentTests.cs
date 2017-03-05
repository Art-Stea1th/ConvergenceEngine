using NUnit.Framework;
using System.Windows;

namespace ConvergenceEngine.Models.UnitTests.Mapping {

    using Common;
    using Models.Mapping.Segments;

    [TestFixture]
    public sealed class SegmentTests {

        [Test]
        public void AngleBetween_EqualExpected() {

            // Arrange
            ISegment segmentA = new Segment(new Point(0.0, 0.0), new Point(222.2, 555.5));
            ISegment segmentB = new Segment(new Point(0.0, 0.0), new Point(555.5, -222.2));

            double expectedAngleAToB = -90.0;
            double expectedAngleBToA = 90.0;

            // Act
            double actualAngleAToB = Segment.AngleBetween(segmentA, segmentB);
            double actualAngleBToA = Segment.AngleBetween(segmentB, segmentA);

            // Assert
            Assert.AreEqual(expectedAngleAToB, actualAngleAToB, Constants.PrecisionAcceptable);
            Assert.AreEqual(expectedAngleBToA, actualAngleBToA, Constants.PrecisionAcceptable);
        }

        [Test]
        public void DistanceToNearestPoint_EqualExpected() {

            // Arrange
            ISegment segmentA = new Segment(new Point(-222.2, -222.2), new Point(111.1, 555.5));
            ISegment segmentB = new Segment(new Point(-222.2, -444.4), new Point(333.3, 111.1));

            double expectedDistanceAToB = 222.2;
            double expectedDistanceBToA = 222.2;

            // Act
            double actualDistanceAToB = segmentA.DistanceToNearestPoint(segmentB);
            double actualDistanceBToA = segmentB.DistanceToNearestPoint(segmentA);

            // Assert
            Assert.AreEqual(expectedDistanceAToB, actualDistanceAToB);
            Assert.AreEqual(expectedDistanceBToA, actualDistanceBToA);
        }

        [Test]
        public void IntersectWith_EqualExpected() {

            Segment segmentA = new Segment(new Point(-3, 4), new Point(-2, 8));
            Segment segmentB = new Segment(new Point(1, -7), new Point(3, -8));

            Point expectedPoint = new Point(-5, -4);
            Point actualPoint = segmentA.IntersectWith(segmentB).Value;

            Assert.AreEqual(expectedPoint, actualPoint);
        }
    }
}