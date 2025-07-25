﻿// Rampastring's INI parser
// http://www.moddb.com/members/rampastring

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;

namespace Rampastring.Tools;

/// <summary>
/// A class for parsing, handling and writing INI files.
/// </summary>
public class IniFile : IIniFile
{
    private const string TextBlockBeginIdentifier = "$$$TextBlockBegin$$$";
    private const string TextBlockEndIdentifier = "$$$TextBlockEnd$$$";

    #region Static methods

    /// <summary>
    /// Consolidates two INI files, adding all of the second INI file's contents
    /// to the first INI file. In case conflicting keys are found, the second
    /// INI file takes priority.
    /// </summary>
    /// <param name="firstIni">The first INI file.</param>
    /// <param name="secondIni">The second INI file.</param>
    public static void ConsolidateIniFiles(IniFile firstIni, IniFile secondIni)
    {
        List<string> sections = secondIni.GetSections();

        foreach (string section in sections)
        {
            List<string> sectionKeys = secondIni.GetSectionKeys(section);
            foreach (string key in sectionKeys)
            {
                firstIni.SetStringValue(section, key, secondIni.GetStringValue(section, key, String.Empty));
            }
        }
    }

    #endregion

    /// <summary>
    /// Creates a new INI file instance.
    /// </summary>
    public IniFile() { }

    /// <summary>
    /// Creates a new INI file instance and parses it.
    /// </summary>
    /// <param name="filePath">The path of the INI file.</param>
    /// <param name="applyBaseIni">Whether to parse potential INI file that the file is based on.</param>
    public IniFile(string filePath, bool applyBaseIni = true)
    {
        FileName = filePath;

        Parse(applyBaseIni);
    }

    /// <summary>
    /// Creates a new INI file instance and parses it.
    /// </summary>
    /// <param name="filePath">The path of the INI file.</param>
    /// <param name="encoding">The encoding of the INI file. Default for UTF-8.</param>
    /// <param name="applyBaseIni">Whether to parse potential INI file that the file is based on.</param>
    public IniFile(string filePath, Encoding encoding, bool applyBaseIni = true)
    {
        FileName = filePath;
        Encoding = encoding;

        Parse(applyBaseIni);
    }

    /// <summary>
    /// Creates a new INI file instance and parses it.
    /// </summary>
    /// <param name="stream">The stream to read the INI file from.</param>
    /// <param name="applyBaseIni">Whether to parse potential INI file that the file is based on.</param>
    public IniFile(Stream stream, bool applyBaseIni = true)
    {
        ParseIniFile(stream, null, applyBaseIni);
    }

    /// <summary>
    /// Creates a new INI file instance and parses it.
    /// </summary>
    /// <param name="stream">The stream to read the INI file from.</param>
    /// <param name="encoding">The encoding of the INI file. Default for UTF-8.</param>
    /// <param name="applyBaseIni">Whether to parse potential INI file that the file is based on.</param>
    public IniFile(Stream stream, Encoding encoding, bool applyBaseIni = true)
    {
        Encoding = encoding;

        ParseIniFile(stream, encoding, applyBaseIni);
    }

    public string FileName { get; set; }
    public Encoding Encoding { get; set; } = new UTF8Encoding(false);

    /// <summary>
    /// Gets or sets a value that determines whether the parser should only parse 
    /// pre-determined (via <see cref="AddSection(string)"/>) sections or all sections in the INI file.
    /// </summary>
    public bool AllowNewSections { get; set; } = true;

    /// <summary>
    /// Comment line to write to the INI file when it's written.
    /// </summary>
    public string Comment { get; set; }

    protected List<IniSection> Sections = new List<IniSection>();
    private int _lastSectionIndex = 0;

    public void Parse(bool applyBaseIni = true)
    {
        FileInfo fileInfo = SafePath.GetFile(FileName);

        if (!fileInfo.Exists)
            return;

        using FileStream stream = fileInfo.OpenRead();

        ParseIniFile(stream, null, applyBaseIni);
    }

    /// <summary>
    /// Clears all data from this IniFile instance and then re-parses the input INI file.
    /// </summary>
    public void Reload()
    {
        _lastSectionIndex = 0;
        Sections.Clear();

        Parse();
    }

    private void ParseIniFile(Stream stream, Encoding encoding = null, bool applyBaseIni = true)
    {
        if (encoding == null)
            encoding = Encoding;

        using var reader = new StreamReader(stream, encoding);

        int currentSectionId = -1;
        string currentLine = string.Empty;

        while (!reader.EndOfStream)
        {
            currentLine = reader.ReadLine();

            int commentStartIndex = currentLine.IndexOf(';');
            if (commentStartIndex > -1)
                currentLine = currentLine.Substring(0, commentStartIndex);

            if (string.IsNullOrWhiteSpace(currentLine))
                continue;

            if (currentLine[0] == '[')
            {
                int sectionNameEndIndex = currentLine.IndexOf(']');
                if (sectionNameEndIndex == -1)
                    throw new IniParseException("Invalid INI section definition: " + currentLine);

                string sectionName = currentLine.Substring(1, sectionNameEndIndex - 1);
                int index = Sections.FindIndex(c => c.SectionName == sectionName);

                if (index > -1)
                {
                    currentSectionId = index;
                }
                else if (AllowNewSections)
                {
                    Sections.Add(new IniSection(sectionName));
                    currentSectionId = Sections.Count - 1;
                }
                else
                    currentSectionId = -1;

                continue;
            }

            if (currentSectionId == -1)
                continue;

            int equalsIndex = currentLine.IndexOf('=');

            if (equalsIndex == -1)
            {
                Sections[currentSectionId].AddOrReplaceKey(currentLine.Trim(), string.Empty);
            }
            else
            {
                string value = currentLine.Substring(equalsIndex + 1).Trim();

                if (value == TextBlockBeginIdentifier)
                {
                    value = ReadTextBlock(reader);
                }

                Sections[currentSectionId].AddOrReplaceKey(currentLine.Substring(0, equalsIndex).Trim(),
                    value);
            }
        }

        if (applyBaseIni)
            ApplyBaseIni();
    }

    private string ReadTextBlock(StreamReader reader)
    {
        StringBuilder stringBuilder = new StringBuilder();

        while (true)
        {
            if (reader.EndOfStream)
            {
                throw new IniParseException("Encountered end-of-file while " +
                    "reading text block. Text block is not terminated properly.");
            }

            string line = reader.ReadLine().Trim();

            if (line.Trim() == TextBlockEndIdentifier)
                break;

            stringBuilder.Append(line + Environment.NewLine);
        }

        if (stringBuilder.Length > 0)
        {
            stringBuilder.Remove(stringBuilder.Length - Environment.NewLine.Length,
                Environment.NewLine.Length);
        }

        return stringBuilder.ToString();
    }

    protected virtual void ApplyBaseIni()
    {
        string basedOn = GetStringValue("INISystem", "BasedOn", String.Empty);
        if (!String.IsNullOrEmpty(basedOn))
        {
            // Consolidate with the INI file that this INI file is based on
            string path = SafePath.CombineFilePath(SafePath.GetFileDirectoryName(FileName), basedOn);
            IniFile baseIni = new IniFile(path);
            ConsolidateIniFiles(baseIni, this);
            Sections = baseIni.Sections;
        }
    }

    /// <summary>
    /// Writes the INI file to the path that was
    /// given to the instance on creation.
    /// </summary>
    public void WriteIniFile()
    {
        WriteIniFile(FileName);
    }

    /// <summary>
    /// Writes the INI file to a specified stream.
    /// </summary>
    /// <param name="stream">The stream to write the INI file to.</param>
    public void WriteIniStream(Stream stream)
    {
        WriteIniStream(stream, Encoding);
    }

    /// <summary>
    /// Writes the INI file to a specified stream.
    /// </summary>
    /// <param name="stream">The stream to read the INI file from.</param>
    /// <param name="encoding">The encoding of the INI file. Default for UTF-8.</param>
    public void WriteIniStream(Stream stream, Encoding encoding)
    {
        using StreamWriter sw = new StreamWriter(stream, encoding);

        if (!string.IsNullOrWhiteSpace(Comment))
        {
            sw.WriteLine("; " + Comment);
            sw.WriteLine();
        }

        foreach (IniSection section in Sections)
        {
            sw.Write("[" + section.SectionName + "]\r\n");
            foreach (var kvp in section.Keys)
            {
                sw.Write(kvp.Key + "=" + kvp.Value + "\r\n");
            }
            sw.Write("\r\n");
        }

        sw.Write("\r\n");
    }

    /// <summary>
    /// Writes the INI file's contents to the specified path.
    /// </summary>
    /// <param name="filePath">The path of the file to write to.</param>
    public void WriteIniFile(string filePath)
    {
        FileInfo fileInfo = SafePath.GetFile(filePath);

        if (fileInfo.Exists)
            fileInfo.Delete();

        using var stream = fileInfo.OpenWrite();

        WriteIniStream(stream);
    }

    /// <summary>
    /// Creates and adds a section into the INI file.
    /// </summary>
    /// <param name="sectionName">The name of the section to add.</param>
    public void AddSection(string sectionName)
    {
        Sections.Add(new IniSection(sectionName));
    }

    /// <summary>
    /// Adds a section into the INI file.
    /// </summary>
    /// <param name="section">The section to add.</param>
    public void AddSection(IniSection section)
    {
        Sections.Add(section);
    }

    /// <summary>
    /// Removes the given section from the INI file.
    /// Uses case-insensitive string comparison when looking for the section.
    /// </summary>
    /// <param name="sectionName">The name of the section to remove.</param>
    public void RemoveSection(string sectionName)
    {
        int index = Sections.FindIndex(section =>
                section.SectionName.Equals(sectionName,
                StringComparison.InvariantCultureIgnoreCase));

        if (index > -1)
            Sections.RemoveAt(index);
    }

    /// <summary>
    /// Moves a section's position to the first place in the INI file's section list.
    /// </summary>
    /// <param name="sectionName">The name of the INI section to move.</param>
    public void MoveSectionToFirst(string sectionName)
    {
        int index = Sections.FindIndex(s => s.SectionName == sectionName);

        if (index == -1)
            return;

        IniSection section = Sections[index];

        Sections.RemoveAt(index);
        Sections.Insert(0, section);
    }

    /// <summary>
    /// Erases all existing keys of a section.
    /// Does nothing if the section does not exist.
    /// </summary>
    /// <param name="sectionName">The name of the section.</param>
    public void EraseSectionKeys(string sectionName)
    {
        int index = Sections.FindIndex(s => s.SectionName == sectionName);

        if (index == -1)
            return;

        Sections[index].Keys.Clear();
    }

    /// <summary>
    /// Combines two INI sections, with the second section overriding 
    /// in case conflicting keys are present. The combined section
    /// then over-writes the second section.
    /// </summary>
    /// <param name="firstSectionName">The name of the first INI section.</param>
    /// <param name="secondSectionName">The name of the second INI section.</param>
    public void CombineSections(string firstSectionName, string secondSectionName)
    {
        int firstIndex = Sections.FindIndex(s => s.SectionName == firstSectionName);

        if (firstIndex == -1)
            return;

        int secondIndex = Sections.FindIndex(s => s.SectionName == secondSectionName);

        if (secondIndex == -1)
            return;

        IniSection firstSection = Sections[firstIndex];
        IniSection secondSection = Sections[secondIndex];

        var newSection = new IniSection(secondSection.SectionName);

        foreach (var kvp in firstSection.Keys)
            newSection.Keys.Add(kvp);

        foreach (var kvp in secondSection.Keys)
        {
            int index = newSection.Keys.FindIndex(k => k.Key == kvp.Key);

            if (index > -1)
                newSection.Keys[index] = kvp;
            else
                newSection.Keys.Add(kvp);
        }

        Sections[secondIndex] = newSection;
    }

    /// <summary>
    /// Returns a string value from the INI file.
    /// </summary>
    /// <param name="section">The name of the key's section.</param>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="defaultValue">The value to return if the section or key wasn't found.</param>
    /// <returns>The given key's value if the section and key was found. Otherwise the given defaultValue.</returns>
    public string GetStringValue(string section, string key, string defaultValue)
    {
        IniSection iniSection = GetSection(section);
        if (iniSection == null)
            return defaultValue;

        return iniSection.GetStringValue(key, defaultValue);
    }

    public string GetStringValue(string section, string key, string defaultValue, out bool success)
    {
        int sectionId = Sections.FindIndex(c => c.SectionName == section);
        if (sectionId == -1)
        {
            success = false;
            return defaultValue;
        }

        var kvp = Sections[sectionId].Keys.Find(k => k.Key == key);

        if (kvp.Value == null)
        {
            success = false;
            return defaultValue;
        }
        else
        {
            success = true;
            return kvp.Value;
        }
    }

    /// <summary>
    /// Returns an integer value from the INI file.
    /// </summary>
    /// <param name="section">The name of the key's section.</param>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="defaultValue">The value to return if the section or key wasn't found,
    /// or converting the key's value to an integer failed.</param>
    /// <returns>The given key's value if the section and key was found and
    /// the value is a valid integer. Otherwise the given defaultValue.</returns>
    public int GetIntValue(string section, string key, int defaultValue)
    {
        return Conversions.IntFromString(GetStringValue(section, key, null), defaultValue);
    }

    /// <summary>
    /// Returns a double-precision floating point value from the INI file.
    /// </summary>
    /// <param name="section">The name of the key's section.</param>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="defaultValue">The value to return if the section or key wasn't found,
    /// or converting the key's value to a double failed.</param>
    /// <returns>The given key's value if the section and key was found and
    /// the value is a valid double. Otherwise the given defaultValue.</returns>
    public double GetDoubleValue(string section, string key, double defaultValue)
    {
        return Conversions.DoubleFromString(GetStringValue(section, key, String.Empty), defaultValue);
    }

    /// <summary>
    /// Returns a single-precision floating point value from the INI file.
    /// </summary>
    /// <param name="section">The name of the key's section.</param>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="defaultValue">The value to return if the section or key wasn't found,
    /// or converting the key's value to a float failed.</param>
    /// <returns>The given key's value if the section and key was found and
    /// the value is a valid float. Otherwise the given defaultValue.</returns>
    public float GetSingleValue(string section, string key, float defaultValue)
    {
        return Conversions.FloatFromString(GetStringValue(section, key, String.Empty), defaultValue);
    }

    /// <summary>
    /// Returns a boolean value from the INI file.
    /// </summary>
    /// <param name="section">The name of the key's section.</param>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="defaultValue">The value to return if the section or key wasn't found,
    /// or converting the key's value to a boolean failed.</param>
    /// <returns>The given key's value if the section and key was found and
    /// the value is a valid boolean. Otherwise the given defaultValue.</returns>
    public bool GetBooleanValue(string section, string key, bool defaultValue)
    {
        return Conversions.BooleanFromString(GetStringValue(section, key, String.Empty), defaultValue);
    }

    /// <summary>
    /// Parses and returns a list value in the INI file.
    /// </summary>
    /// <typeparam name="T">The type of the list elements.</typeparam>
    /// <param name="section">The name of the INI section.</param>
    /// <param name="key">The INI key.</param>
    /// <param name="separator">The separator between the list elements.</param>
    /// <param name="converter">The function that converts the list elements from strings to the given type.</param>
    /// <returns>A list that contains the parsed elements.</returns>
    public List<T> GetListValue<T>(string sectionName, string key, char separator, Func<string, T> converter)
    {
        var section = GetSection(sectionName);
        if (section == null)
            return new List<T>();

        return section.GetListValue(key, separator, converter);
    }

    /// <summary>
    /// Parses and returns a path string from the INI file.
    /// The path string has all of its directory separators ( / \ )
    /// replaced with an environment-specific one.
    /// </summary>
    public string GetPathStringValue(string section, string key, string defaultValue)
    {
        IniSection iniSection = GetSection(section);
        if (iniSection == null)
            return defaultValue;

        return iniSection.GetPathStringValue(key, defaultValue);
    }

    /// <summary>
    /// Returns an INI section from the file, or null if the section doesn't exist.
    /// </summary>
    /// <param name="name">The name of the section.</param>
    /// <returns>The section of the file; null if the section doesn't exist.</returns>
    public IniSection GetSection(string name)
    {
        for (int i = _lastSectionIndex; i < Sections.Count; i++)
        {
            if (Sections[i].SectionName == name)
            {
                _lastSectionIndex = i;
                return Sections[i];
            }
        }

        int sectionId = Sections.FindIndex(c => c.SectionName == name);
        if (sectionId == -1)
        {
            _lastSectionIndex = 0;
            return null;
        }

        _lastSectionIndex = sectionId;

        return Sections[sectionId];
    }

    /// <summary>
    /// Sets the string value of a specific key of a specific section in the INI file.
    /// </summary>
    /// <param name="section">The name of the key's section.</param>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="value">The value to set to the key.</param>
    public void SetStringValue(string section, string key, string value)
    {
        var iniSection = Sections.Find(s => s.SectionName == section);
        if (iniSection == null)
        {
            iniSection = new IniSection(section);
            Sections.Add(iniSection);
        }

        iniSection.SetStringValue(key, value);
    }

    /// <summary>
    /// Sets the integer value of a specific key of a specific section in the INI file.
    /// </summary>
    /// <param name="section">The name of the key's section.</param>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="value">The value to set to the key.</param>
    public void SetIntValue(string section, string key, int value)
    {
        var iniSection = Sections.Find(s => s.SectionName == section);
        if (iniSection == null)
        {
            iniSection = new IniSection(section);
            Sections.Add(iniSection);
        }

        iniSection.SetIntValue(key, value);
    }

    /// <summary>
    /// Sets the double value of a specific key of a specific section in the INI file.
    /// </summary>
    /// <param name="section">The name of the key's section.</param>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="value">The value to set to the key.</param>
    public void SetDoubleValue(string section, string key, double value)
    {
        var iniSection = Sections.Find(s => s.SectionName == section);
        if (iniSection == null)
        {
            iniSection = new IniSection(section);
            Sections.Add(iniSection);
        }

        iniSection.SetDoubleValue(key, value);
    }

    public void SetSingleValue(string section, string key, float value)
    {
        SetSingleValue(section, key, value, 0);
    }

    public void SetSingleValue(string section, string key, double value, int decimals)
    {
        SetSingleValue(section, key, Convert.ToSingle(value), decimals);
    }

    /// <summary>
    /// Sets the float value of a key in the INI file.
    /// </summary>
    /// <param name="section">The name of the key's section.</param>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="value">The value to set to the key.</param>
    /// <param name="decimals">Defines how many decimal places the stringified float should have.</param>
    public void SetSingleValue(string section, string key, float value, int decimals)
    {
        string stringValue = value.ToString("N" + decimals, CultureInfo.GetCultureInfo("en-US").NumberFormat);
        var iniSection = Sections.Find(s => s.SectionName == section);
        if (iniSection == null)
        {
            iniSection = new IniSection(section);
            Sections.Add(iniSection);
        }

        iniSection.SetStringValue(key, stringValue);
    }

    /// <summary>
    /// Sets the boolean value of a key in the INI file.
    /// </summary>
    /// <param name="section">The name of the key's section.</param>
    /// <param name="key">The name of the INI key.</param>
    /// <param name="value">The value to set to the key.</param>
    public void SetBooleanValue(string section, string key, bool value)
    {
        var iniSection = Sections.Find(s => s.SectionName == section);
        if (iniSection == null)
        {
            iniSection = new IniSection(section);
            Sections.Add(iniSection);
        }

        iniSection.SetBooleanValue(key, value);
    }

    /// <summary>
    /// Sets the list value of a key in the INI section.
    /// The list elements are converted to strings using the list element's
    /// ToString method and the given separator is applied between the elements.
    /// </summary>
    /// <typeparam name="T">The type of the list elements.</typeparam>
    /// <param name="section">The INI section.</param>
    /// <param name="key">The INI key.</param>
    /// <param name="list">The list.</param>
    /// <param name="separator">The separator between list elements.</param>
    public void SetListValue<T>(string section, string key, List<T> list, char separator)
    {
        var iniSection = Sections.Find(s => s.SectionName == section);
        if (iniSection == null)
        {
            iniSection = new IniSection(section);
            Sections.Add(iniSection);
        }

        iniSection.SetListValue(key, list, separator);
    }

    /// <summary>
    /// Gets the names of all INI keys in the specified INI section.
    /// </summary>
    public List<string> GetSectionKeys(string sectionName)
    {
        IniSection section = Sections.Find(c => c.SectionName == sectionName);

        if (section == null)
            return null;

        List<string> returnValue = new List<string>();

        section.Keys.ForEach(kvp => returnValue.Add(kvp.Key));

        return returnValue;
    }

    /// <summary>
    /// Gets the names of all sections in the INI file.
    /// </summary>
    public List<string> GetSections()
    {
        List<string> sectionList = new List<string>();

        Sections.ForEach(section => sectionList.Add(section.SectionName));

        return sectionList;
    }

    /// <summary>
    /// Checks whether a section exists. Returns true if the section
    /// exists, otherwise returns false.
    /// </summary>
    /// <param name="sectionName">The name of the INI section.</param>
    /// <returns></returns>
    public bool SectionExists(string sectionName)
    {
        return Sections.FindIndex(c => c.SectionName == sectionName) != -1;
    }

    /// <summary>
    /// Checks whether a specific INI key exists in a specific INI section.
    /// </summary>
    /// <param name="sectionName">The name of the INI section.</param>
    /// <param name="keyName">The name of the INI key.</param>
    /// <returns>True if the key exists, otherwise false.</returns>
    public bool KeyExists(string sectionName, string keyName)
    {
        IniSection section = GetSection(sectionName);
        if (section == null)
            return false;

        return section.KeyExists(keyName);
    }

    /// <summary>
    /// Removes a key from the given section in the INI file.
    /// </summary>
    /// <param name="sectionName">The name of the section to remove the key from.</param>
    /// <param name="key">The key to remove from the section.</param>
    public void RemoveKey(string sectionName, string key)
    {
        var section = GetSection(sectionName);

        if (section != null)
            section.RemoveKey(key);
    }
}
