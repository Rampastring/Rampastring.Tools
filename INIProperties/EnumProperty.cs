using System;

namespace Rampastring.Tools.INIProperties;

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
