using System;
using System.Text;
using System.IO;

namespace Rampastring.Tools
{
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
            lock (locker)
            {
                if (WriteToConsole)
                    Console.WriteLine(data);

                if (WriteLogFile)
                {
                    try
                    {
                        StreamWriter sw = new StreamWriter(LogPath + LogFileName, true);

                        DateTime now = DateTime.Now;

                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0,2:D2}", now.Hour));
                        sb.Append(":");
                        sb.Append(String.Format("{0,2:D2}", now.Minute));
                        sb.Append(":");
                        sb.Append(String.Format("{0,2:D2}", now.Second));
                        sb.Append(".");
                        sb.Append(String.Format("{0,3:D3}", now.Millisecond));
                        sb.Append("    ");
                        sb.Append(data);

                        sw.WriteLine(now.Day + ". " + now.Month + ". " + sb.ToString());

                        sw.Close();
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
                        StreamWriter sw = new StreamWriter(LogPath + fileName, true);

                        DateTime now = DateTime.Now;

                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0,2:D2}", now.Hour));
                        sb.Append(":");
                        sb.Append(String.Format("{0,2:D2}", now.Minute));
                        sb.Append(":");
                        sb.Append(String.Format("{0,2:D2}", now.Second));
                        sb.Append(".");
                        sb.Append(String.Format("{0,3:D3}", now.Millisecond));
                        sb.Append("    ");
                        sb.Append(data);

                        sw.WriteLine(now.Day + ". " + now.Month + ". " + sb.ToString());

                        sw.Close();
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
                        StreamWriter sw = new StreamWriter(LogPath + LogFileName, true);

                        DateTime now = DateTime.Now;

                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0,2:D2}", now.Hour));
                        sb.Append(":");
                        sb.Append(String.Format("{0,2:D2}", now.Minute));
                        sb.Append(":");
                        sb.Append(String.Format("{0,2:D2}", now.Second));
                        sb.Append(".");
                        sb.Append(String.Format("{0,3:D3}", now.Millisecond));
                        sb.Append("    ");
                        sb.Append(String.Format(data, f1));

                        sw.WriteLine(now.Day + ". " + now.Month + ". " + sb.ToString());

                        sw.Close();
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
                        StreamWriter sw = new StreamWriter(LogPath + LogFileName, true);

                        DateTime now = DateTime.Now;

                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0,2:D2}", now.Hour));
                        sb.Append(":");
                        sb.Append(String.Format("{0,2:D2}", now.Minute));
                        sb.Append(":");
                        sb.Append(String.Format("{0,2:D2}", now.Second));
                        sb.Append(".");
                        sb.Append(String.Format("{0,3:D3}", now.Millisecond));
                        sb.Append("    ");
                        sb.Append(String.Format(data, f1, f2));

                        sw.WriteLine(now.Day + ". " + now.Month + ". " + sb.ToString());

                        sw.Close();
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
                    StreamWriter sw = new StreamWriter(LogPath + LogFileName, true);

                    DateTime now = DateTime.Now;

                    StringBuilder sb = new StringBuilder();
                    sb.Append(String.Format("{0,2:D2}", now.Hour));
                    sb.Append(":");
                    sb.Append(String.Format("{0,2:D2}", now.Minute));
                    sb.Append(":");
                    sb.Append(String.Format("{0,2:D2}", now.Second));
                    sb.Append(".");
                    sb.Append(String.Format("{0,3:D3}", now.Millisecond));
                    sb.Append("    ");
                    sb.Append(data);

                    sw.WriteLine(now.Day + ". " + now.Month + ". " + sb.ToString());

                    sw.Close();
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
                    StreamWriter sw = new StreamWriter(LogPath + fileName, true);

                    DateTime now = DateTime.Now;

                    StringBuilder sb = new StringBuilder();
                    sb.Append(String.Format("{0,2:D2}", now.Hour));
                    sb.Append(":");
                    sb.Append(String.Format("{0,2:D2}", now.Minute));
                    sb.Append(":");
                    sb.Append(String.Format("{0,2:D2}", now.Second));
                    sb.Append(".");
                    sb.Append(String.Format("{0,3:D3}", now.Millisecond));
                    sb.Append("    ");
                    sb.Append(data);

                    sw.WriteLine(now.Day + ". " + now.Month + ". " + sb.ToString());

                    sw.Close();
                }
                catch
                {
                }
            }
        }
    }
}
