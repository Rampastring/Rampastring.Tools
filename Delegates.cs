using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rampastring.Tools
{
    /// <summary>
    /// Provides commonly used delegates.
    /// </summary>
    public static class Delegates
    {
        public delegate void StringDelegate(string parameter);
        public delegate void IntDelegate(int parameter);
        public delegate void DualStringDelegate(string param1, string param2);
        public delegate void TripleStringDelegate(string param1, string param2, string param3);
        public delegate void StringArrayDelegate(string[] args);
    }
}
