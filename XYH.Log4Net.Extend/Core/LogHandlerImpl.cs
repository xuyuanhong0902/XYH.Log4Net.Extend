/**********************************************************************************
 * 类 名 称： LogHandlerImpl
 * 机器名称： LogHandlerImpl.cs
 * 命名空间： XYH.Log4Net.Extend
 * 文 件 名： LogHandlerImpl
 * 创建时间： 2019-06-09 
 * 作    者： 
 * 说    明：
 * ----------------------------------------------------------------------------------
 * 修改时间：
 * 修 改 人：
 * 修改说明:
 **********************************************************************************/
using log4net.Core;
using System;
using System.Linq;
using System.Net;

namespace XYH.Log4Net.Extend
{
    /// <summary>
    /// 日志记录实现.
    /// </summary>
    public class LogHandlerImpl : LogImpl
    {
        /// <summary>
        /// 类型.
        /// </summary>
        private static readonly Type thisDeclaringType = typeof(LogHandlerImpl);

        /// <summary>
        /// 构造函数.
        /// </summary>
        /// <param name="logger">日志记录对象</param>
        public LogHandlerImpl(ILogger logger) : base(logger) { }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="extendLogInfor">具体的日志消息model</param>
        public void WriteLog(LogMessage extendLogInfor)
        {
            if (
                   (extendLogInfor.Level == LogLevel.Info && this.IsInfoEnabled)
                || (extendLogInfor.Level == LogLevel.Debug && this.IsDebugEnabled)
                || (extendLogInfor.Level == LogLevel.Warn && this.IsWarnEnabled)
                || (extendLogInfor.Level == LogLevel.Error && this.IsErrorEnabled)
                || (extendLogInfor.Level == LogLevel.Fatal && this.IsFatalEnabled))
            {
                Logger.Log(this.BuildLoggingEvent(extendLogInfor));
            }
        }

        /// <summary>
        /// 构建日志信息
        /// </summary>
        /// <param name="extendLogInfor">具体的日志model</param>
        /// <returns></returns>
        private LoggingEvent BuildLoggingEvent(LogMessage extendLogInfor)
        {
            LoggingEvent loggingEvent = null;
            //// 获取IP地址
            if (string.IsNullOrEmpty(extendLogInfor.IpAddress))
            {
                IPHostEntry myEntry = Dns.GetHostEntry(Dns.GetHostName());
                extendLogInfor.IpAddress = myEntry.AddressList.FirstOrDefault<IPAddress>(e => e.AddressFamily.ToString().Equals("InterNetwork")).ToString();
            }

            switch (extendLogInfor.Level)
            {
                case LogLevel.Info:
                    loggingEvent = new LoggingEvent(thisDeclaringType, Logger.Repository, Logger.Name, Level.Info, extendLogInfor.Message, extendLogInfor.Exception);
                    break;
                case LogLevel.Debug:
                    loggingEvent = new LoggingEvent(thisDeclaringType, Logger.Repository, Logger.Name, Level.Debug, extendLogInfor.Message, extendLogInfor.Exception);
                    break;
                case LogLevel.Warn:
                    loggingEvent = new LoggingEvent(thisDeclaringType, Logger.Repository, Logger.Name, Level.Warn, extendLogInfor.Message, extendLogInfor.Exception);
                    break;
                case LogLevel.Error:
                    loggingEvent = new LoggingEvent(thisDeclaringType, Logger.Repository, Logger.Name, Level.Error, extendLogInfor.Message, extendLogInfor.Exception);
                    break;
                case LogLevel.Fatal:
                    loggingEvent = new LoggingEvent(thisDeclaringType, Logger.Repository, Logger.Name, Level.Fatal, extendLogInfor.Message, extendLogInfor.Exception);
                    break;
                default:
                    loggingEvent = new LoggingEvent(thisDeclaringType, Logger.Repository, Logger.Name, Level.Fatal, extendLogInfor.Message, extendLogInfor.Exception);
                    break;
            }

            //// 构建日志详情
            if (loggingEvent != null)
            {
                loggingEvent.Properties["LogUniqueCode"] = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                loggingEvent.Properties["LogSerialNumber"] = extendLogInfor.LogSerialNumber;
                ///// System.Web.HttpContext.Current.Session["LogSerialNumber"];
                loggingEvent.Properties["LogMachineCode"] = Dns.GetHostName();
                loggingEvent.Properties["LogIpAddress"] = extendLogInfor.IpAddress;
                loggingEvent.Properties["LogContent"] = extendLogInfor.Message;
                loggingEvent.Properties["MethodName"] = extendLogInfor.MethodName;
                loggingEvent.Properties["MethodParam"] = extendLogInfor.MethodParam;
                loggingEvent.Properties["MethodResult"] = extendLogInfor.MethodResult;
                loggingEvent.Properties["LogProjectName"] = extendLogInfor.LogProjectName;
                loggingEvent.Properties["LogRecordLevel"] = extendLogInfor.Level;
                loggingEvent.Properties["LogRecordTime"] = DateTime.Now;
                loggingEvent.Properties["ExceptionMessage"] = extendLogInfor.Exception;
                loggingEvent.Properties["InvokeName"] = extendLogInfor.InvokeName;
                loggingEvent.Properties["ExecuteStartTime"] = extendLogInfor.ExecuteStartTime == null ? "" : extendLogInfor.ExecuteStartTime.ToString("yyyy-MM-dd HH:mm:ss ffff");
                loggingEvent.Properties["ExecuteEndTime"] = extendLogInfor.ExecuteEndTime == null ? "" : extendLogInfor.ExecuteEndTime.ToString("yyyy-MM-dd HH:mm:ss ffff");
                loggingEvent.Properties["ExecuteTime"] = (extendLogInfor.ExecuteStartTime != null && extendLogInfor.ExecuteEndTime != null) ?
                    (extendLogInfor.ExecuteEndTime - extendLogInfor.ExecuteStartTime).TotalSeconds + "秒" : "";
            }
            return loggingEvent;
        }
    }
}
