using NUnit.Framework;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Models.UnitTests.Mapping.Extensions.Ops {

    using Common;
    using Models.Mapping.Extensions.Ops;

    [TestFixture]
    public sealed class ApproximatorTests {

        [Test]
        public void Approximate_ValidPoints() {

            // Arrange
            double xStart = -250.0, xEnd = 250.0, step = 0.5, yConst = 200.0, errX = 0.2, errY = 0.5;
            int xSeed = 20, ySeed = 250;

            for (int i = 0; i < 1000; ++i) {                                       // Test i = 1M PASS

                List<Point> line = new List<Point>();
                var expectedPoint1 = new Point(xStart, yConst);
                var expectedPoint2 = new Point(xEnd, yConst);

                line.Add(expectedPoint1);
                using (var random = new RNGUniformDistribution()) {
                    for (double x = xStart + step; x < xEnd - step; x += step) {
                        double currX = x + random.Next(-xSeed, xSeed) * 0.01;      // +/- 0.2 | WARNING!! Random();
                        double currY = yConst + random.Next(-ySeed, ySeed) * 0.01; // +/- 1.0 | WARNING!! Random();
                        line.Add(new Point(currX, currY));
                    }
                }
                line.Add(expectedPoint2);

                // Act
                var resultPair = line.ApproximateSorted();

                var actualPoint1 = resultPair.Item1;
                var actualPoint2 = resultPair.Item2;

                // Assert
                Assert.AreEqual(expectedPoint1.X, actualPoint1.X, errX);
                Assert.AreEqual(expectedPoint1.Y, actualPoint1.Y, errY);

                Assert.AreEqual(expectedPoint2.X, actualPoint2.X, errX);
                Assert.AreEqual(expectedPoint2.Y, actualPoint2.Y, errY);
            }
        }
    }
}