using System;
using System.Runtime.CompilerServices;

namespace SLAM.Models.Map.BrutforceMapperResources {

    internal sealed class AngleCalculator {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double AlphaAngle(double a, double b, double c) {
            return RadiansToDegrees(RadiansAngle(c, b, a));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double BetaAngle(double a, double b, double c) {
            return RadiansToDegrees(RadiansAngle(a, c, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GammaAngle(double a, double b, double c) {  
            return RadiansToDegrees(RadiansAngle(a, b, c));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double RadiansAngle(double a, double b, double c) {
            return // radians γ = arccos((a² + b² - c²) / (2ab))
                Math.Acos(
                    (Math.Pow(a, 2.0) + Math.Pow(b, 2.0) - Math.Pow(c, 2.0))
                    / (2.0 * a * b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double RadiansToDegrees(double radians) {
            return (radians * 180.0) / Math.PI;
        }
    }
}