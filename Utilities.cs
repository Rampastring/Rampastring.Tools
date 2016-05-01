using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Rampastring.Tools
{
    /// <summary>
    /// A static class that contains various useful functions.
    /// </summary>
    public static class Utilities
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

            if (boolean)
                return trueString;

            return falseString;
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
        /// Calculates the SHA1 checksum of a file.
        /// </summary>
        /// <param name="path">The file's path.</param>
        /// <returns>A string that represents the file's SHA1.</returns>
        public static string CalculateSHA1ForFile(string path)
        {
            if (!File.Exists(path))
                return String.Empty;

            SHA1 sha1 = new SHA1CryptoServiceProvider();
            Stream stream = File.OpenRead(path);
            byte[] hash = sha1.ComputeHash(stream);
            return BytesToString(hash);
        }

        /// <summary>
        /// Calculates the SHA1 checksum of a string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>A string that represents the input string's SHA1.</returns>
        public static string CalculateSHA1ForString(string str)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] buffer = Encoding.ASCII.GetBytes(str);
            byte[] hash = sha1.ComputeHash(buffer);
            return BytesToString(hash);
        }

        private static string BytesToString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
