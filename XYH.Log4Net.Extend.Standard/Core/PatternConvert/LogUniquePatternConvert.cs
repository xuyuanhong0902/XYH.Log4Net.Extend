/**********************************************************************************
 * 类 名 称： LogUniquePatternConvert
 * 机器名称： LogUniquePatternConvert.cs
 * 命名空间： XYH.Log4Net.Extend
 * 文 件 名： LogUniquePatternConvert
 * 创建时间： 2019-06-09 
 * 作    者： 
 * 说    明：
 * ----------------------------------------------------------------------------------
 * 修改时间：
 * 修 改 人：
 * 修改说明:
 **********************************************************************************/
using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  XYH.Log4Net.Extend.Standard
{
    /// <summary>
    /// 日志唯一码[日志编号].
    /// </summary>
    public class LogUniquePatternConvert : PatternLayoutConverter
    {
        /// <summary>
        /// 转化.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="loggingEvent"></param>
        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            var LogMessage = loggingEvent.MessageObject as LogMessage;
            if (LogMessage != null)
            {
                writer.Write(LogMessage.LogUniqueCode);
            }
        }
    }
}
