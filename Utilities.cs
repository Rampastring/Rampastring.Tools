using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Rampastring.Tools;

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
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        FileInfo fileInfo = SafePath.GetFile(path);

        if (!fileInfo.Exists)
            return string.Empty;

        using Stream stream = fileInfo.OpenRead();
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms
#if NET7_0_OR_GREATER // Note: this is intented to be NET7_0_OR_GREATER, not NET5_0_OR_GREATER. Do not change it even if you see NET5_0_OR_GREATER below.
        byte[] hash = SHA1.HashData(stream);
#else
        using SHA1 sha1 = SHA1.Create();
        byte[] hash = sha1.ComputeHash(stream);
#endif
#pragma warning restore CA5350 // Do Not Use Weak Cryptographic Algorithms

        return BytesToHexString(hash);
    }

    /// <summary>
    /// Calculates the SHA1 checksum of a string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>A string that represents the input string's SHA1.</returns>
    public static string CalculateSHA1ForString(string str)
    {
        if (str is null)
            return string.Empty;

        byte[] buffer = Encoding.UTF8.GetBytes(str);
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms
#if NET5_0_OR_GREATER // Note: this is intended to be NET5_0_OR_GREATER, not NET7_0_OR_GREATER. Do not change it even if you see NET7_0_OR_GREATER above.
        byte[] hash = SHA1.HashData(buffer);
#else
        using SHA1 sha1 = SHA1.Create();
        byte[] hash = sha1.ComputeHash(buffer);
#endif
#pragma warning restore CA5350 // Do Not Use Weak Cryptographic Algorithms
        return BytesToHexString(hash);
    }

    /// <summary>
    /// Converts a byte array to a hexadecimal string representation.
    /// </summary>
    /// <param name="value">The byte array to convert.</param>
    /// <param name="capitalize">Indicates whether to capitalize the hexadecimal characters.</param>
    /// <returns>A string that represents the byte array in hexadecimal format, without hyphens.</returns>
    public static string BytesToHexString(byte[] value, bool capitalize = false)
    {
        return value == null ? string.Empty : BytesToHexString(value, 0, value.Length, capitalize);
    }

    /// <summary>
    /// Converts a byte array to a hexadecimal string representation.
    /// </summary>
    /// <param name="value">The byte array to convert.</param>
    /// <param name="startIndex">The index of the first byte to convert.</param>
    /// <param name="capitalize">Indicates whether to capitalize the hexadecimal characters.</param>
    /// <returns>A string that represents the byte array in hexadecimal format, without hyphens.</returns>
    public static string BytesToHexString(byte[] value, int startIndex, bool capitalize = false)
    {
        return value == null ? string.Empty : BytesToHexString(value, startIndex, value.Length - startIndex, capitalize);
    }

    /// <summary>
    /// Converts a byte array to a hexadecimal string representation.
    /// </summary>
    /// <param name="value">The byte array to convert.</param>
    /// <param name="startIndex">The index of the first byte to convert.</param>
    /// <param name="length">The number of bytes to convert. Will trim the length if it exceeds the byte array's length.</param>
    /// <param name="capitalize">Indicates whether to capitalize the hexadecimal characters.</param>
    /// <returns>A string that represents the byte array in hexadecimal format, without hyphens.</returns>
    public static string BytesToHexString(byte[] value, int startIndex, int length, bool capitalize = false)
    {
        if (value == null)
            return string.Empty;

        if (startIndex < 0)
            startIndex = 0;

        if (length < 0)
            length = 0;

        var sb = new StringBuilder();

        string byteFormat = capitalize ? "X2" : "x2";
        for (int i = startIndex; i - startIndex < length && i < value.Length; i++)
        {
            sb.Append(value[i].ToString(byteFormat, CultureInfo.InvariantCulture));
        }

        return sb.ToString();
    }
}
