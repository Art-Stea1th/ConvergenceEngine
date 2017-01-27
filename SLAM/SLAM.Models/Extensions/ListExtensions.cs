using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SLAM.Models.Extensions {

    public static class ListExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this List<T> list) {
            return list.IsNull() || list.IsEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(this List<T> list) {
            return list == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this List<T> list) {
            return list.Count < 1;
        }
    }
}