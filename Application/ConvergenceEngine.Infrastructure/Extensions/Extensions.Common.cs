using System.Runtime.CompilerServices;

namespace ConvergenceEngine.Infrastructure.Extensions {

    public static partial class Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(this T anyReferenceType) where T : class {
            return anyReferenceType == null;
        }
    }
}