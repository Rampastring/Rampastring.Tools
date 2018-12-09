using System.Collections.Generic;

namespace Rampastring.Tools
{
    public interface IIniFile
    {
        // TODO take documentation comments from IniFile

        bool AllowNewSections { get; set; }
        string FileName { get; set; }

        void AddSection(IniSection section);
        void AddSection(string sectionName);
        void CombineSections(string firstSectionName, string secondSectionName);
        void EraseSectionKeys(string sectionName);
        bool GetBooleanValue(string section, string key, bool defaultValue);
        double GetDoubleValue(string section, string key, double defaultValue);
        int GetIntValue(string section, string key, int defaultValue);
        IniSection GetSection(string name);
        List<string> GetSectionKeys(string sectionName);
        List<string> GetSections();
        float GetSingleValue(string section, string key, float defaultValue);
        string GetStringValue(string section, string key, string defaultValue);
        string GetStringValue(string section, string key, string defaultValue, out bool success);
        bool KeyExists(string sectionName, string keyName);
        void Parse();
        void Reload();
        bool SectionExists(string sectionName);
        void SetBooleanValue(string section, string key, bool value);
        void SetDoubleValue(string section, string key, double value);
        void SetIntValue(string section, string key, int value);
        void SetSingleValue(string section, string key, double value, int decimals);
        void SetSingleValue(string section, string key, float value);
        void SetSingleValue(string section, string key, float value, int decimals);
        void SetStringValue(string section, string key, string value);
        void WriteIniFile();
        void WriteIniFile(string filePath);
    }
}