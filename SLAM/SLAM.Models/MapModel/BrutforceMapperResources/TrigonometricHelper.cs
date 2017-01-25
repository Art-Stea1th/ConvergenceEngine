using System;
using System.Runtime.CompilerServices;

namespace SLAM.Models.MapModel.BrutforceMapperResources {

    internal static class TrigonometricHelper {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DegreesFromRadians(double radians) {
            return (radians * 180.0) / Math.PI;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double RadiansFromDegrees(double degrees) {
            return (Math.PI / 180.0) * degrees;
        }
    }
}