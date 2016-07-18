using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rampastring.Tools
{
    public class ExtendedStringBuilder
    {
        public ExtendedStringBuilder()
        {
            stringBuilder = new StringBuilder();
        }

        public ExtendedStringBuilder(string value, bool useSeparator)
        {
            stringBuilder = new StringBuilder(value);
            UseSeparator = useSeparator;
        }

        private StringBuilder stringBuilder;

        public char Separator { get; set; }
        public bool UseSeparator { get; private set; }

        public int Length { get { return stringBuilder.Length; } }

        public void Append(int value)
        {
            Append(value.ToString());
        }

        public void Append(object value)
        {
            Append(value.ToString());
        }

        public void Append(string value)
        {
            stringBuilder.Append(value);
            if (UseSeparator)
                stringBuilder.Append(Separator);
        }

        public void Remove(int startIndex, int length)
        {
            stringBuilder.Remove(startIndex, length);
        }

        public override string ToString()
        {
            if (UseSeparator)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);

            return stringBuilder.ToString();
        }
    }
}
