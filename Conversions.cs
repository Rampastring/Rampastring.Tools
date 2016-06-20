using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Rampastring.Tools
{
    /// <summary>
    /// Provides static methods for converting data types.
    /// </summary>
    public static class Conversions
    {
        /// <summary>
        /// Converts a string to a boolean.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <param name="defaultValue">The default value to return if the conversion fails.</param>
        /// <returns>A boolean based on the given string.</returns>
        public static bool BooleanFromString(string str, bool defaultValue)
        {
            if (String.IsNullOrEmpty(str))
                return defaultValue;

            char firstChar = str.ToLower()[0];

            switch (firstChar)
            {
                case 't':
                case 'y':
                case '1':
                case 'a':
                case 'e':
                    return true;
                case 'n':
                case 'f':
                case '0':
                    return false;
                default:
                    return defaultValue;
            }
        }

        /// <summary>
        /// Converts a boolean to a string with the specified style.
        /// </summary>
        /// <param name="boolean">The boolean.</param>
        /// <param name="stringStyle">The style of the boolean string.</param>
        /// <returns>A string that represents the boolean with the specified style.</returns>
        public static string BooleanToString(bool boolean, BooleanStringStyle stringStyle)
        {
            string trueString = "True";
            string falseString = "False";

            if (stringStyle == BooleanStringStyle.YESNO)
            {
                trueString = "Yes";
                falseString = "No";
            }

            return boolean ? trueString : falseString;
        }

        /// <summary>
        /// Converts a string with the English number format to a float.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <param name="defaultValue">The default value to return if the conversion fails.</param>
        /// <returns>A float based on the given string.</returns>
        public static float FloatFromString(string str, float defaultValue)
        {
            try
            {
                return Convert.ToSingle(str, CultureInfo.GetCultureInfo("en-US").NumberFormat);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Converts a string with the English number format to a double.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <param name="defaultValue">The default value to return if the conversion fails.</param>
        /// <returns>A double based on the given string.</returns>
        public static double DoubleFromString(string str, double defaultValue)
        {
            try
            {
                return Convert.ToDouble(str, CultureInfo.GetCultureInfo("en-US").NumberFormat);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Converts a string with the English number format to an integer.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <param name="defaultValue">The default value to return if the conversion fails.</param>
        /// <returns>An integer based on the given string.</returns>
        public static int IntFromString(string str, int defaultValue)
        {
            if (String.IsNullOrEmpty(str))
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Int32.Parse(str);
                }
                catch
                {
                    return defaultValue;
                }
            }
        }

        public static int[] IntArrayFromStringArray(string[] array)
        {
            int[] intArray = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
                intArray[i] = Int32.Parse(array[i]);

            return intArray;
        }

        /// <summary>
        /// Converts an array of booleans into an array of bytes,
        /// packing 8 boolean values into a single byte.
        /// </summary>
        /// <param name="boolArray">The boolean array.</param>
        /// <returns>The generated array of bytes.</returns>
        public static byte[] BoolArrayIntoBytes(bool[] boolArray)
        {
            // Slight modification of Marc Gravell's code at
            // http://stackoverflow.com/questions/713057/convert-bool-to-byte

            int byteCount = boolArray.Length / 8;
            if ((boolArray.Length % 8) != 0)
                byteCount++;

            byte[] bytes = new byte[byteCount];
            int optionIndex = 0;
            int byteIndex = 0;

            for (int i = 0; i < boolArray.Length; i++)
            {
                if (boolArray[i])
                {
                    bytes[byteIndex] |= (byte)(((byte)1) << optionIndex);
                }
                optionIndex++;
                if (optionIndex == 8)
                {
                    optionIndex = 0;
                    byteIndex++;
                }
            }

            return bytes;
        }

        public static bool[] BytesIntoBoolArray(byte[] byteArray)
        {
            int booleanCount = byteArray.Length * 8;
            bool[] boolArray = new bool[booleanCount];

            // Worth reading: 
            // http://stackoverflow.com/questions/141525/what-are-bitwise-shift-bit-shift-operators-and-how-do-they-work

            for (int i = 0; i < byteArray.Length; i++)
            {
                byte b = byteArray[i];
                bool[] booleans = ByteToBoolArray(b);

                for (int j = 0; j < booleans.Length; j++)
                {
                    boolArray[i * 8 + j] = booleans[j];
                }
            }

            return boolArray;
        }

        /// <summary>
        /// Converts a byte to an array of 8 booleans.
        /// </summary>
        /// <param name="b">The byte.</param>
        /// <returns>An array of 8 booleans.</returns>
        public static bool[] ByteToBoolArray(byte b)
        {
            // prepare the return result
            bool[] result = new bool[8];

            // check each bit in the byte. if 1 set to true, if 0 set to false
            for (int i = 0; i < 8; i++)
                result[i] = (b & (1 << i)) == 0 ? false : true;

            // reverse the array
            // Array.Reverse(result);

            return result;
        }
    }
}
