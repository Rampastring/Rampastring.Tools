using System;

namespace Rampastring.Tools;

public class IniParseException : Exception
{
    public IniParseException(string message) : base(message)
    {
    }
}
