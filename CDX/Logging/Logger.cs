namespace CDX.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;

    public static class Logger
    {
        private static readonly List<ILogger> loggers = new();
        private static LogLevel logLevel;
        private static bool addSource = true;
        private const string CATEGORY = "CDX";
        private static int omitLimit = 4;
        private static Dictionary<LogLevel, string> lastSourceInfo = new();
        private static Dictionary<LogLevel, int> lastSourceCounter = new();

        public static LogLevel LogLevel
        {
            get { return logLevel; }
            set { logLevel = value; }
        }

        public static bool AddSource
        {
            get { return addSource; }
            set { addSource = value; }
        }

        public static void AddLogger(ILogger logger)
        {
            loggers.Add(logger);
        }
        public static void RemoveLogger(ILogger logger)
        {
            loggers.Remove(logger);
        }

        private static int GetSourceCounter(LogLevel logLevel, string sourceInfo)
        {
            int counter = 0;
            if (lastSourceInfo.TryGetValue(logLevel, out string? last))
            {
                if (sourceInfo == last)
                {
                    counter = lastSourceCounter[logLevel] + 1;
                }
                else
                {
                    counter = 1;
                }
            }
            lastSourceInfo[logLevel] = sourceInfo;
            lastSourceCounter[logLevel] = counter;
            return counter;
        }
        public static void Log(LogLevel level, string? category, string? msg)
        {
            DateTime timeStamp = DateTime.Now;
            foreach (ILogger logger in loggers)
            {
                logger.Log(timeStamp, level, category, msg);
            }
        }
        public static void Verbose(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (logLevel <= LogLevel.VERBOSE)
            {
                string srcInfo = BuildSourceInfo(memberName, sourceFilePath, sourceLineNumber);
                int srcCounter = GetSourceCounter(logLevel, srcInfo);
                if (srcCounter < omitLimit)
                {
                    Log(LogLevel.VERBOSE, CATEGORY, BuildMessage(message, srcInfo));
                }
                else if (srcCounter == omitLimit)
                {
                    Log(LogLevel.VERBOSE, "LOG", "Omitting more messages like these...");
                }
            }
        }

        public static void Debug(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (logLevel <= LogLevel.DEBUG)
            {
                string srcInfo = BuildSourceInfo(memberName, sourceFilePath, sourceLineNumber);
                int srcCounter = GetSourceCounter(logLevel, srcInfo);
                if (srcCounter < omitLimit)
                {
                    Log(LogLevel.DEBUG, CATEGORY, BuildMessage(message, srcInfo));
                }
                else if (srcCounter == omitLimit)
                {
                    Log(LogLevel.DEBUG, "LOG", "Omitting more messages like these...");
                }
            }
        }

        public static void Info(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (logLevel <= LogLevel.INFO)
            {
                string srcInfo = BuildSourceInfo(memberName, sourceFilePath, sourceLineNumber);
                int srcCounter = GetSourceCounter(logLevel, srcInfo);
                if (srcCounter < omitLimit)
                {
                    Log(LogLevel.INFO, CATEGORY, BuildMessage(message, srcInfo));
                }
                else if (srcCounter == omitLimit)
                {
                    Log(LogLevel.INFO, "LOG", "Omitting more messages like these...");
                }
            }
        }

        public static void Warn(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (logLevel <= LogLevel.WARN)
            {
                string srcInfo = BuildSourceInfo(memberName, sourceFilePath, sourceLineNumber);
                int srcCounter = GetSourceCounter(logLevel, srcInfo);
                if (srcCounter < omitLimit)
                {
                    Log(LogLevel.WARN, CATEGORY, BuildMessage(message, srcInfo));
                }
                else if (srcCounter == omitLimit)
                {
                    Log(LogLevel.WARN, "LOG", "Omitting more messages like these...");
                }
            }
        }

        public static void Error(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (logLevel <= LogLevel.ERROR)
            {
                Log(LogLevel.ERROR, CATEGORY, BuildMessage(memberName, sourceFilePath, sourceLineNumber, message));
            }
        }
        public static void Critical(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (logLevel <= LogLevel.CRITICAL)
            {
                Log(LogLevel.CRITICAL, CATEGORY, BuildMessage(memberName, sourceFilePath, sourceLineNumber, message));
            }
        }

        private static string BuildSourceInfo(string memberName, string sourceFilePath, int sourceLineNumber)
        {
            StringBuilder sb = new();
            sb.Append(Path.GetFileName(sourceFilePath));
            sb.Append(':');
            sb.Append(sourceLineNumber);
            sb.Append('(');
            sb.Append(memberName);
            sb.Append(')');
            return sb.ToString();
        }

        private static string BuildMessage(string message, string srcInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('"');
            sb.Append(message);
            sb.Append('"');
            if (addSource)
            {
                sb.Append(' ');
                sb.Append(srcInfo);
            }
            return sb.ToString();
        }
        private static string BuildMoreOmitedMessage(string memberName, string sourceFilePath, int sourceLineNumber, string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('"');
            sb.Append(message);
            sb.Append("\" ...omitting more messages like these...");
            return sb.ToString();
        }
        private static string BuildMessage(string memberName, string sourceFilePath, int sourceLineNumber, string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('"');
            sb.Append(message);
            sb.Append('"');
            if (addSource)
            {
                sb.Append(' ');
                sb.Append(BuildSourceInfo(memberName, sourceFilePath, sourceLineNumber));
            }
            return sb.ToString();
        }
    }
}
