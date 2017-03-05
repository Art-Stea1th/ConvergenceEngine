using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.UnitTests.Mapping.Extensions.Ops {

    using Models.Mapping.Segments;
    using Models.Mapping.Extensions.Ops;

    [TestFixture]
    public sealed class DeterminatorTests {

        [Test]
        public void ComputeConvergence_AfterInvoke_NotChangedSource() {

            // Arrange
            var source = BuildSegmentsPair(new Point(0.0, 0.0), new Point(0.0, 1.0), new Point(0.0, 0.0), new Point(1.0, 0.0));
            var expected = BuildSegmentsPair(new Point(0.0, 0.0), new Point(0.0, 1.0), new Point(0.0, 0.0), new Point(1.0, 0.0));

            // Act
            source.ComputeConvergence(100.0, 360.0);
            var actual = source;

            // Assert

            Assert.AreEqual(expected.First().Item1.PointA, actual.First().Item1.PointA);
            Assert.AreEqual(expected.First().Item1.PointB, actual.First().Item1.PointB);

            Assert.AreEqual(expected.First().Item2.PointA, actual.First().Item2.PointA);
            Assert.AreEqual(expected.First().Item2.PointB, actual.First().Item2.PointB);
        }

        private IEnumerable<Tuple<ISegment, ISegment>> BuildSegmentsPair(
            Point segment1pointA, Point segment1pointB, Point segment2pointA, Point segment2pointB) {
            return new List<Tuple<ISegment, ISegment>> { new Tuple<ISegment, ISegment>(
                new Segment(segment1pointA, segment1pointB), new Segment(segment2pointA, segment2pointB)) };
        }

    }
}