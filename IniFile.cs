// Rampastring's INI parser
// http://www.moddb.com/members/rampastring

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;

namespace Rampastring.Tools;

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
    /// Gets or sets a value that determines whether reading from or writing to a file
    /// named "desktop.ini" (in any directory) is forbidden. When set to <see langword="true"/>,
    /// any such attempt throws an <see cref="InvalidOperationException"/>.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public static bool DisallowDesktopIni { get; set; } = false;

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
        FilePath = filePath;

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
        FilePath = filePath;
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

    /// <summary>
    /// The path to the INI file on the file system, if any.
    /// </summary>
    public string FilePath { get; set; }

    [Obsolete("Use FilePath instead.")]
    public string FileName
    {
        get => FilePath;
        set => FilePath = value;
    }

    public Encoding Encoding { get; set; } = new UTF8Encoding(false);

    public bool AllowNewSections { get; set; } = true;

    /// <summary>
    /// Comment line to write to the INI file when it's written.
    /// </summary>
    public string Comment { get; set; }

    protected List<IniSection> Sections = new List<IniSection>();
    private int _lastSectionIndex = 0;

    public void Parse(bool applyBaseIni = true)
    {
        ThrowIfDesktopIniDisallowed(FileName);

        FileInfo fileInfo = SafePath.GetFile(FileName);

        if (!fileInfo.Exists)
            return;

        using FileStream stream = fileInfo.OpenRead();

        ParseIniFile(stream, null, applyBaseIni);
    }

    public void Reload()
    {
        _lastSectionIndex = 0;
        Sections.Clear();

        Parse();
    }

    private static void ThrowIfDesktopIniDisallowed(string filePath)
    {
        if (DisallowDesktopIni &&
            string.Equals(Path.GetFileName(filePath), "desktop.ini", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Access to desktop.ini files is not allowed. Path: {filePath}");
        }
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
            stringBuilder.Remove(stringBuilder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
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

    public void WriteIniFile()
    {
        WriteIniFile(FileName);
    }

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

    public void WriteIniFile(string filePath)
    {
        ThrowIfDesktopIniDisallowed(filePath);

        FileInfo fileInfo = SafePath.GetFile(filePath);

        if (fileInfo.Exists)
            fileInfo.Delete();

        using var stream = fileInfo.OpenWrite();

        WriteIniStream(stream);
    }

    public void AddSection(string sectionName)
    {
        Sections.Add(new IniSection(sectionName));
    }

    public void AddSection(IniSection section)
    {
        Sections.Add(section);
    }

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

    public void EraseSectionKeys(string sectionName)
    {
        int index = Sections.FindIndex(s => s.SectionName == sectionName);

        if (index == -1)
            return;

        Sections[index].Keys.Clear();
    }

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

    public int GetIntValue(string section, string key, int defaultValue)
    {
        return Conversions.IntFromString(GetStringValue(section, key, null), defaultValue);
    }

    public double GetDoubleValue(string section, string key, double defaultValue)
    {
        return Conversions.DoubleFromString(GetStringValue(section, key, String.Empty), defaultValue);
    }

    public float GetSingleValue(string section, string key, float defaultValue)
    {
        return Conversions.FloatFromString(GetStringValue(section, key, String.Empty), defaultValue);
    }

    public bool GetBooleanValue(string section, string key, bool defaultValue)
    {
        return Conversions.BooleanFromString(GetStringValue(section, key, String.Empty), defaultValue);
    }

    public List<T> GetListValue<T>(string sectionName, string key, char separator, Func<string, T> converter)
    {
        var section = GetSection(sectionName);
        if (section == null)
            return new List<T>();

        return section.GetListValue(key, separator, converter);
    }

    public string GetPathStringValue(string section, string key, string defaultValue)
    {
        IniSection iniSection = GetSection(section);
        if (iniSection == null)
            return defaultValue;

        return iniSection.GetPathStringValue(key, defaultValue);
    }

    public T GetEnumValue<T>(string section, string key, T defaultValue) where T : struct, Enum
    {
        IniSection iniSection = GetSection(section);
        if (iniSection == null)
            return defaultValue;

        return iniSection.GetEnumValue(key, defaultValue);
    }

    public void SetEnumValue<T>(string section, string key, T value) where T : struct, Enum
    {
        SetStringValue(section, key, value.ToString());
    }

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

    public List<string> GetSectionKeys(string sectionName)
    {
        IniSection section = Sections.Find(c => c.SectionName == sectionName);

        if (section == null)
            return null;

        List<string> returnValue = new List<string>();

        section.Keys.ForEach(kvp => returnValue.Add(kvp.Key));

        return returnValue;
    }

    public List<string> GetSections()
    {
        List<string> sectionList = new List<string>();

        Sections.ForEach(section => sectionList.Add(section.SectionName));

        return sectionList;
    }

    public bool SectionExists(string sectionName)
    {
        return Sections.FindIndex(c => c.SectionName == sectionName) != -1;
    }

    public bool KeyExists(string sectionName, string keyName)
    {
        IniSection section = GetSection(sectionName);
        if (section == null)
            return false;

        return section.KeyExists(keyName);
    }

    public void RemoveKey(string sectionName, string key)
    {
        var section = GetSection(sectionName);

        if (section != null)
            section.RemoveKey(key);
    }
}
