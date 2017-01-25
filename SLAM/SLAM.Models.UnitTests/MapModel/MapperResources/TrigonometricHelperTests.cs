using NUnit.Framework;
using SLAM.Models.MapModel.BrutforceMapperResources;

namespace SLAM.Models.UnitTests.MapModel.MapperResources {

    [TestFixture]
    public sealed class TrigonometricHelperTests {

        [Test]
        [TestCase(0.0, 0.0000)]
        [TestCase(30.0, 0.5236)]
        [TestCase(45.0, 0.7854)]
        [TestCase(60.0, 1.0472)]
        [TestCase(90.0, 1.5708)]
        [TestCase(120.0, 2.0944)]
        [TestCase(135.0, 2.3562)]
        [TestCase(150.0, 2.6180)]
        [TestCase(180.0, 3.1416)]
        [TestCase(270.0, 4.7124)]
        [TestCase(360.0, 6.2832)]
        [TestCase(999.9, 17.4515)]
        public void RadiansFromDegrees_AfterConvert_EqualsExpected(double degrees, double expectedRadians) {
            double radians = TrigonometricHelper.RadiansFromDegrees(degrees);
            Assert.AreEqual(expectedRadians, radians, 0.00005);
        }

        [Test]
        [TestCase(0.0000, 0.0)]
        [TestCase(0.5236, 30.0)]
        [TestCase(0.7854, 45.0)]
        [TestCase(1.0472, 60.0)]
        [TestCase(1.5708, 90.0)]
        [TestCase(2.0944, 120.0)]
        [TestCase(2.3562, 135.0)]
        [TestCase(2.6180, 150.0)]
        [TestCase(3.1416, 180.0)]
        [TestCase(4.7124, 270.0)]
        [TestCase(6.2832, 360.0)]
        [TestCase(17.4515, 999.9)]
        public void DegreesFromRadians_AfterConvert_EqualsExpected(double radians, double expectedDegrees) {
            double degrees = TrigonometricHelper.DegreesFromRadians(radians);
            Assert.AreEqual(expectedDegrees, degrees, 0.005);
        }
    }
}