using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYH.Log4Net.Extend.Standard.Standard;

namespace  XYH.Log4Net.Extend.Standard
{
    /// <summary>
    /// 记录日志扩展入口
    /// </summary>
    public class XYHLogOperator
    {
        /// <summary>
        /// 添加日志.
        /// </summary>
        /// <param name="message">日志信息对象</param>
        public static void WriteLog(object message)
        {
            new MessageIntoQueue().WriteLog(message);
        }

        /// <summary>
        /// 添加日志.
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="level">日志信息级别</param>
        public static void WriteLog(object message, LogLevel level)
        {
            new MessageIntoQueue().WriteLog(message, level);
        }

        /// <summary>
        /// 添加日志.
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="level">日志信息级别</param>
        /// <param name="exception">异常信息对象</param>
        public static void WriteLog(object message, Exception exception)
        {
            new MessageIntoQueue().WriteLog(message, exception);
        }

        /// <summary>
        /// 添加日志.
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="methodParam">方法入参</param>
        /// <param name="methodResult">方法请求结果</param>
        public static void WriteLog(object message, string methodName, object methodParam, object methodResult)
        {
            new MessageIntoQueue().WriteLog(message, methodName, methodParam, methodResult);
        }

        /// <summary>
        /// 添加日志.
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="methodParam">方法入参</param>
        /// <param name="methodResult">方法请求结果</param>
        /// <param name="level">日志记录级别</param>
        public static void WriteLog(object message, string methodName, object methodParam, object methodResult, LogLevel level)
        {
            new MessageIntoQueue().WriteLog(message, methodName, methodParam, methodResult, level);
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="extendLogInfor">具体的日志消息model</param>
        public static void WriteLog(LogMessage extendLogInfor)
        {
            new MessageIntoQueue().WriteLog(extendLogInfor);
        }
    }
}
