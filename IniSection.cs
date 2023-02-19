// Rampastring's INI parser
// http://www.moddb.com/members/rampastring

namespace Rampastring.Tools;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

/// <summary>
/// Represents a [section] in an INI file.
/// </summary>
public class IniSection : IIniSection
{
    public List<KeyValuePair<string, string>> Keys = new();

    public IniSection()
    {
    }

    public IniSection(string sectionName)
    {
        SectionName = sectionName;
    }

    public string SectionName { get; set; }

    /// <summary>
    /// Adds a key to the INI section.
    /// Throws a <see cref="InvalidOperationException"/> if the key already exists.
    /// Use <see cref="AddOrReplaceKey(string, string)"/> if you want to replace
    /// an existing key instead.
    /// </summary>
    /// <param name="keyName">The name of the INI key.</param>
    /// <param name="value">The value of the INI key.</param>
    public void AddKey(string keyName, string value)
    {
        if (keyName == null || value == null)
            throw new ArgumentException("INI keys cannot have null key names or values.");

        if (Keys.FindIndex(kvp => kvp.Key == keyName) > -1)
            throw new InvalidOperationException("The given key already exists in the section!");

        Keys.Add(new(keyName, value));
    }

    /// <summary>
    /// Adds a key to the INI section, or replaces the key's value if the key
    /// already exists.
    /// </summary>
    /// <param name="keyName">The name of the INI key.</param>
    /// <param name="value">The value of the INI key.</param>
    public void AddOrReplaceKey(string keyName, string value)
    {
        if (keyName == null || value == null)
            throw new ArgumentException("INI keys cannot have null key names or values.");

        int index = Keys.FindIndex(k => k.Key == keyName);
        if (index > -1)
            Keys[index] = new(keyName, value);
        else
            Keys.Add(new(keyName, value));
    }

    /// <summary>
    /// Removes a key from the INI section.
    /// Does not throw an exception if the key does not exist.
    /// </summary>
    /// <param name="keyName">The name of the INI key to remove.</param>
    public void RemoveKey(string keyName)
    {
        int index = Keys.FindIndex(k => k.Key == keyName);
        if (index > -1)
            Keys.RemoveAt(index);
    }

    /// <summary>
    /// Returns a string value from the INI section.
    /// </summary>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="defaultValue">The value to return if the section or key wasn't found.</param>
    /// <returns>The given key's value if the section and key was found. Otherwise the given defaultValue.</returns>
    public string GetStringValue(string key, string defaultValue)
    {
        KeyValuePair<string, string> kvp = Keys.Find(k => k.Key == key);

        return kvp.Value ?? defaultValue;
    }

    /// <summary>
    /// Returns an integer value from the INI section.
    /// </summary>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="defaultValue">The value to return if the section or key wasn't found,
    /// or converting the key's value to an integer failed.</param>
    /// <returns>The given key's value if the section and key was found and
    /// the value is a valid integer. Otherwise the given defaultValue.</returns>
    public int GetIntValue(string key, int defaultValue) => Conversions.IntFromString(GetStringValue(key, string.Empty), defaultValue);

    /// <summary>
    /// Returns a double-precision floating point value from the INI section.
    /// </summary>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="defaultValue">The value to return if the section or key wasn't found,
    /// or converting the key's value to a double failed.</param>
    /// <returns>The given key's value if the section and key was found and
    /// the value is a valid double. Otherwise the given defaultValue.</returns>
    public double GetDoubleValue(string key, double defaultValue) => Conversions.DoubleFromString(GetStringValue(key, string.Empty), defaultValue);

    /// <summary>
    /// Returns a single-precision floating point value from the INI section.
    /// </summary>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="defaultValue">The value to return if the section or key wasn't found,
    /// or converting the key's value to a float failed.</param>
    /// <returns>The given key's value if the section and key was found and
    /// the value is a valid float. Otherwise the given defaultValue.</returns>
    public float GetSingleValue(string key, float defaultValue) => Conversions.FloatFromString(GetStringValue(key, string.Empty), defaultValue);

    /// <summary>
    /// Sets the string value of a key in the INI section.
    /// If the key doesn't exist, it is created.
    /// </summary>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="value">The value of the INI key.</param>
    public void SetStringValue(string key, string value) => AddOrReplaceKey(key, value);

    /// <summary>
    /// Sets the integer value of a key in the INI section.
    /// If the key doesn't exist, it is created.
    /// </summary>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="value">The value of the INI key.</param>
    public void SetIntValue(string key, int value) => AddOrReplaceKey(key, value.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Sets the double-precision floating point value of a key in the INI section.
    /// If the key doesn't exist, it is created.
    /// </summary>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="value">The value of the INI key.</param>
    public void SetDoubleValue(string key, double value) => AddOrReplaceKey(key, value.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Sets the single-precision floating point value of a key in the INI section.
    /// If the key doesn't exist, it is created.
    /// </summary>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="value">The value of the INI key.</param>
    public void SetFloatValue(string key, float value) => AddOrReplaceKey(key, value.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Sets the boolean value of a key in the INI section.
    /// If the key doesn't exist, it is created.
    /// Uses the <see cref="BooleanStringStyle.TRUEFALSE"/> boolean string style.
    /// </summary>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="value">The value of the INI key.</param>
    public void SetBooleanValue(string key, bool value) => SetBooleanValue(key, value, BooleanStringStyle.TRUEFALSE);

    /// <summary>
    /// Sets the boolean value of a key in the INI section.
    /// If the key doesn't exist, it is created.
    /// </summary>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="value">The value of the INI key.</param>
    /// <param name="booleanStringStyle">The boolean string style.</param>
    public void SetBooleanValue(string key, bool value, BooleanStringStyle booleanStringStyle)
    {
        string strValue = Conversions.BooleanToString(value, booleanStringStyle);
        AddOrReplaceKey(key, strValue);
    }

    /// <summary>
    /// Returns a boolean value from the INI section.
    /// </summary>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="defaultValue">The value to return if the section or key wasn't found,
    /// or converting the key's value to a boolean failed.</param>
    /// <returns>The given key's value if the section and key was found and
    /// the value is a valid boolean. Otherwise the given defaultValue.</returns>
    public bool GetBooleanValue(string key, bool defaultValue) => Conversions.BooleanFromString(GetStringValue(key, string.Empty), defaultValue);

    /// <summary>
    /// Sets the list value of a key in the INI section.
    /// The list elements are converted to strings using the list element's
    /// ToString method and the given separator is applied between the elements.
    /// </summary>
    /// <typeparam name="T">The type of the list elements.</typeparam>
    /// <param name="key">The INI key.</param>
    /// <param name="list">The list.</param>
    /// <param name="separator">The separator between list elements.</param>
    public void SetListValue<T>(string key, List<T> list, char separator) => AddOrReplaceKey(key, string.Join(separator.ToString(), list));

    /// <summary>
    /// Parses and returns a list value of a key in the INI section.
    /// </summary>
    /// <typeparam name="T">The type of the list elements.</typeparam>
    /// <param name="key">The INI key.</param>
    /// <param name="separator">The separator between the list elements.</param>
    /// <param name="converter">The function that converts the list elements from strings to the given type.</param>
    /// <returns>A list that contains the parsed elements.</returns>
    public List<T> GetListValue<T>(string key, char separator, Func<string, T> converter)
    {
        var list = new List<T>();
        string value = GetStringValue(key, string.Empty);
        string[] parts = value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string part in parts)
            list.Add(converter(part));

        return list;
    }

    /// <summary>
    /// Parses and returns a path string from the INI section.
    /// The path string has all of its directory separators ( / \ )
    /// replaced with an environment-specific one.
    /// </summary>
    public string GetPathStringValue(string key, string defaultValue) => GetStringValue(key, defaultValue).Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

    /// <summary>
    /// Checks if the specified INI key exists in this section.
    /// </summary>
    /// <param name="key">The INI key.</param>
    /// <returns>True if the key exists in this section, otherwise false.</returns>
    public bool KeyExists(string key) => Keys.FindIndex(k => k.Key == key) > -1;
}