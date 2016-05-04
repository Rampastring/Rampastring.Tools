// Rampastring's INI parser
// http://www.moddb.com/members/rampastring

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Linq;

namespace Rampastring.Tools
{
    /// <summary>
    /// A class for parsing, handling and writing INI files.
    /// </summary>
    public class IniFile
    {
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

        public IniFile() { }

        public IniFile(string filePath)
        {
            FileName = filePath;

            if (File.Exists(filePath))
            {
                ParseIniFile(File.OpenRead(filePath));
            }
        }

        public IniFile(Stream stream)
        {
            ParseIniFile(stream);
        }

        public string FileName { get; set; }
        List<IniSection> Sections = new List<IniSection>();
        int _lastSectionIndex = 0;

        private void ParseIniFile(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);

            int currentSectionId = -1;
            string currentLine = String.Empty;

            while (!reader.EndOfStream)
            {
                currentLine = reader.ReadLine();

                int commentStartIndex = currentLine.IndexOf(';');
                if (commentStartIndex > -1)
                    currentLine = currentLine.Substring(0, commentStartIndex);

                if (String.IsNullOrEmpty(currentLine))
                    continue;

                if (currentLine[0] == '[')
                {
                    string sectionName = currentLine.Substring(1, currentLine.IndexOf(']') - 1);
                    int index = Sections.FindIndex(c => c.SectionName == sectionName);

                    if (index > -1)
                    {
                        currentSectionId = index;
                    }
                    else
                    {
                        Sections.Add(new IniSection(sectionName));
                        currentSectionId = Sections.Count - 1;
                    }

                    continue;
                }

                if (currentSectionId == -1)
                    continue;

                int equalsIndex = currentLine.IndexOf('=');

                if (equalsIndex == -1)
                {
                    Sections[currentSectionId].AddKey(currentLine.Trim(), String.Empty);
                }
                else
                {
                    Sections[currentSectionId].AddKey(currentLine.Substring(0, equalsIndex).Trim(),
                        currentLine.Substring(equalsIndex + 1).Trim());
                }
            }

            reader.Close();

            string basedOn = GetStringValue("INISystem", "BasedOn", String.Empty);
            if (!String.IsNullOrEmpty(basedOn))
            {
                // Consolidate with the INI file that this INI file is based on
                string path = Path.GetDirectoryName(FileName) + "\\" + basedOn;
                IniFile baseIni = new IniFile(path);
                ConsolidateIniFiles(baseIni, this);
                this.Sections = baseIni.Sections;
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
        /// Writes the INI file's contents to the specified path.
        /// </summary>
        /// <param name="filePath">The path of the file to write to.</param>
        public void WriteIniFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            StreamWriter sw = new StreamWriter(File.OpenWrite(filePath));
            foreach (IniSection section in Sections)
            {
                sw.WriteLine("[" + section.SectionName + "]");
                foreach (string[] key in section.Keys)
                {
                    sw.WriteLine(key[0] + "=" + key[1]);
                }
                sw.WriteLine();
            }

            sw.WriteLine();
            sw.Close();
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

            IniSection newSection = new IniSection(secondSection.SectionName);

            foreach (string[] keyAndValue in firstSection.Keys)
                newSection.Keys.Add(keyAndValue);

            foreach (string[] keyAndValue in secondSection.Keys)
            {
                int index = newSection.Keys.FindIndex(k => k[0] == keyAndValue[0]);

                if (index > -1)
                    newSection.Keys[index] = keyAndValue;
                else
                    newSection.Keys.Add(keyAndValue);
            }

            Sections[secondIndex] = newSection;
        }

        /// <summary>
        /// Returns a string value from the INI file.
        /// </summary>
        /// <param name="section">The section of the key.</param>
        /// <param name="key">The INI key.</param>
        /// <param name="defaultValue">The value to return if the section or key wasn't found.</param>
        /// <returns>The given key's value if the section and key was found. Otherwise the given defaultValue.</returns>
        public string GetStringValue(string section, string key, string defaultValue)
        {
            IniSection iniSection = GetSection(section);
            if (iniSection == null)
                return defaultValue;

            string[] keyAndValue = iniSection.Keys.Find(str => str != null && str[0] == key);

            if (keyAndValue == null)
                return defaultValue;

            return keyAndValue[1];
        }

        public string GetStringValue(string section, string key, string defaultValue, out bool success)
        {
            int sectionId = Sections.FindIndex(c => c.SectionName == section);
            if (sectionId == -1)
            {
                success = false;
                return defaultValue;
            }

            string[] keyArray = Sections[sectionId].Keys.Find(c => c[0] == key);

            if (keyArray == null)
            {
                success = false;
                return defaultValue;
            }
            else
            {
                success = true;
                return keyArray[1];
            }
        }

        /// <summary>
        /// Returns an integer value from the INI file.
        /// </summary>
        /// <param name="section">The section of the key.</param>
        /// <param name="key">The INI key.</param>
        /// <param name="defaultValue">The value to return if the section or key wasn't found,
        /// or converting the key's value to an integer failed.</param>
        /// <returns>The given key's value if the section and key was found and
        /// the value is a valid integer. Otherwise the given defaultValue.</returns>
        public int GetIntValue(string section, string key, int defaultValue)
        {
            return Utilities.IntFromString(GetStringValue(section, key, String.Empty), defaultValue);
        }

        public int GetIntValue(string section, string key, int defaultValue, out bool success)
        {
            int sectionId = Sections.FindIndex(c => c.SectionName == section);
            if (sectionId == -1)
            {
                success = false;
                return defaultValue;
            }

            string[] keyArray = Sections[sectionId].Keys.Find(c => c[0] == key);

            if (keyArray == null)
            {
                success = false;
                return defaultValue;
            }
            else
            {
                try
                {
                    success = true;
                    return Convert.ToInt32(keyArray[1]);
                }
                catch
                {
                    success = false;
                    return defaultValue;
                }
            }
        }

        /// <summary>
        /// Returns a double-precision floating point value from the INI file.
        /// </summary>
        /// <param name="section">The section of the key.</param>
        /// <param name="key">The INI key.</param>
        /// <param name="defaultValue">The value to return if the section or key wasn't found,
        /// or converting the key's value to a double failed.</param>
        /// <returns>The given key's value if the section and key was found and
        /// the value is a valid double. Otherwise the given defaultValue.</returns>
        public double GetDoubleValue(string section, string key, double defaultValue)
        {
            return Utilities.DoubleFromString(GetStringValue(section, key, String.Empty), defaultValue);
        }

        /// <summary>
        /// Returns a single-precision floating point value from the INI file.
        /// </summary>
        /// <param name="section">The section of the key.</param>
        /// <param name="key">The INI key.</param>
        /// <param name="defaultValue">The value to return if the section or key wasn't found,
        /// or converting the key's value to a float failed.</param>
        /// <returns>The given key's value if the section and key was found and
        /// the value is a valid float. Otherwise the given defaultValue.</returns>
        public float GetSingleValue(string section, string key, float defaultValue)
        {
            return Utilities.FloatFromString(GetStringValue(section, key, String.Empty), defaultValue);
        }

        /// <summary>
        /// Returns a boolean value from the INI file.
        /// </summary>
        /// <param name="section">The section of the key.</param>
        /// <param name="key">The INI key.</param>
        /// <param name="defaultValue">The value to return if the section or key wasn't found,
        /// or converting the key's value to a boolean failed.</param>
        /// <returns>The given key's value if the section and key was found and
        /// the value is a valid boolean. Otherwise the given defaultValue.</returns>
        public bool GetBooleanValue(string section, string key, bool defaultValue)
        {
            return Utilities.BooleanFromString(GetStringValue(section, key, String.Empty), defaultValue);
        }

        private IniSection GetSection(string name)
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
            int sectionId = Sections.FindIndex(c => c.SectionName == section);
            if (sectionId == -1)
            {
                Sections.Add(new IniSection(section));
                Sections[Sections.Count - 1].Keys.Add(new string[2] { key, value });
            }
            else
            {
                int keyId = Sections[sectionId].Keys.FindIndex(c => c[0] == key);
                if (keyId == -1)
                    Sections[sectionId].Keys.Add(new string[2] { key, value });
                else
                    Sections[sectionId].Keys[keyId][1] = value;
            }
        }

        public void SetIntValue(string section, string key, int value)
        {
            int sectionId = Sections.FindIndex(c => c.SectionName == section);
            if (sectionId == -1)
            {
                Sections.Add(new IniSection(section));
                Sections[Sections.Count - 1].Keys.Add(new string[2] { key, Convert.ToString(value) });
            }
            else
            {
                int keyId = Sections[sectionId].Keys.FindIndex(c => c[0] == key);
                if (keyId == -1)
                    Sections[sectionId].Keys.Add(new string[2] { key, Convert.ToString(value) });
                else
                    Sections[sectionId].Keys[keyId][1] = Convert.ToString(value);
            }
        }

        public void SetDoubleValue(string section, string key, double value)
        {
            int sectionId = Sections.FindIndex(c => c.SectionName == section);
            if (sectionId == -1)
            {
                Sections.Add(new IniSection(section));
                Sections[Sections.Count - 1].Keys.Add(new string[2] { key, Convert.ToString(value, CultureInfo.GetCultureInfo("en-US").NumberFormat) });
            }
            else
            {
                int keyId = Sections[sectionId].Keys.FindIndex(c => c[0] == key);
                if (keyId == -1)
                    Sections[sectionId].Keys.Add(new string[2] { key, Convert.ToString(value, CultureInfo.GetCultureInfo("en-US").NumberFormat) });
                else
                    Sections[sectionId].Keys[keyId][1] = Convert.ToString(value, CultureInfo.GetCultureInfo("en-US").NumberFormat);
            }
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
            string strValue = Convert.ToString(value, CultureInfo.GetCultureInfo("en-US").NumberFormat);

            if (decimals > 0)
            {
                strValue = strValue + ".";
                for (int dc = 0; dc < decimals; dc++)
                    strValue = strValue + "0";
            }

            int sectionId = Sections.FindIndex(c => c.SectionName == section);
            if (sectionId == -1)
            {
                Sections.Add(new IniSection(section));
                Sections[Sections.Count - 1].Keys.Add(new string[2] { key, Convert.ToString(value, CultureInfo.GetCultureInfo("en-US").NumberFormat) });
            }
            else
            {
                int keyId = Sections[sectionId].Keys.FindIndex(c => c[0] == key);
                if (keyId == -1)
                    Sections[sectionId].Keys.Add(new string[2] { key, Convert.ToString(value, CultureInfo.GetCultureInfo("en-US").NumberFormat) });
                else
                    Sections[sectionId].Keys[keyId][1] = Convert.ToString(value, CultureInfo.GetCultureInfo("en-US").NumberFormat);
            }
        }

        public void SetBooleanValue(string section, string key, bool value)
        {
            string strValue;
            if (value)
                strValue = "Yes";
            else
                strValue = "No";

            int sectionId = Sections.FindIndex(c => c.SectionName == section);
            if (sectionId == -1)
            {
                Sections.Add(new IniSection(section));
                Sections[Sections.Count - 1].Keys.Add(new string[2] { key, strValue });
            }
            else
            {
                int keyId = Sections[sectionId].Keys.FindIndex(c => c[0] == key);
                if (keyId == -1)
                    Sections[sectionId].Keys.Add(new string[2] { key, strValue });
                else
                    Sections[sectionId].Keys[keyId][1] = strValue;
            }
        }

        /// <summary>
        /// Gets the names of all INI keys in the specified INI section.
        /// </summary>
        public List<string> GetSectionKeys(string sectionName)
        {
            IniSection section = Sections.Find(c => c.SectionName == sectionName);

            if (section == null)
                return null;
            else
            {
                List<string> returnValue = new List<string>();

                foreach (string[] key in section.Keys)
                    returnValue.Add(key[0]);

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the names of all sections in the INI file.
        /// </summary>
        public List<string> GetSections()
        {
            List<string> sectionList = new List<string>();

            foreach (IniSection section in Sections)
                sectionList.Add(section.SectionName);

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
    }

    public class IniSection
    {
        public IniSection() { }

        public IniSection(string sectionName)
        {
            SectionName = sectionName;
        }

        public string SectionName { get; set; }
        public List<string[]> Keys = new List<string[]>();

        public void AddKey(string keyName, string value)
        {
            string[] key = new string[2];
            key[0] = keyName;
            key[1] = value;

            Keys.Add(key);
        }
    }
}
