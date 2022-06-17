using System;
using System.IO;
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
        /// Calculates the SHA1 checksum of a file.
        /// </summary>
        /// <param name="path">The file's path.</param>
        /// <returns>A string that represents the file's SHA1.</returns>
        public static string CalculateSHA1ForFile(string path)
        {
            FileInfo fileInfo = SafePath.GetFile(path);

            if (!fileInfo.Exists)
                return String.Empty;

            using SHA1 sha1 = new SHA1CryptoServiceProvider();
            using Stream stream = fileInfo.OpenRead();
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
            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                byte[] buffer = Encoding.ASCII.GetBytes(str);
                byte[] hash = sha1.ComputeHash(buffer);
                return BytesToString(hash);
            }
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
