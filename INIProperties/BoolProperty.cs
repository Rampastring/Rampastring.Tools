namespace Rampastring.Tools.INIProperties;

/// <summary>
/// A boolean to be parsed from an INI file.
/// </summary>
public class BoolProperty : GenericINIProperty<bool>, IIniProperty
{
    public BoolProperty()
        : this(false)
    {
    }

    public BoolProperty(bool defaultValue)
        : base(defaultValue)
    {
    }

    public override void ParseValue(IniFile iniFile, string sectionName, string keyName)
        => Value = iniFile.GetBooleanValue(sectionName, keyName, DefaultValue);
}