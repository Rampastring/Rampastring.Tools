using System;

namespace Rampastring.Tools;

/// <summary>
/// Defines how boolean values are converted to strings when a <see cref="IniFile"/> is written.
/// </summary>
[Flags]
public enum BooleanStringStyle
{
    /// <summary>
    /// Write boolean values as "True" and "False".
    /// </summary>
    TRUEFALSE = 0,

    /// <summary>
    /// Write boolean values as "Yes" and "No".
    /// </summary>
    YESNO = 1,

    /// <summary>
    /// Write boolean values as "true" and "false".
    /// </summary>
    TRUEFALSE_LOWERCASE = 2,

    /// <summary>
    /// Write boolean values as "yes" and "no".
    /// </summary>
    YESNO_LOWERCASE = 3,

    /// <summary>
    /// Write boolean values as "1" and "0".
    /// </summary>
    ONEZERO = 4
}
