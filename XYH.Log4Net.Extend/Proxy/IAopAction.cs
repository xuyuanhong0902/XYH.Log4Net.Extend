/**********************************************************************************
 * 类 名 称： IAopAction
 * 机器名称： IAopAction.cs
 * 命名空间： XYH.Log4Net.Extend
 * 文 件 名： IAopAction
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
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace XYH.Log4Net.Extend
{
    /// <summary>
    /// 代理处理接口.
    /// </summary>
    public interface IAopAction
    {
        /// <summary>
        /// 代理方法前处理.
        /// </summary>
        /// <param name="methodInvoke">代理方法</param>
        IMessage PreProcess(IMessage methodInvoke, MarshalByRefObject refObject);

        /// <summary>
        /// 代理方法后处理.
        /// </summary>
        /// <param name="methodInvoke">代理方法</param>
        /// <param name="returnInvoke">代理返回</param>
        IMessage PostProcess(IMessage methodInvoke, IMessage returnInvoke, MarshalByRefObject refObject, DateTime executeStartTime, DateTime executeEndTime);

        /// <summary>
        /// 代理方法异常处理.
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="methodInvoke">代理方法</param>
        IMessage ExceptionProcess(Exception ex, IMessage methodInvoke, MarshalByRefObject refObject, DateTime executeStartTime, DateTime executeEndTime);
    }
}
