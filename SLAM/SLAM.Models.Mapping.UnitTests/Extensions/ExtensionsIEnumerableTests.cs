using NUnit.Framework;
using System.Windows;

namespace SLAM.Models.Mapping.UnitTests.Extensions {

    using Mapping.Extensions;

    [TestFixture]
    public sealed partial class ExtensionsTests {

        [Test]
        public void Sum_EqualExpected() {

            var vectors = new Vector[] {
                new Vector(44.4, 144.4), new Vector(144.4, -44.4), new Vector(-44.4, -144.4), new Vector(-144.4, 44.4) };

            Vector expected = new Vector(0.0, 0.0);
            Vector actual = vectors.Sum();

            Assert.AreEqual(expected.X, actual.X, Common.PrecisionMax);
            Assert.AreEqual(expected.Y, actual.Y, Common.PrecisionMax);
        }
    }
}