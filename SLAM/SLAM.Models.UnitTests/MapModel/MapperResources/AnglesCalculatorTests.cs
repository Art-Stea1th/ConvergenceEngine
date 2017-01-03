using NUnit.Framework;
using SLAM.Models.MapModel.MapperResources;
using System;


namespace SLAM.Models.UnitTests.MapModel.MapperResources {

    [TestFixture]
    public sealed class AnglesCalculatorTests {

        private AnglesCalculator anglesCalculator;

        [SetUp]
        public void SetUp() {
            anglesCalculator = new AnglesCalculator();
        }

        [Test]
        [TestCase(3.0, 4.0, 5.0, 36.87)]
        [TestCase(4.0, 5.0, 3.0, 53.13)]
        [TestCase(5.0, 3.0, 4.0, 90.00)]
        public void AlphaAngle_ValidSides_ReturnValidAngle(double sideA, double sideB, double sideC, double expectedAngle) {
            double resultAlpha = anglesCalculator.AlphaAngle(sideA, sideB, sideC);
            Assert.AreEqual(expectedAngle, resultAlpha, 0.005);
        }

        [Test]
        [TestCase(5.0, 4.0, 3.0, 53.13)]
        [TestCase(4.0, 3.0, 5.0, 36.87)]
        [TestCase(3.0, 5.0, 4.0, 90.00)]
        public void BetaAngle_ValidSides_ReturnValidAngle(double sideA, double sideB, double sideC, double expectedAngle) {
            double resultBeta = anglesCalculator.BetaAngle(sideA, sideB, sideC);
            Assert.AreEqual(expectedAngle, resultBeta, 0.005);
        }

        [Test]
        [TestCase(3.0, 4.0, 5.0, 90.00)]
        [TestCase(4.0, 5.0, 3.0, 36.87)]
        [TestCase(5.0, 3.0, 4.0, 53.13)]
        public void GammaAngle_ValidSides_ReturnValidAngle(double sideA, double sideB, double sideC, double expectedAngle) {
            double resultGamma = anglesCalculator.GammaAngle(sideA, sideB, sideC);
            Assert.AreEqual(expectedAngle, resultGamma, 0.005);
        }
    }
}