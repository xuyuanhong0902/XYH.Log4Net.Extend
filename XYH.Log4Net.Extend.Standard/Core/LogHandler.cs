using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  XYH.Log4Net.Extend.Standard
{
    /// <summary>
    /// 日志记录实现.
    /// </summary>
    public class LogHandler : ILogHandler
    {
        /// <summary>
        /// 添加日志（只记录一条消息的日志）
        /// </summary>
        /// <param name="message">日志信息对象</param>
        public void WriteLog(object message)
        {
            //// 记录日志（入队列）
            ExtendLogQueue.Instance().EnqueueMessage(new LogMessage()
            {
                Message = message,
                Level = LogLevel.Info
            });
        }

        /// <summary>
        /// 添加日志（只记录一条指定日志类型的日志记录）
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="level">日志信息级别</param>
        public void WriteLog(object message, LogLevel level)
        {
            //// 记录日志（入队列）
            ExtendLogQueue.Instance().EnqueueMessage(new LogMessage()
            {
                Message = message,
                Level = level
            });
        }

        /// <summary>
        /// 添加日志（记录一条日常日志）
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="level">日志信息级别</param>
        /// <param name="exception">异常信息对象</param>
        public void WriteLog(object message, Exception exception)
        {
            //// 记录日志（入队列）
            ExtendLogQueue.Instance().EnqueueMessage(new LogMessage()
            {
                Message = message,
                Level = LogLevel.Error,
                Exception = exception
            });
        }

        /// <summary>
        /// 添加日志（简单记录一个方法的出参、入参）
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="methodParam">方法入参</param>
        /// <param name="methodResult">方法请求结果</param>
        public void WriteLog(object message, string methodName, object methodParam, object methodResult)
        {
            //// 记录日志（入队列）
            ExtendLogQueue.Instance().EnqueueMessage(new LogMessage()
            {
                Message = message,
                Level = LogLevel.Info,
                MethodName = methodName,
                MethodParam = methodParam,
                MethodResult = methodResult
            });
        }

        /// <summary>
        /// 添加日志.（简单记录一个方法的出参、入参,以及日志类型）
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="methodParam">方法入参</param>
        /// <param name="methodResult">方法请求结果</param>
        /// <param name="level">日志记录级别</param>
        public void WriteLog(object message, string methodName, object methodParam, object methodResult, LogLevel level)
        {
            //// 记录日志（入队列）
            ExtendLogQueue.Instance().EnqueueMessage(new LogMessage()
            {
                Message = message,
                Level = level,
                MethodName = methodName,
                MethodParam = methodParam,
                MethodResult = methodResult
            });
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="extendLogInfor">具体的日志消息model</param>
        public void WriteLog(LogMessage extendLogInfor)
        {
            //// 记录日志（入队列）
            ExtendLogQueue.Instance().EnqueueMessage(extendLogInfor);
        }
    }
}