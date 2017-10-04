using System;
using System.IO;
using System.Text;
namespace ludoux.DuduSpider
{
    class LogWriter
    {
        public static void WriteLine(string logText, string timeFormat = "[U yy-MM-dd HH:mm:ss] ", bool toConsole = true, string logFilePath = "log.txt")
        {
            string t = DateTime.UtcNow.ToString(timeFormat) + logText;
            if (toConsole)
                Console.Write(Environment.NewLine + t);
            if (logFilePath != "")
                File.AppendAllText(logFilePath, Environment.NewLine + t, Encoding.UTF8);
        }
        public static void Write(string logText, bool toConsole = true, string logFilePath = "log.txt")
        {
            string t = logText;
            if (toConsole)
                Console.Write(t);
            if (logFilePath != "")
                File.AppendAllText(logFilePath, t, Encoding.UTF8);
        }
    }
}
