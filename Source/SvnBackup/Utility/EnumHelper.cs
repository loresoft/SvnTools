using System;
using System.Collections.Generic;
using System.Text;

// $Id$

namespace SvnBackup.Utility
{
    /// <summary>
    /// A class to help with Enum Flags.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Determines whether any flag is on for the specified mask.
        /// </summary>
        /// <typeparam name="T">The flag type.</typeparam>
        /// <param name="mask">The mask to check if the flag is on.</param>
        /// <param name="flag">The flag to check for in the mask.</param>
        /// <returns>
        /// 	<c>true</c> if any flag is on for the specified mask; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAnyFlagOn<T>(T mask, T flag)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            ulong flagInt = Convert.ToUInt64(flag);
            ulong maskInt = Convert.ToUInt64(mask);

            return (maskInt & flagInt) != 0;
        }

        /// <summary>
        /// Determines whether the flag is on for the specified mask.
        /// </summary>
        /// <typeparam name="T">The flag type.</typeparam>
        /// <param name="mask">The mask to check if the flag is on.</param>
        /// <param name="flag">The flag to check for in the mask.</param>
        /// <returns>
        /// 	<c>true</c> if the flag is on for the specified mask; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFlagOn<T>(T mask, T flag)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            ulong flagInt = Convert.ToUInt64(flag);
            ulong maskInt = Convert.ToUInt64(mask);

            return (maskInt & flagInt) == flagInt;
        }

        /// <summary>
        /// Sets the flag on in the specified mask.
        /// </summary>
        /// <typeparam name="T">The flag type.</typeparam>
        /// <param name="mask">The mask to set flag on.</param>
        /// <param name="flag">The flag to set.</param>
        /// <returns>The mask with the flag set to on.</returns>
        public static T SetFlagOn<T>(T mask, T flag)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            ulong flagInt = Convert.ToUInt64(flag);
            ulong maskInt = Convert.ToUInt64(mask);

            maskInt |= flagInt;

            return ConvertFlag<T>(maskInt);
        }



        /// <summary>
        /// Sets the flag off in the specified mask.
        /// </summary>
        /// <typeparam name="T">The flag type.</typeparam>
        /// <param name="mask">The mask to set flag off.</param>
        /// <param name="flag">The flag to set.</param>
        /// <returns>The mask with the flag set to off.</returns>
        public static T SetFlagOff<T>(T mask, T flag)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            ulong flagInt = Convert.ToUInt64(flag);
            ulong maskInt = Convert.ToUInt64(mask);

            maskInt &= ~flagInt;

            return ConvertFlag<T>(maskInt);
        }

        /// <summary>
        /// Toggles the flag in the specified mask.
        /// </summary>
        /// <typeparam name="T">The flag type.</typeparam>
        /// <param name="mask">The mask to toggle the flag against.</param>
        /// <param name="flag">The flag to toggle.</param>
        /// <returns>The mask with the flag set in the opposite position then it was.</returns>
        public static T ToggleFlag<T>(T mask, T flag)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            ulong flagInt = Convert.ToUInt64(flag);
            ulong maskInt = Convert.ToUInt64(mask);

            maskInt ^= flagInt;

            return ConvertFlag<T>(maskInt);
        }

        /// <summary>
        /// Gets the string hex of the enum.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="enum">The enum to get the string hex from.</param>
        /// <returns></returns>
        public static string ToStringHex<T>(T @enum)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return string.Format("{0:x8}", @enum); //hex            
        }

        /// <summary>
        /// Tries to get an enum from a string.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="input">The input string.</param>
        /// <param name="returnValue">The return enum value.</param>
        /// <returns>
        /// 	<c>true</c> if the string was able to be parsed to an enum; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryParseEnum<T>(string input, out T returnValue)
             where T : struct, IComparable, IFormattable, IConvertible
        {
            Type t = typeof(T);
            if (t.IsEnum && Enum.IsDefined(t, input))
            {
                returnValue = (T)Enum.Parse(t, input, true);
                return true;
            }
            returnValue = default(T);
            return false;
        }

        private static T ConvertFlag<T>(ulong maskInt)
        {
            Type t = typeof(T);
            if (t.IsEnum)
                return (T)Enum.ToObject(t, maskInt);

            return (T)Convert.ChangeType(maskInt, t);
        }
    }
}
