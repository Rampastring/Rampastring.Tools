using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rampastring.Tools.INIProperties
{
    /// <summary>
    /// A string to be parsed from an INI file.
    /// </summary>
    public class StringProperty : GenericINIProperty<string>, IIniProperty
    {
        public StringProperty(string defaultValue) : base(defaultValue)
        {
        }

        public override void ParseValue(IniFile iniFile, string sectionName, string keyName)
        {
            Value = iniFile.GetStringValue(sectionName, keyName, DefaultValue);
        }
    }
}
