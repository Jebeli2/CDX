namespace CDX.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ConsoleLogger : ILogger
    {

        private static void SetConsoleColor(LogLevel level, string? category)
        {
            switch (level)
            {
                case LogLevel.ERROR:
                case LogLevel.CRITICAL:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.WARN:
                    if ("LOG" == category)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    break;
                case LogLevel.DEBUG:
                case LogLevel.VERBOSE:
                    if ("LOG" == category)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    break;
                case LogLevel.INFO:
                default:
                    if ("LOG" == category)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    break;
            }
        }

        private static void ClearConsoleColor()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Log(DateTime timeStamp, LogLevel level, string? category, string? message)
        {
            ClearConsoleColor();
            Console.Write($"{timeStamp:yy-MM-dd HH:mm:ss.fff} ");
            SetConsoleColor(level, category);
            Console.WriteLine($"{level} {category} {message}");
            ClearConsoleColor();
        }
    }
}
