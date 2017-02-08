using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SLAM.Models.Extensions {

    public static class CollectionExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this ICollection<T> list) {
            return list.IsNull() || list.IsEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(this ICollection<T> list) {
            return list == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this ICollection<T> list) {
            return list.Count < 1;
        }
    }
}