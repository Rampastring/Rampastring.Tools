namespace Rampastring.Tools;

using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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
#if NETFRAMEWORK
        using var sha1 = SHA1.Create();
        byte[] hash = sha1.ComputeHash(stream);
#else
        byte[] hash = SHA1.HashData(stream);
#endif
#pragma warning restore CA5350 // Do Not Use Weak Cryptographic Algorithms

        return BytesToString(hash);
    }

    /// <summary>
    /// Calculates the SHA1 checksum of a string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>A string that represents the input string's SHA1.</returns>
    public static string CalculateSHA1ForString(string str)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(str);
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms
#if NETFRAMEWORK
        using var sha1 = SHA1.Create();
        byte[] hash = sha1.ComputeHash(buffer);
#else
        byte[] hash = SHA1.HashData(buffer);
#endif
#pragma warning restore CA5350 // Do Not Use Weak Cryptographic Algorithms
        return BytesToString(hash);
    }

    private static string BytesToString(byte[] bytes)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("x2", CultureInfo.InvariantCulture));
        }

        return sb.ToString();
    }
}