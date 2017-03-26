using System;
using System.Windows;

namespace ConvergenceEngine.Infrastructure.Extensions {

    public static partial class Extensions {

        public static double ToDegrees(this double radians)
            => (radians * 180.0) / Math.PI;

        public static double ToRadians(this double degrees)
            => (Math.PI / 180.0) * degrees;

        public static double AsNormalizedAngle(this double degrees)
            => ((int)(degrees %= 360.0) / 180) % 2 == 0 ? degrees : (degrees < 0 ? degrees + 360.0 : degrees - 360.0);

        public static Vector RadiansToVector(this double radians)
            => new Vector(Math.Cos(radians), Math.Sin(radians));

        public static Vector DegreesToVector(this double degrees)
            => new Vector(Math.Cos(degrees.ToRadians()), Math.Sin(degrees.ToRadians()));
    }
}