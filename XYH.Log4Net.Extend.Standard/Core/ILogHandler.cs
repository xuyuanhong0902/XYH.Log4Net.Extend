/**********************************************************************************
 * 类 名 称： ILogHandler
 * 机器名称： ILogHandler.cs
 * 命名空间： XYH.Log4Net.Extend
 * 文 件 名： ILogHandler
 * 创建时间： 2019-06-09 
 * 作    者： 
 * 说    明：
 * ----------------------------------------------------------------------------------
 * 修改时间：
 * 修 改 人：
 * 修改说明:
 **********************************************************************************/
using log4net;
using System;

namespace  XYH.Log4Net.Extend.Standard
{
    /// <summary>
    /// 日志接口名称.
    /// </summary>
    public interface ILogHandler 
    {
        /// <summary>
        /// 添加日志（只记录一条消息的日志）
        /// </summary>
        /// <param name="message">日志信息对象</param>
        void WriteLog(object message);

        /// <summary>
        /// 添加日志（只记录一条指定日志类型的日志记录）
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="level">日志信息级别</param>
        void WriteLog(object message, LogLevel level);

        /// <summary>
        /// 添加日志（记录一条日常日志）
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="level">日志信息级别</param>
        /// <param name="exception">异常信息对象</param>
        void WriteLog(object message, Exception exception);

        /// <summary>
        /// 添加日志（简单记录一个方法的出参、入参）
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="methodParam">方法入参</param>
        /// <param name="methodResult">方法请求结果</param>
        void WriteLog(object message, string methodName, object methodParam, object methodResult);

        /// <summary>
        /// 添加日志.（简单记录一个方法的出参、入参,以及日志类型）
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="methodParam">方法入参</param>
        /// <param name="methodResult">方法请求结果</param>
        /// <param name="level">日志记录级别</param>
        void WriteLog(object message, string methodName, object methodParam, object methodResult, LogLevel level);

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="extendLogInfor">具体的日志消息model</param>
        void WriteLog(LogMessage extendLogInfor);
    }
}
