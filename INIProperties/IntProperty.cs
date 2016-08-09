using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rampastring.Tools.INIProperties
{
    /// <summary>
    /// An integer to be parsed from an INI file.
    /// </summary>
    public class IntProperty : GenericINIProperty<int>, IIniProperty
    {
        public IntProperty(int defaultValue) : base(defaultValue)
        {
        }

        public override void ParseValue(IniFile iniFile, string sectionName, string keyName)
        {
            Value = iniFile.GetIntValue(sectionName, keyName, DefaultValue);
        }
    }
}
