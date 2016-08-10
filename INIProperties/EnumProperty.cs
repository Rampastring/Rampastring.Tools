using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rampastring.Tools.INIProperties
{
    public class EnumProperty<T> : GenericINIProperty<T>, IIniProperty
    {
        public EnumProperty() : this((T)Activator.CreateInstance(typeof(T))) { }

        public EnumProperty(T defaultValue) : base(defaultValue)
        {
        }

        public override void ParseValue(IniFile iniFile, string sectionName, string keyName)
        {
            Value = (T)Enum.Parse(typeof(T),
                iniFile.GetStringValue(sectionName, keyName, DefaultValue.ToString()), true);
        }
    }
}
