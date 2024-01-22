using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame.CustomShitHack;

namespace DuckGame.CustomShitHack.Utility
{
    /// <summary>
    /// Severity of a log message.
    /// </summary>
    internal enum LogSeverity
    {
        /// <summary>
        /// General information.
        /// </summary>
        Information = 0,
        /// <summary>
        /// Information oly useful to developers.
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Might cause errors.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Crashes and/or malcfunctions are likely to happen.
        /// </summary>
        Error = 3,
        /// <summary>
        /// Something went wrong. Program must be terminated.
        /// </summary>
        Fatal = 4,
        /// <summary>
        /// A task was completed successfully.
        /// </summary>
        Success = 5
    }

    /// <summary>
    /// Logs to the DevConsole using appropriate formatting.
    /// </summary>
    internal static class Logger
    {
        private const string MOD_FACADE = "|DGORANGE|CSH-";
        private const int MAX_LINE_LENGTH = 50;

        /// <summary>
        /// Logs a message into the DevConsole.
        /// </summary>
        public static void Log(string message, LogSeverity severity = LogSeverity.Information)
        {
            LogLine(message, severity, true);
        }

        /// <summary>
        /// Logs several messages into the DevConsole.
        /// </summary>
        public static void Log(string[] messages, LogSeverity severity = LogSeverity.Information)
        {
            if (messages.Length == 0) return;

            LogLine(messages[0], severity, true);

            for (int i = 1; i < messages.Length; i++)
            {
                LogLine(messages[i], severity, false);
            }
        }

        private static void LogLine(string message, LogSeverity severity, bool firstLine = true)
        {
            string severityStr = get_severity_prefix(severity);

            // Split lines.
            if (message.Length > MAX_LINE_LENGTH)
            {
                string[] msg = message.SplitByLength(MAX_LINE_LENGTH).ToArray();
                Log(msg, severity);
                return;
            }

            // Get separator.
            string separator;

            if (firstLine)
            {
                separator = $"|WHITE|| {severityStr} |GRAY|>";
            }
            else
            {
                separator = $"|WHITE||  ";
            }

            DevConsole.Log($"{MOD_FACADE}{separator} |WHITE|{message}");
        }

        private static string get_severity_prefix(LogSeverity severity)
        {
            string prefix;

            switch (severity)
            {
                case LogSeverity.Debug:
                    prefix = "|GRAY|  DEBUG";
                    break;
                case LogSeverity.Warning:
                    prefix = "|YELLOW|WARNING";
                    break;
                case LogSeverity.Error:
                    prefix = "|DGRED|  ERROR";
                    break;
                case LogSeverity.Fatal:
                    prefix = "|RED|  FATAL";
                    break;
                case LogSeverity.Success:
                    prefix = "|LIME|SUCCESS";
                    break;
                case LogSeverity.Information:
                default:
                    prefix = "|DGBLUE|   INFO";
                    break;
            }

            return prefix;
        }
    }
}
