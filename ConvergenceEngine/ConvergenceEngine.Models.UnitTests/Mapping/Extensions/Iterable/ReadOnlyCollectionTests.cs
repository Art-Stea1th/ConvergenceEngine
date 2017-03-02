using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Models.UnitTests.Mapping.Extensions.Iterable {

    using Models.Mapping.Extensions.Iterable;

    [TestFixture]
    public sealed partial class ExtensionsTests {

        [Test]
        public void SplitBy_ValidSequences() {

            // Arrange
            int count = 12345, index = count / 2;

            int expectedLeftCount = index + 1;
            Point expectedLeftFirst = new Point(0, 0);
            Point expectedLeftLast = new Point(index, index);

            int expectedRightCount = count - index;
            Point expectedRightFirst = new Point(index, index);
            Point expectedRightLast = new Point(count - 1, count - 1);

            List<Point> seed = new List<Point>(count);

            for (int i = 0; i < count; ++i) {
                seed.Add(new Point(i, i));
            }
            IReadOnlyCollection<Point> sequence = seed.AsReadOnly();

            // Act
            var sequencesPair = sequence.SplitBy(index);
            List<Point> actualLeft = new List<Point>(sequencesPair.Item1);
            List<Point> actualRight = new List<Point>(sequencesPair.Item2);

            // Assert
            Assert.AreEqual(expectedLeftCount, actualLeft.Count);
            Assert.AreEqual(expectedLeftFirst, actualLeft.First());
            Assert.AreEqual(expectedLeftLast, actualLeft.Last());

            Assert.AreEqual(expectedRightCount, actualRight.Count);
            Assert.AreEqual(expectedRightFirst, actualRight.First());
            Assert.AreEqual(expectedRightLast, actualRight.Last());
        }
    }
}