namespace CDX.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ILogger
    {
        void Log(DateTime timeStamp, LogLevel level, string? category, string? message);
    }
}
