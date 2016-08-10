using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rampastring.Tools.INIProperties
{
    /// <summary>
    /// A double to be parsed from an INI file.
    /// </summary>
    public class DoubleProperty : GenericINIProperty<double>, IIniProperty
    {
        public DoubleProperty() : this(0.0) { }

        public DoubleProperty(double defaultValue) : base(defaultValue)
        {
        }

        public override void ParseValue(IniFile iniFile, string sectionName, string keyName)
        {
            Value = iniFile.GetDoubleValue(sectionName, keyName, DefaultValue);
        }
    }
}
