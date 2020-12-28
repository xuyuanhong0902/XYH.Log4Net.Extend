/**********************************************************************************
 * 类 名 称： LogMessage
 * 机器名称： LogMessage.cs
 * 命名空间： XYH.Log4Net.Extend
 * 文 件 名： LogMessage
 * 创建时间： 2019-06-09 
 * 作    者： 
 * 说    明：
 * ----------------------------------------------------------------------------------
 * 修改时间：
 * 修 改 人：
 * 修改说明:
 **********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  XYH.Log4Net.Extend.Standard
{
    /// <summary>
    /// 日志消息.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// 日志唯一码.
        /// </summary>
        public string LogUniqueCode { get; set; }

        /// <summary>
        /// 日志序列号.
        /// </summary>
        public string LogSerialNumber { get; set; }

        /// <summary>
        /// 日志机器码.
        /// </summary>
        public string LogMachineCode { get; set; }

        /// <summary>
        /// 程序名称.
        /// </summary>
        public string LogProjectName { get; set; }

        /// <summary>
        /// 日志说明.
        /// </summary>
        public object LogContent { get; set; }

        /// <summary>
        /// 方法名.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 日志参数.
        /// </summary>
        public object MethodParam { get; set; }

        /// <summary>
        /// 返回结果.
        /// </summary>
        public object MethodResult { get; set; }

        /// <summary>
        /// 日志记录时间.
        /// </summary>
        public DateTime LogRecordTime { get; set; }

        /// <summary>
        /// 用户名.
        /// </summary>
        public string LogUserName { get; set; }

        /// <summary>
        /// 调用.
        /// </summary>
        public object InvokeName { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        /// <returns></returns>
        public object Message { get; set; }

        /// <summary>
        /// 执行开始时间
        /// </summary>
        public DateTime ExecuteStartTime { get; set; }

        /// <summary>
        /// 执行结束时间
        /// </summary>
        public DateTime ExecuteEndTime { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public string ExecuteTime { get; set; }

        /// <summary>
        /// 请求IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 日志等级类型
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// 异常消息
        /// </summary>
        public Exception Exception { get; set; }
    }
}
