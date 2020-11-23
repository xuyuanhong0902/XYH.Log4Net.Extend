/**********************************************************************************
 * 类 名 称： XYHAopAttribute
 * 机器名称： XYHAopAttribute.cs
 * 命名空间： XYH.Log4Net.Extend
 * 文 件 名： XYHAopAttribute
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
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace XYH.Log4Net.Extend
{
    /// <summary>
    /// XYH代理属性[作用于类].
    /// ************************************
    /// [DecorateSymbol] Class ClassName
    /// ************************************
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class XYHAopAttribute : ProxyAttribute
    {
        /// <summary>
        /// 代理处理类型.
        /// </summary>
        private ProcessType processType = ProcessType.None;

        /// <summary>
        /// 代理处理类型.
        /// </summary>
        public ProcessType ProcessType
        {
            get { return this.processType; }
        }

        public XYHAopAttribute()
        {
        }

        /// <summary>
        /// 构造函数(自定义设置是否需要记录日志，如果需要记录那么：记录入参、出参数日志)
        /// </summary>
        public XYHAopAttribute(ProcessType processType)
        {
            this.processType = processType;
        }

        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            XYHAopProxy realProxy = new XYHAopProxy(serverType);
            return realProxy.GetTransparentProxy() as MarshalByRefObject;
        }
    }
}
