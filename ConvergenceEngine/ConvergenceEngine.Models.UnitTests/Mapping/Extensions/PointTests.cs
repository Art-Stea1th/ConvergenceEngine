using NUnit.Framework;
using System.Windows;
using System.Windows.Media;

namespace ConvergenceEngine.Models.UnitTests.Mapping.Extensions {

    using Common;
    using Models.Mapping.Extensions;

    [TestFixture]
    public sealed partial class ExtensionsTests {

        [Test]
        public void Rotate_ExtremeAngles_EqualExpected() {

            // Arrange
            double range = 360.0 * 16.0, step = 0.1;

            for (double a = -range; a < range + step; a += step) {

                // Arrange
                Matrix m = new Matrix();
                m.Rotate(a);

                Point expected = m.Transform(new Point(1.0, 0.0));
                Point actual = new Point(1.0, 0.0).Rotate(a); // Act

                // Assert
                Assert.AreEqual(expected.X, actual.X, Constants.PrecisionMaximum);
                Assert.AreEqual(expected.Y, actual.Y, Constants.PrecisionMaximum);
            }
        }

        [Test]
        public void RotateAt_ExtremeAngles_EqualExpected() {

            // Arrange
            double rangeXY = 500.0, stepXY = 1.0;
            double rangeA = 360.0 * 16.0, stepA = 0.1;

            for (double angle = -rangeA; angle < rangeA + stepA; angle += rangeA) {
                for (double atX = -rangeXY; atX < rangeXY + stepXY; atX += stepXY) {
                    for (double atY = -rangeXY; atY < rangeXY + stepXY; atY += stepXY) {

                        // Arrange
                        Matrix m = new Matrix();
                        m.RotateAt(angle, atX, atY);

                        Point expected = m.Transform(new Point(1.0, 0.0));
                        Point actual = new Point(1.0, 0.0).RotateAt(angle, atX, atY); // Act

                        // Assert
                        Assert.AreEqual(expected.X, actual.X, Constants.PrecisionMaximum);
                        Assert.AreEqual(expected.Y, actual.Y, Constants.PrecisionMaximum);
                    }
                }
            }
        }

        [Test]
        public void DistancePointTo_PointIsTheStartOrEndOfTheSegment_ReturnSourcePoint() {

            // Arrange
            Point pointA = new Point(-1.0, 1.0);
            Point pointB = new Point(1.0, -1.0);
            Point pointC1 = pointA;
            Point pointC2 = pointB;

            Point expectedPoint1 = pointC1;
            Point expectedPoint2 = pointC2;

            // Act
            Point actualPoint1 = pointC1.DistancePointTo(pointA, pointB);
            Point actualPoint2 = pointC2.DistancePointTo(pointA, pointB);

            // Assert
            Assert.AreEqual(expectedPoint1, actualPoint1);
            Assert.AreEqual(expectedPoint2, actualPoint2);
        }

        [Test]
        public void DistancePointTo_ZeroLengthSegment_ReturnAnyPointOfSegment() {

            // Arrange
            Point pointA = new Point(-1.0, -1.0);
            Point pointB = new Point(-1.0, -1.0);
            Point pointC = new Point(1.0, 1.0);

            Point expectedPoint1 = pointA;
            Point expectedPoint2 = pointB;

            // Act
            Point actualPoint = pointC.DistancePointTo(pointA, pointB);

            // Assert
            Assert.AreEqual(expectedPoint1, actualPoint);
            Assert.AreEqual(expectedPoint2, actualPoint);
        }

        [Test]
        public void ConvergenceTo_ValidDirection() {

            // Arrange
            Point sourcePoint = new Point(-1.0, -1.0);
            Point targetPoint = new Point(1.0, 1.0);

            Vector expectedConvergence = new Vector(2.0, 2.0);
            Vector actualConvergence = sourcePoint.ConvergenceTo(targetPoint); // Act

            // Assert
            Assert.AreEqual(expectedConvergence, actualConvergence);
        }
    }
}