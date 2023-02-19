﻿namespace Rampastring.Tools;

using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

/// <summary>
/// A StringBuilder that can automatically add a separator between
/// appended strings.
/// </summary>
public class ExtendedStringBuilder : ISerializable
{
    private readonly StringBuilder stringBuilder;

    public ExtendedStringBuilder()
    {
        stringBuilder = new();
    }

    public ExtendedStringBuilder(string value, bool useSeparator)
    {
        stringBuilder = new(value);
        UseSeparator = useSeparator;
    }

    public ExtendedStringBuilder(string value, bool useSeparator, char separator)
    {
        stringBuilder = new(value);
        UseSeparator = useSeparator;
        Separator = separator;
    }

    public ExtendedStringBuilder(bool useSeparator, char separator)
    {
        stringBuilder = new();
        UseSeparator = useSeparator;
        Separator = separator;
    }

    public char Separator { get; set; }

    public bool UseSeparator { get; set; }

    public int Length => stringBuilder.Length;

    public void Append(int value)
        => Append(value.ToString(CultureInfo.InvariantCulture));

    public void Append(object value)
        => Append(value.ToString());

    public void Append(string value)
    {
        stringBuilder.Append(value);
        if (UseSeparator)
            stringBuilder.Append(Separator);
    }

    public void Remove(int startIndex, int length)
        => stringBuilder.Remove(startIndex, length);

    public override string ToString()
    {
        if (UseSeparator)
            stringBuilder.Remove(stringBuilder.Length - 1, 1);

        return stringBuilder.ToString();
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
        => ((ISerializable)stringBuilder).GetObjectData(info, context);
}