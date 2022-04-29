using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rampastring.Tools
{
    public class IniParseException : Exception
    {
        public IniParseException(string message) : base(message)
        {
        }
    }
}
