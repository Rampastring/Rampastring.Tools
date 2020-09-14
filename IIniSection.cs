using System;
using System.Collections.Generic;

namespace Rampastring.Tools
{
    public interface IIniSection
    {
        /// <summary>
        /// Returns the name of the INI section.
        /// </summary>
        string SectionName { get; }

        /// <summary>
        /// Adds a key to the INI section.
        /// Throws a <see cref="InvalidOperationException"/> if the key already exists.
        /// Use <see cref="AddOrReplaceKey(string, string)"/> if you want to replace
        /// an existing key instead.
        /// </summary>
        /// <param name="keyName">The name of the INI key.</param>
        /// <param name="value">The value of the INI key.</param>
        void AddKey(string keyName, string value);

        /// <summary>
        /// Adds a key to the INI section, or replaces the key's value if the key
        /// already exists.
        /// </summary>
        /// <param name="keyName">The name of the INI key.</param>
        /// <param name="value">The value of the INI key.</param>
        void AddOrReplaceKey(string keyName, string value);

        /// <summary>
        /// Removes a key from the INI section.
        /// Does not throw an exception if the key does not exist.
        /// </summary>
        /// <param name="keyName">The name of the INI key to remove.</param>
        void RemoveKey(string keyName);

        /// <summary>
        /// Returns a boolean value from the INI section.
        /// </summary>
        /// <param name="key">The name of the INI key.</param>
        /// <param name="defaultValue">The value to return if the section or key wasn't found,
        /// or converting the key's value to a boolean failed.</param>
        /// <returns>The given key's value if the section and key was found and
        /// the value is a valid boolean. Otherwise the given defaultValue.</returns>
        bool GetBooleanValue(string key, bool defaultValue);

        /// <summary>
        /// Returns a double-precision floating point value from the INI section.
        /// </summary>
        /// <param name="key">The name of the INI key.</param>
        /// <param name="defaultValue">The value to return if the section or key wasn't found,
        /// or converting the key's value to a double failed.</param>
        /// <returns>The given key's value if the section and key was found and
        /// the value is a valid double. Otherwise the given defaultValue.</returns>
        double GetDoubleValue(string key, double defaultValue);

        /// <summary>
        /// Returns an integer value from the INI section.
        /// </summary>
        /// <param name="key">The name of the INI key.</param>
        /// <param name="defaultValue">The value to return if the section or key wasn't found,
        /// or converting the key's value to an integer failed.</param>
        /// <returns>The given key's value if the section and key was found and
        /// the value is a valid integer. Otherwise the given defaultValue.</returns>
        int GetIntValue(string key, int defaultValue);

        /// <summary>
        /// Parses and returns a list value of a key in the INI section.
        /// </summary>
        /// <typeparam name="T">The type of the list elements.</typeparam>
        /// <param name="key">The INI key.</param>
        /// <param name="separator">The separator between the list elements.</param>
        /// <param name="converter">The function that converts the list elements from strings to the given type.</param>
        /// <returns>A list that contains the parsed elements.</returns>
        List<T> GetListValue<T>(string key, char separator, Func<string, T> converter);

        /// <summary>
        /// Returns a single-precision floating point value from the INI section.
        /// </summary>
        /// <param name="key">The name of the INI key.</param>
        /// <param name="defaultValue">The value to return if the section or key wasn't found,
        /// or converting the key's value to a float failed.</param>
        /// <returns>The given key's value if the section and key was found and
        /// the value is a valid float. Otherwise the given defaultValue.</returns>
        float GetSingleValue(string key, float defaultValue);

        /// <summary>
        /// Returns a string value from the INI section.
        /// </summary>
        /// <param name="key">The name of the INI key.</param>
        /// <param name="defaultValue">The value to return if the section or key wasn't found.</param>
        /// <returns>The given key's value if the section and key was found. Otherwise the given defaultValue.</returns>
        string GetStringValue(string key, string defaultValue);

        /// <summary>
        /// Checks if the specified INI key exists in this section.
        /// </summary>
        /// <param name="key">The INI key.</param>
        /// <returns>True if the key exists in this section, otherwise false.</returns>
        bool KeyExists(string key);

        /// <summary>
        /// Sets the boolean value of a key in the INI section.
        /// If the key doesn't exist, it is created.
        /// </summary>
        /// <param name="key">The name of the INI key.</param>
        /// <param name="value">The value of the INI key.</param>
        void SetBooleanValue(string key, bool value);

        /// <summary>
        /// Sets the double-precision floating point value of a key in the INI section.
        /// If the key doesn't exist, it is created.
        /// </summary>
        /// <param name="key">The name of the INI key.</param>
        /// <param name="value">The value of the INI key.</param>
        void SetDoubleValue(string key, double value);

        /// <summary>
        /// Sets the single-precision floating point value of a key in the INI section.
        /// If the key doesn't exist, it is created.
        /// </summary>
        /// <param name="key">The name of the INI key.</param>
        /// <param name="value">The value of the INI key.</param>
        void SetFloatValue(string key, float value);

        /// <summary>
        /// Sets the integer value of a key in the INI section.
        /// If the key doesn't exist, it is created.
        /// </summary>
        /// <param name="key">The name of the INI key.</param>
        /// <param name="value">The value of the INI key.</param>
        void SetIntValue(string key, int value);

        /// <summary>
        /// Sets the list value of a key in the INI section.
        /// The list elements are converted to strings using the list element's
        /// ToString method and the given separator is applied between the elements.
        /// </summary>
        /// <typeparam name="T">The type of the list elements.</typeparam>
        /// <param name="key">The INI key.</param>
        /// <param name="list">The list.</param>
        /// <param name="separator">The separator between list elements.</param>
        void SetListValue<T>(string key, List<T> list, char separator);

        /// <summary>
        /// Sets the string value of a key in the INI section.
        /// If the key doesn't exist, it is created.
        /// </summary>
        /// <param name="key">The name of the INI key.</param>
        /// <param name="value">The value of the INI key.</param>
        void SetStringValue(string key, string value);
    }
}