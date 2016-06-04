﻿using System;
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
    }
}