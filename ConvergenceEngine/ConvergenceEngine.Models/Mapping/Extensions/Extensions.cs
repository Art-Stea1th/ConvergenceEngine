using System.Runtime.CompilerServices;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    internal static partial class Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(this T anyReferenceType) where T : class {
            return anyReferenceType == null;
        }
    }
}