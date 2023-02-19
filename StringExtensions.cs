namespace Rampastring.Tools;
#if !NETFRAMEWORK

using System;
#endif

public static class StringExtensions
{
    public static string SafeReplace(this string str, string oldValue, string newValue)
#if NETFRAMEWORK
        => str.Replace(oldValue, newValue);
#else
        => str.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase);
#endif

    public static int SafeIndexOf(this string str, char value)
#if NETFRAMEWORK
        => str.IndexOf(value);
#else
        => str.IndexOf(value, StringComparison.OrdinalIgnoreCase);
#endif

    public static string SafeSubstring(this string str, int startIndex)
#if NETFRAMEWORK
        => str.Substring(startIndex);
#else
        => str[startIndex..];
#endif

    public static string SafeSubstring(this string str, int startIndex, int length)
#if NETFRAMEWORK
        => str.Substring(startIndex, length);
#else
        => str[startIndex..(startIndex + length)];
#endif
}