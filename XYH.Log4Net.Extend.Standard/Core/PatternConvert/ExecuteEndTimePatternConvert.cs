using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  XYH.Log4Net.Extend.Standard
{
    /// <summary>
    /// 结束执行时间.
    /// </summary>
    public class ExecuteEndTimePatternConvert : PatternLayoutConverter
    {
        /// <summary>
        /// 结束执行时间.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="loggingEvent"></param>
        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            var LogMessage = loggingEvent.MessageObject as LogMessage;
            if (LogMessage != null)
            {
                writer.Write(LogMessage.ExecuteEndTime);
            }
        }
    }
}
