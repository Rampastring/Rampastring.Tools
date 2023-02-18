namespace Rampastring.Tools.INIProperties;

/// <summary>
/// A string to be parsed from an INI file.
/// </summary>
public class StringProperty : GenericINIProperty<string>, IIniProperty
{
    public StringProperty()
        : this(string.Empty)
    {
    }

    public StringProperty(string defaultValue)
        : base(defaultValue)
    {
    }

    public override void ParseValue(IniFile iniFile, string sectionName, string keyName) => Value = iniFile.GetStringValue(sectionName, keyName, DefaultValue);
}