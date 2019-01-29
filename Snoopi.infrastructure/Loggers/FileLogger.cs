
using System;
using System.IO;
using System.Text;

namespace Snoopi.infrastructure.Loggers
{
    public class FileLogger : ILogger
    {
        void ILogger.LogText(string header, string text, string targetLocation)
        {
            var sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString());
            sb.Append(": ");
            sb.Append(header);
            sb.Append(" - ");
            sb.Append(text);
            sb.Append("\n");
            File.AppendAllText(targetLocation + "snoopi-log.txt", sb.ToString());
            sb.Clear();
        }
    }
}
