using NUnit.Framework;
using System.Windows;
using System.Windows.Media;


namespace SLAM.Models.Mapping.UnitTests.Extensions {

    using Mapping.Extensions;

    [TestFixture]
    public sealed partial class ExtensionsTests {

        [Test]
        public void Rotate_ExtremeAngles_EqualExpected() {

            double range = 360.0 * 16.0, step = 0.1;

            for (double a = -range; a < range + step; a += step) {
                Matrix m = new Matrix();
                m.Rotate(a);

                Point expected = m.Transform(new Point(1.0, 0.0));
                Point actual = new Point(1.0, 0.0).Rotate(a);

                Assert.AreEqual(expected.X, actual.X, Common.PrecisionMax);
                Assert.AreEqual(expected.Y, actual.Y, Common.PrecisionMax);
            }
        }

        [Test]
        public void RotateAt_ExtremeAngles_EqualExpected() {

            double rangeXY = 500.0, stepXY = 1.0;
            double rangeA = 360.0 * 16.0, stepA = 0.1;

            for (double angle = -rangeA; angle < rangeA + stepA; angle += rangeA) {
                for (double atX = -rangeXY; atX < rangeXY + stepXY; atX += stepXY) {
                    for (double atY = -rangeXY; atY < rangeXY + stepXY; atY += stepXY) {

                        Matrix m = new Matrix();
                        m.RotateAt(angle, atX, atY);

                        Point expected = m.Transform(new Point(1.0, 0.0));
                        Point actual = new Point(1.0, 0.0).RotateAt(angle, atX, atY);

                        Assert.AreEqual(expected.X, actual.X, Common.PrecisionMax);
                        Assert.AreEqual(expected.Y, actual.Y, Common.PrecisionMax);
                    }
                }
            }
        }

        [Test]
        public void ConvergenceTo_ValidDirection() {

            Point sourcePoint = new Point(-1.0, -1.0);
            Point targetPoint = new Point(1.0, 1.0);

            Vector convergence = sourcePoint.ConvergenceTo(targetPoint);

            Assert.AreEqual(new Vector(2.0, 2.0), convergence);
        }
    }
}