using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ConvergenceEngine.Models.Mapping.UnitTests.Common {

    [TestFixture]
    public sealed class RNGUniformDistributionTests {

        private const int iterations = 1000;

        [Test]
        public void Next_UniformDistibution() {

            // Arrange
            int numbersVariety = 10;                        // 0 - 9
            int iterationsCount = 1000000 * numbersVariety; // same for each number

            // Key - number, Value - distribution percent
            SortedList<int, double> numberDistibutionPercent = new SortedList<int, double>(numbersVariety);

            using (var random = new RNGUniformDistribution()) {
                // Act: count
                for (int i = 0; i < iterationsCount; ++i) {

                    int nextNumber = random.Next() % numbersVariety; // Act

                    if (numberDistibutionPercent.ContainsKey(nextNumber)) {
                        numberDistibutionPercent[nextNumber] += 1.0;
                        continue;
                    }
                    numberDistibutionPercent.Add(nextNumber, 1.0);
                }
            }

            // Act: count to percent
            for (int i = 0; i < numberDistibutionPercent.Count; ++i) {
                numberDistibutionPercent[i] = numberDistibutionPercent[i] * 100 / iterationsCount;
            }

            // Assert
            for (int i = 0; i < numberDistibutionPercent.Count; ++i) {
                for (int j = i + 1; j < numberDistibutionPercent.Count; ++j) {
                    Assert.AreEqual(numberDistibutionPercent[i], numberDistibutionPercent[j], 1.0E-1);
                }
            }
        }

        [Test]
        public void Next_WithMaxValue_Max_Zero_One_AlwaysReturnZero() {

            using (var random = new RNGUniformDistribution()) {
                for (int i = 0; i < iterations; ++i) {
                    Assert.AreEqual(0, random.Next(0));
                    Assert.AreEqual(0, random.Next(1));
                }
            }
        }

        [Test]
        public void Next_WithMaxValue_ThrowMaxLessThenZero() {
            using (var random = new RNGUniformDistribution()) {
                Assert.Throws(typeof(ArgumentOutOfRangeException), new TestDelegate(() => random.Next(-1)));
            }
        }

        [Test]
        public void Next_WithMaxValue_ValidRange() {

            // Arrange
            int max = 2;
            List<int> numbers = new List<int>(iterations);

            // Act
            using (var random = new RNGUniformDistribution()) {
                for (int i = 0; i < iterations; ++i) {
                    numbers.Add(random.Next(max));
                }
            }

            // Assert: exists items from generator range
            for (int i = 0; i < max; i++) {
                Assert.IsTrue(numbers.Exists(n => n == i));
            }

            // Assert: all numbers within the range
            foreach (var number in numbers) {
                Assert.IsTrue(number >= 0 && number < max);
            }
        }

        [Test]
        public void Next_WithMinMaxValue_ThrowMaxLessThenMin() {
            using (var random = new RNGUniformDistribution()) {
                Assert.Throws(typeof(ArgumentOutOfRangeException), new TestDelegate(() => random.Next(5, 4)));
            }
        }

        [Test]
        public void Next_WithMinMaxValue_NegativeLimit_EqualExpected() {
            using (var random = new RNGUniformDistribution()) {
                for (int i = 0; i < iterations; ++i) {
                    int expected = int.MinValue;
                    int actual = random.Next(int.MinValue, int.MinValue + 1);
                    Assert.AreEqual(expected, actual);
                }
            }
        }

        [Test]
        public void Next_WithMinMaxValue_NonnegativeLimit_EqualExpected() {
            using (var random = new RNGUniformDistribution()) {
                for (int i = 0; i < iterations; ++i) {
                    int expected = int.MaxValue - 1;
                    int actual = random.Next(int.MaxValue - 1, int.MaxValue);
                    Assert.AreEqual(expected, actual);
                }
            }
        }

        [Test]
        public void Next_WithMinMaxValue_ValidRange() {

            // Arrange
            int min = -1, max = 2;
            List<int> numbers = new List<int>(iterations);

            // Act
            using (var random = new RNGUniformDistribution()) {
                for (int i = 0; i < iterations; ++i) {
                    numbers.Add(random.Next(min, max));
                }
            }

            // Assert: exists items from generator range
            for (int i = min; i < max; i++) {
                Assert.IsTrue(numbers.Exists(n => n == i));
            }

            // Assert: all numbers within the range
            foreach (var number in numbers) {
                Assert.IsTrue(number >= min && number < max);
            }
        }
    }
}