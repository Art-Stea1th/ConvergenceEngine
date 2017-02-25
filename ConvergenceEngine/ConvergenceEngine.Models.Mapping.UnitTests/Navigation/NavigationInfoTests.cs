using NUnit.Framework;

namespace ConvergenceEngine.Models.Mapping.UnitTests.Navigation {

    using Common;
    using Mapping.Navigation;

    [TestFixture]
    public sealed class NavigationInfoTests {

        [Test]
        public void OperatorPlus_EqualExpected() {

            // Arrange
            NavigationInfo navInfoA = new NavigationInfo(150.4, 1200.9, 2570.2);
            NavigationInfo navInfoB = new NavigationInfo(849.6, 799.1, 429.8);

            var expected = new NavigationInfo(1000.0, 2000.0, 3000.0);
            var actual = navInfoA + navInfoB; // Act

            // Assert
            Assert.AreEqual(expected.X, actual.X, Constants.PrecisionMaximum);
            Assert.AreEqual(expected.Y, actual.Y, Constants.PrecisionMaximum);
            Assert.AreEqual(expected.A, actual.A, Constants.PrecisionMaximum);
        }

        [Test]
        public void OperatorMinus_EqualExpected() {

            // Arrange
            NavigationInfo navInfoA = new NavigationInfo(-150.4, -1200.9, -2570.2);
            NavigationInfo navInfoB = new NavigationInfo(849.6, 799.1, 429.8);

            var expected = new NavigationInfo(-1000.0, -2000.0, -3000.0);
            var actual = navInfoA - navInfoB; // Act

            // Assert
            Assert.AreEqual(expected.X, actual.X, Constants.PrecisionMaximum);
            Assert.AreEqual(expected.Y, actual.Y, Constants.PrecisionMaximum);
            Assert.AreEqual(expected.A, actual.A, Constants.PrecisionMaximum);
        }
    }
}