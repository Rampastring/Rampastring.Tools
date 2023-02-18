namespace Rampastring.Tools.INIProperties;

/// <summary>
/// An integer to be parsed from an INI file.
/// </summary>
public class IntProperty : GenericINIProperty<int>, IIniProperty
{
    public IntProperty()
        : this(0)
    {
    }

    public IntProperty(int defaultValue)
        : base(defaultValue)
    {
    }

    public override void ParseValue(IniFile iniFile, string sectionName, string keyName) => Value = iniFile.GetIntValue(sectionName, keyName, DefaultValue);
}