namespace Rampastring.Tools.INIProperties;

public abstract class GenericINIProperty<T> : IIniProperty
{
    protected GenericINIProperty(T defaultValue)
    {
        DefaultValue = defaultValue;
    }

    public T DefaultValue { get; }

    public T Value { get; protected set; }

    public static implicit operator T(GenericINIProperty<T> property)
    {
        return property.Value;
    }

    public abstract void ParseValue(IniFile iniFile, string sectionName, string keyName);

    public override string ToString()
        => Value.ToString();

    public T ToT()
        => Value;
}