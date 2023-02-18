namespace Rampastring.Tools;

using System;
using System.Globalization;
using System.IO;
using System.Text;

/// <summary>
/// A fairly self-explanatory class for logging.
/// </summary>
public static class Logger
{
    private static readonly object locker = new();

    private static string logPath;

    private static string logFileName;

    public static bool WriteToConsole { get; set; }

    public static bool WriteLogFile { get; set; }

    public static void Initialize(string logFilePath, string logFileName)
    {
        logPath = logFilePath;
        Logger.logFileName = logFileName;
    }

    public static void Log(string data)
    {
        lock (locker)
        {
            if (WriteToConsole)
                Console.WriteLine(data);

            if (WriteLogFile)
            {
                try
                {
                    using var sw = new StreamWriter(SafePath.CombineFilePath(logPath, logFileName), true);

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

    public static void Log(string data, string fileName)
    {
        lock (locker)
        {
            if (WriteToConsole)
                Console.WriteLine(data);

            if (WriteLogFile)
            {
                try
                {
                    using var sw = new StreamWriter(SafePath.CombineFilePath(logPath, fileName), true);

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

    public static void Log(string data, object f1)
    {
        lock (locker)
        {
            if (WriteToConsole)
                Console.WriteLine(data);

            if (WriteLogFile)
            {
                try
                {
                    using var sw = new StreamWriter(SafePath.CombineFilePath(logPath, logFileName), true);

                    DateTime now = DateTime.Now;

                    var sb = new StringBuilder();
                    sb.Append(now.ToString("dd.MM. HH:mm:ss.fff", CultureInfo.InvariantCulture));
                    sb.Append("    ");
                    sb.Append(string.Format(CultureInfo.InvariantCulture, data, f1));

                    sw.WriteLine(sb.ToString());
                }
                catch
                {
                }
            }
        }
    }

    public static void Log(string data, object f1, object f2)
    {
        lock (locker)
        {
            if (WriteToConsole)
                Console.WriteLine(data);

            if (WriteLogFile)
            {
                try
                {
                    using var sw = new StreamWriter(SafePath.CombineFilePath(logPath, logFileName), true);

                    DateTime now = DateTime.Now;

                    var sb = new StringBuilder();
                    sb.Append(now.ToString("dd.MM. HH:mm:ss.fff", CultureInfo.InvariantCulture));
                    sb.Append("    ");
                    sb.Append(string.Format(CultureInfo.InvariantCulture, data, f1, f2));

                    sw.WriteLine(sb.ToString());
                }
                catch
                {
                }
            }
        }
    }

    public static void ForceLog(string data)
    {
        lock (locker)
        {
            Console.WriteLine(data);

            try
            {
                using var sw = new StreamWriter(SafePath.CombineFilePath(logPath, logFileName), true);

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

    public static void ForceLog(string data, string fileName)
    {
        lock (locker)
        {
            Console.WriteLine(data);

            try
            {
                using var sw = new StreamWriter(SafePath.CombineFilePath(logPath, fileName), true);

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