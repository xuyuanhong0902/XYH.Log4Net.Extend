/**********************************************************************************
 * 类 名 称： LogAopActionImpl
 * 机器名称： LogAopActionImpl.cs
 * 命名空间： XYH.Log4Net.Extend
 * 文 件 名： LogAopActionImpl
 * 创建时间： 2019-06-09 
 * 作    者： 
 * 说    明：
 * ----------------------------------------------------------------------------------
 * 修改时间：
 * 修 改 人：
 * 修改说明:
 **********************************************************************************/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace XYH.Log4Net.Extend
{
    /// <summary>
    /// 代理日志处理实现.
    /// </summary>
    public class LogAopActionImpl : IAopAction
    {
        /// <summary>
        /// 日志记录方法调用前处理.
        /// </summary>
        /// <param name="methodInvoke">代理方法</param>
        /// <param name="refObject">代理对象</param>
        public IMessage PreProcess(IMessage methodInvoke, MarshalByRefObject refObject)
        {
            IMethodCallMessage call = methodInvoke as IMethodCallMessage;
            XYHLogOperator.WriteLog(new LogMessage()
            {
                MethodName = call.MethodName,
                MethodParam = JsonConvert.SerializeObject(call.Args),
                LogProjectName = call.TypeName,
                Level = LogLevel.Info
            });
            return null;
        }

        /// <summary>
        /// 日志记录方法调用后处理.
        /// </summary>
        /// <param name="methodInvoke">代理方法</param>
        /// <param name="returnInvoke">代理返回</param>
        /// <param name="refObject">代理对象</param>
        public IMessage PostProcess(IMessage methodInvoke, IMessage returnInvoke, MarshalByRefObject refObject, DateTime executeStartTime, DateTime executeEndTime)
        {
            IMethodCallMessage call = methodInvoke as IMethodCallMessage;
            ReturnMessage result = returnInvoke as ReturnMessage;
            XYHLogOperator.WriteLog(new LogMessage()
            {
                MethodName = call.MethodName,
                MethodParam = JsonConvert.SerializeObject(call.Args),
                MethodResult = JsonConvert.SerializeObject(result.Properties["__Return"]),
                LogProjectName = call.TypeName,
                ExecuteEndTime = executeEndTime,
                ExecuteStartTime = executeStartTime,
                Level = LogLevel.Info
            });

            return returnInvoke;
        }

        /// <summary>
        /// 日志记录方法异常处理.
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="methodInvoke">代理方法</param>
        /// <param name="refObject">代理对象</param>
        public IMessage ExceptionProcess(Exception ex, IMessage methodInvoke, MarshalByRefObject refObject, DateTime executeStartTime, DateTime executeEndTime)
        {
            IMethodCallMessage call = methodInvoke as IMethodCallMessage;

            XYHLogOperator.WriteLog(new LogMessage()
            {
                MethodName = call.MethodName,
                MethodParam = JsonConvert.SerializeObject(call.Args),
                LogProjectName = call.TypeName,
                Level = LogLevel.Error,
                Exception = ex.InnerException,
                ExecuteEndTime = executeEndTime,
                ExecuteStartTime = executeStartTime
            });
            return null;
        }
    }
}
