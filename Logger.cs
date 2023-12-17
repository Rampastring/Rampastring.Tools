using System;
using System.Text;
using System.IO;
using System.Globalization;

namespace Rampastring.Tools;

/// <summary>
/// A fairly self-explanatory class for logging.
/// </summary>
public static class Logger
{
    public static bool WriteToConsole { get; set; }

    public static bool WriteLogFile { get; set; }

    private static string LogPath;

    private static string LogFileName;

    private static readonly object locker = new object();

    public static void Initialize(string logFilePath, string logFileName)
    {
        LogPath = logFilePath;
        LogFileName = logFileName;
    }

    public static void Log(string data)
    {
        DoLog(data, LogFileName, WriteToConsole, WriteLogFile);
    }

    public static void Log(string data, string fileName)
    {
        DoLog(data, fileName, WriteToConsole, WriteLogFile);
    }

    public static void Log(string data, object f1)
    {
        DoLog(string.Format(CultureInfo.InvariantCulture, data, f1), LogFileName, WriteToConsole, WriteLogFile);
    }

    public static void Log(string data, object f1, object f2)
    {
        DoLog(string.Format(CultureInfo.InvariantCulture, data, f1, f2), LogFileName, WriteToConsole, WriteLogFile);
    }

    public static void ForceLog(string data)
    {
        DoLog(data, LogFileName, true, true);
    }

    public static void ForceLog(string data, string fileName)
    {
        DoLog(data, fileName, true, true);
    }

    private static void DoLog(string data, string fileName, bool writeToConsole, bool writeToFile)
    {
        lock (locker)
        {
            if (writeToConsole)
                Console.WriteLine(data);

            if (writeToFile)
            {
                try
                {
                    using var sw = new StreamWriter(SafePath.CombineFilePath(LogPath, fileName), true);

                    DateTime now = DateTime.Now;

                    var sb = new StringBuilder();
                    sb.Append(now.ToString("dd.MM. HH:mm:ss.fff", CultureInfo.InvariantCulture));
                    sb.Append("    ");
                    sb.Append(data);

                    sw.WriteLine(sb.ToString());
                }
                catch
                {
                }
            }
        }
    }
}
