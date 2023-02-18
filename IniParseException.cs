namespace Rampastring.Tools;

using System;

public class IniParseException : Exception
{
    public IniParseException(string message)
        : base(message)
    {
    }
}