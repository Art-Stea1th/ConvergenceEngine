using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;


namespace ConvergenceEngine.Models.Mapping.UnitTests.Common {

    /// <summary>
    ///     Represents a generator of pseudo-random numbers, a device that produces a sequence
    ///     of uniformly distributed numbers.
    /// 
    ///     by Stanislav Kuzmich [ a.k.a. Art-Stea1th: https://github.com/Art-Stea1th ]
    /// </summary>
    public sealed class RNGUniformDistribution : IDisposable {

        private Lazy<RNGCryptoServiceProvider> rngcsp;

        public RNGUniformDistribution() {
            rngcsp = new Lazy<RNGCryptoServiceProvider>(() => new RNGCryptoServiceProvider());
        }
        public void Dispose() {
            rngcsp.Value.Dispose();
        }
        ~RNGUniformDistribution() {
            Dispose();
        }

        /// <summary>
        ///     Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="min">
        ///     The inclusive lower bound of the random number returned.
        /// </param>
        /// <param name="max">
        ///     The exclusive upper bound of the random number returned. "max" must be greater
        ///     than or equal to "min".
        /// </param>
        /// <returns>
        ///     A 32-bit signed integer greater than or equal to "min" and less than "max";
        ///     that is, the range of return values includes minValue but not "max". If "min"
        ///     equals "max", "min" is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     "min" is greater than "max"
        /// </exception>
        public int Next(int min, int max) {
            if (min > max) {
                throw new ArgumentOutOfRangeException();
            }
            if (min == max) {
                return min;
            }
            return (int)((NextUnsigned() % ((long)max - min)) + min);
        }

        /// <summary>
        ///     Returns a nonnegative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="max">
        ///     The exclusive upper bound of the random number to be generated. "max" must
        ///     be greater than or equal to zero.
        /// </param>
        /// <returns>
        ///     A 32-bit signed integer greater than or equal to zero, and less than "max";
        ///     that is, the range of return values ordinarily includes zero but not "max".
        ///     However, if "max" equals zero, "max" is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     "max" is less than zero.
        /// </exception>
        public int Next(int max) {
            if (max < 0) {
                throw new ArgumentOutOfRangeException();
            }
            if (max == 0 || max == 1) {
                return 0;
            }
            return Next() % max;
        }

        /// <summary>
        ///     Returns a nonnegative random integer.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer greater than or equal to zero and less than System.Int32.MaxValue.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next() {
            return (int)(NextUnsigned() % ((uint)int.MaxValue + 1));
        }

        private uint NextUnsigned() {

            byte typeSize = sizeof(uint);
            byte[] resultData = GetCryptographicByteOrder(typeSize);

            uint result = 0;

            for (byte i = 0; i < typeSize; ++i) {
                if (i > 0)
                    result <<= 8;
                result |= resultData[i];
            }
            return result;
        }

        private byte[] GetCryptographicByteOrder(int bytesCount) {

            byte[] resultData = new byte[bytesCount];
            rngcsp.Value.GetBytes(resultData);
            return resultData;
        }
    }
}