using NUnit.Framework;

namespace SLAM.Models.Mapping.UnitTests.Navigation {

    using Mapping.Navigation;

    [TestFixture]
    public sealed class NavigationInfoTests {

        [Test]
        public void OperatorPlus_EqualExpected() {

            NavigationInfo navInfoA = new NavigationInfo(150.4, 200.9, 570.2);
            NavigationInfo navInfoB = new NavigationInfo(849.6, 799.1, 429.8);

            var expected = new NavigationInfo(1000.0, 1000.0, 1000.0);
            var actual = navInfoA + navInfoB;

            Assert.AreEqual(expected.Direction.X, actual.Direction.X, Common.PrecisionMax);
            Assert.AreEqual(expected.Direction.Y, actual.Direction.Y, Common.PrecisionMax);
            Assert.AreEqual(expected.Angle, actual.Angle, Common.PrecisionMax);
        }

        [Test]
        public void OperatorMinus_EqualExpected() {

            NavigationInfo navInfoA = new NavigationInfo(-150.4, -200.9, -570.2);
            NavigationInfo navInfoB = new NavigationInfo(849.6, 799.1, 429.8);

            var expected = new NavigationInfo(-1000.0, -1000.0, -1000.0);
            var actual = navInfoA - navInfoB;

            Assert.AreEqual(expected.Direction.X, actual.Direction.X, Common.PrecisionMax);
            Assert.AreEqual(expected.Direction.Y, actual.Direction.Y, Common.PrecisionMax);
            Assert.AreEqual(expected.Angle, actual.Angle, Common.PrecisionMax);
        }
    }
}