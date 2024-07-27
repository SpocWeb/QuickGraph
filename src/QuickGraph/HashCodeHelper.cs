using System;

namespace QuickGraph
{
    static class HashCodeHelper
    {
        const int FNV1_prime_32 = 16777619;
        const int FNV1_basis_32 = unchecked((int)2166136261);
        const long FNV1_prime_64 = 1099511628211;
        const long FNV1_basis_64 = unchecked((int)14695981039346656037);

        public static int GetHashCode(long x)
        {
            return Combine((int)x, (int)(((ulong)x) >> 32));
        }

        private static int Fold(int hash, byte value)
        {
            return (hash * FNV1_prime_32) ^ (int)value;
        }

        private static int Fold(int hash, int value)
        {
            return Fold(Fold(Fold(Fold(hash,
                (byte)value),
                (byte)(((uint)value) >> 8)),
                (byte)(((uint)value) >> 16)),
                (byte)(((uint)value) >> 24));
        }

        /// <summary>
        /// Combines two hashcodes in a strong way.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int Combine(int x, int y)
        {
            return Fold(Fold(FNV1_basis_32, x), y);
        }

        /// <summary>
        /// Combines three hashcodes in a strong way.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static int Combine(int x, int y, int z)
        {
            return Fold(Fold(Fold(FNV1_basis_32, x), y), z);
        }

        /// <summary>
        /// Combines four hashcodes in a strong way.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static int Combine(int x, int y, int z, int w)
        {
            return Fold(Fold(Fold(Fold(FNV1_basis_32, x), y), z), w);
        }
    }
}
