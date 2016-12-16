using NUnit.Framework;
using SLAM.Models.MapModel.MapperResources;
using System;


namespace SLAM.Models.UnitTests.MapModel.MapperResources {

    [TestFixture]
    public sealed class AnglesCalculatorTests {

        [Test]
        [TestCase(3.0, 4.0, 5.0, 36.87)]
        [TestCase(4.0, 5.0, 3.0, 53.13)]
        [TestCase(5.0, 3.0, 4.0, 90.00)]
        public void AlphaAngle_ValidSides_ReturnValidAngle(double sideA, double sideB, double sideC, double expectedAngle) {
            AnglesCalculator ac = new AnglesCalculator();               // --- Arrange            
            double resultGamma = ac.AlphaAngle(sideA, sideB, sideC);    // --- Act            
            Assert.AreEqual(expectedAngle, Math.Round(resultGamma, 2)); // --- Assert
        }

        [Test]
        [TestCase(5.0, 4.0, 3.0, 53.13)]
        [TestCase(4.0, 3.0, 5.0, 36.87)]
        [TestCase(3.0, 5.0, 4.0, 90.00)]
        public void BetaAngle_ValidSides_ReturnValidAngle(double sideA, double sideB, double sideC, double expectedAngle) {
            AnglesCalculator ac = new AnglesCalculator();               // --- Arrange            
            double resultGamma = ac.BetaAngle(sideA, sideB, sideC);     // --- Act            
            Assert.AreEqual(expectedAngle, Math.Round(resultGamma, 2)); // --- Assert
        }

        [Test]
        [TestCase(3.0, 4.0, 5.0, 90.00)]
        [TestCase(4.0, 5.0, 3.0, 36.87)]
        [TestCase(5.0, 3.0, 4.0, 53.13)]
        public void GammaAngle_ValidSides_ReturnValidAngle(double sideA, double sideB, double sideC, double expectedAngle) {
            AnglesCalculator ac = new AnglesCalculator();               // --- Arrange            
            double resultGamma = ac.GammaAngle(sideA, sideB, sideC);    // --- Act            
            Assert.AreEqual(expectedAngle, Math.Round(resultGamma, 2)); // --- Assert
        }
    }
}