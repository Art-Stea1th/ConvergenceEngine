using System;
using System.Runtime.CompilerServices;

namespace SLAM.Models.Mapping.Extensions {

    internal static partial class Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDegrees(this double radians) {
            return (radians * 180.0) / Math.PI;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRadians(this double degrees) {
            return (Math.PI / 180.0) * degrees;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AsNormalizedAngle(this double degrees) {
            return ((int)(degrees %= 360.0) / 180) % 2 == 0 ? degrees : (degrees < 0 ? degrees + 360.0 : degrees - 360.0);
            //       if it even so (-180 < degrees < 180),    return;  //  else clamp degrees to -180 / 180 and return;
        }
    }
}