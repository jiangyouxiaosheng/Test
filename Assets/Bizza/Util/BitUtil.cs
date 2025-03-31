using System;

namespace Bizza.Library
{
    public static class BitUtil
    {
        /// <summary>
        /// 判断 value 是否包含 mask 中的一部分
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static bool Any(long value, long mask)
        {
            return (value & mask) != 0;
        }
        public static bool Any(int value, int mask)
        {
            return (value & mask) != 0;
        }

        /// <summary>
        /// 判断 value 是否完全包含 mask
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static bool All(long value, long mask)
        {
            return (value & mask) == mask;
        }
        public static bool All(int value, int mask)
        {
            return (value & mask) == mask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <param name="bool"></param>
        /// <returns></returns>
        public static long SetMask(long value, long mask, bool @bool)
        {
            return @bool ? value | mask : value & ~mask;
        }
        public static int SetMask(int value, int mask, bool @bool)
        {
            return @bool ? value | mask : value & ~mask;
        }
        public static void SetMask(ref long value, long mask, bool @bool)
        {
            value = SetMask(value, mask, @bool);
        }
        public static void SetMask(ref int value, int mask, bool @bool)
        {
            value = SetMask(value, mask, @bool);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static long ToMaskL(int index)
        {
            return 1L << index;
        }
        public static int ToMaskI(int index)
        {
            return 1 << index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <param name="bool"></param>
        /// <returns></returns>
        public static long SetIndex(long value, int index, bool @bool)
        {
            long mask = ToMaskL(index);
            return SetMask(value, mask, @bool);
        }
        public static int SetIndex(int value, int index, bool @bool)
        {
            int mask = ToMaskI(index);
            return SetMask(value, mask, @bool);
        }
        public static void SetIndex(ref long value, int index, bool @bool)
        {
            value = SetIndex(value, index, @bool);
        }
        public static void SetIndex(ref int value, int index, bool @bool)
        {
            value = SetIndex(value, index, @bool);
        }
    }
}

