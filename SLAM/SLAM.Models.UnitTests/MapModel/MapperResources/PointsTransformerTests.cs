using NUnit.Framework;
using System.Windows;

namespace SLAM.Models.UnitTests.MapModel.MapperResources {

    using SLAM.Models.MapModel.MapperResources;

    [TestFixture]
    public sealed class PointsTransformerTests {

        private PointsTransformer transformer;

        [SetUp]
        public void SetUp() {
            transformer = new PointsTransformer();
        }

        [Test]
        public void Rotate_AfterRotate_EqualExpected() {

            var points = new [] { new Point(-1.0, -1.0), new Point(1.0, 1.0) };
            var expected = new [] { new Point(-1.0, 1.0), new Point(1.0, -1.0) };

            transformer.Rotate(points, 90.0);

            Assert.AreEqual(expected[0].Y, points[0].Y, 0.00000005);
            Assert.AreEqual(expected[0].X, points[0].X, 0.00000005);

            Assert.AreEqual(expected[1].X, points[1].X, 0.00000005);
            Assert.AreEqual(expected[1].Y, points[1].Y, 0.00000005);
        }

        [Test]
        [TestCase(1.0, 1.0,  0.0,    1.0000,  1.0000)]
        [TestCase(1.0, 1.0,  30.0,   1.3660,  0.3660)]
        [TestCase(1.0, 1.0,  45.0,   1.4142,  0.0000)]
        [TestCase(1.0, 1.0,  60.0,   1.3660, -0.3660)]
        [TestCase(1.0, 1.0,  90.0,   1.0000, -1.0000)]
        [TestCase(1.0, 1.0,  120.0,  0.3660, -1.3660)]
        [TestCase(1.0, 1.0,  135.0,  0.0000, -1.4142)]
        [TestCase(1.0, 1.0,  150.0, -0.3660, -1.3660)]
        [TestCase(1.0, 1.0,  180.0, -1.0000, -1.0000)]
        [TestCase(1.0, 1.0,  270.0, -1.0000,  1.0000)]
        [TestCase(1.0, 1.0,  360.0,  1.0000,  1.0000)]
        [TestCase(1.0, 1.0,  999.9, -0.8132,  1.1570)]
        [TestCase(1.0, 1.0, -30.0,   0.3660,  1.3660)]
        [TestCase(1.0, 1.0, -45.0,   0.0000,  1.4142)]
        [TestCase(1.0, 1.0, -60.0,  -0.3660,  1.3660)]
        [TestCase(1.0, 1.0, -90.0,  -1.0000,  1.0000)]
        [TestCase(1.0, 1.0, -120.0, -1.3660,  0.3660)]
        [TestCase(1.0, 1.0, -135.0, -1.4142,  0.0000)]
        [TestCase(1.0, 1.0, -150.0, -1.3660, -0.3660)]
        [TestCase(1.0, 1.0, -180.0, -1.0000, -1.0000)]
        [TestCase(1.0, 1.0, -270.0,  1.0000, -1.0000)]
        [TestCase(1.0, 1.0, -360.0,  1.0000,  1.0000)]
        [TestCase(1.0, 1.0, -999.9,  1.1570, -0.8132)]
        public void Rotate_AfterRotate_EqualExpected(double x, double y, double angle, double expectedX, double expectedY) {
            Point resultPoint = transformer.Rotate(new Point(x, y), angle);
            Assert.AreEqual(expectedX, resultPoint.X, 0.00005);
            Assert.AreEqual(expectedY, resultPoint.Y, 0.00005);
        }        

        [Test]
        [TestCase(-1.0, -1.0, 1.0, 1.0,  1.0,  1.0)]
        [TestCase(-1.0, -1.0, 1.0, 1.0, -1.0,  1.0)]
        [TestCase(-1.0, -1.0, 1.0, 1.0,  1.0, -1.0)]
        [TestCase(-1.0, -1.0, 1.0, 1.0, -1.0, -1.0)]
        public void ShiftXY_AfterShift_EqualExpected(double x1, double y1, double x2, double y2, double offsetX, double offsetY) {

            var points = new [] { new Point(x1, y1), new Point(x2, y2) };
            var expected = new [] { new Point(x1 + offsetX, y1 + offsetY), new Point(x2 + offsetX, y2 + offsetY) };

            transformer.ShiftXY(points, offsetX, offsetY);

            Assert.AreEqual(expected[0].X, points[0].X, 0.00000005);
            Assert.AreEqual(expected[0].Y, points[0].Y, 0.00000005);

            Assert.AreEqual(expected[1].X, points[1].X, 0.00000005);
            Assert.AreEqual(expected[1].Y, points[1].Y, 0.00000005);
        }

        [Test]
        [TestCase(-1.0, -1.0, 1.0, 1.0, 1.0)]
        [TestCase(-1.0, -1.0, 1.0, 1.0, -1.0)]
        public void ShiftX_AfterShift_EqualExpected(double x1, double y1, double x2, double y2, double offsetX) {

            var points = new [] { new Point(x1, y1), new Point(x2, y2) };
            var expected = new [] { new Point(x1 + offsetX, y1), new Point(x2 + offsetX, y2) };

            transformer.ShiftX(points, offsetX);

            Assert.AreEqual(expected[0].X, points[0].X, 0.00000005);
            Assert.AreEqual(expected[0].Y, points[0].Y, 0.00000005);

            Assert.AreEqual(expected[1].X, points[1].X, 0.00000005);
            Assert.AreEqual(expected[1].Y, points[1].Y, 0.00000005);
        }
        
        [Test]
        [TestCase(-1.0, -1.0, 1.0, 1.0, 1.0)]
        [TestCase(-1.0, -1.0, 1.0, 1.0, -1.0)]
        public void ShiftY_AfterShift_EqualExpected(double x1, double y1, double x2, double y2, double offsetY) {

            var points = new [] { new Point(x1, y1), new Point(x2, y2) };
            var expected = new [] { new Point(x1, y1 + offsetY), new Point(x2, y2 + offsetY) };

            transformer.ShiftY(points, offsetY);

            Assert.AreEqual(expected[0].X, points[0].X, 0.00000005);
            Assert.AreEqual(expected[0].Y, points[0].Y, 0.00000005);

            Assert.AreEqual(expected[1].X, points[1].X, 0.00000005);
            Assert.AreEqual(expected[1].Y, points[1].Y, 0.00000005);
        }
    }
}