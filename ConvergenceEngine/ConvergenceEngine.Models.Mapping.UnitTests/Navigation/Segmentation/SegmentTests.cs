using NUnit.Framework;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.UnitTests.Navigation.Segmentation {

    using Common;
    using Mapping.Navigation.Segmentation;

    [TestFixture]
    public sealed class SegmentTests {

        [Test]
        public void AngleBetween_EqualExpected() {

            // Arrange
            Segment segmentA = new Segment(new List<Point> { new Point(0.0, 0.0), new Point(222.2, 555.5) });
            Segment segmentB = new Segment(new List<Point> { new Point(0.0, 0.0), new Point(555.5, -222.2) });

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
        public void ConvergenceToNearestPoint_EqualExpected() {

            // Arrange
            Segment segmentA = new Segment(new List<Point> { new Point(-222.2, -222.2), new Point(111.1, 555.5) });
            Segment segmentB = new Segment(new List<Point> { new Point(-111.1, -444.4), new Point(333.3, 111.1) });

            Vector expectedConvergenceAToB = new Vector(111.1, -222.2);
            Vector expectedConvergenceBToA = new Vector(-111.1, 222.2);

            // Act
            Vector actualConvergenceAToB = segmentA.ConvergenceToNearestPoint(segmentB);
            Vector actualConvergenceBToA = segmentB.ConvergenceToNearestPoint(segmentA);

            // Assert
            Assert.AreEqual(expectedConvergenceAToB, actualConvergenceAToB);
            Assert.AreEqual(expectedConvergenceBToA, actualConvergenceBToA);
        }

        [Test]
        public void DistanceToNearestPoint_EqualExpected() {

            // Arrange
            Segment segmentA = new Segment(new List<Point> { new Point(-222.2, -222.2), new Point(111.1, 555.5) });
            Segment segmentB = new Segment(new List<Point> { new Point(-222.2, -444.4), new Point(333.3, 111.1) });

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
        public void IntersectionPointWith_EqualExpected() {

            Segment segmentA = new Segment(new List<Point> { new Point(-3, 4), new Point(-2, 8) });
            Segment segmentB = new Segment(new List<Point> { new Point(1, -7), new Point(3, -8) });

            Point expectedPoint = new Point(-5, -4);
            Point actualPoint = segmentA.IntersectionPointWith(segmentB).Value;

            Assert.AreEqual(expectedPoint, actualPoint);
        }
    }
}