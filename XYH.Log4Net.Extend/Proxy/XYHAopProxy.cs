/**********************************************************************************
 * 类 名 称： IankaAopProxy
 * 机器名称： IankaAopProxy.cs
 * 命名空间： XYH.Log4Net.Extend
 * 文 件 名： IankaAopProxy
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
using System.Reflection;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace XYH.Log4Net.Extend
{
    /// <summary>
    /// XYH代理实现类.
    /// </summary>
    public class XYHAopProxy : RealProxy
    {
        /// <summary>
        /// 构造函数.
        /// </summary>
        /// <param name="target">目标类型.</param>
        public XYHAopProxy(Type target)
            : base(target)
        {
        }

        /// <summary>
        /// 重写代理实现.
        /// </summary>
        /// <param name="msg">代理函数</param>
        /// <returns>返回结果</returns>
        public override IMessage Invoke(IMessage methodInvoke)
        {
            //// 方法开始执行时间
            DateTime executeStartTime = System.DateTime.Now;

            //// 方法执行结束时间
            DateTime executeEndTime = System.DateTime.Now;

            IMessage message = null;
            IMethodCallMessage call = methodInvoke as IMethodCallMessage;
            object[] customAttributeArray = call.MethodBase.GetCustomAttributes(false);
            call.MethodBase.GetCustomAttributes(false);

            try
            {
                // 前处理.
                List<IAopAction> proActionList = this.InitAopAction(customAttributeArray, AdviceType.Before);

                //// 方法执行开始记录日志
                if (proActionList != null && proActionList.Count > 0  )
                {
                    foreach (IAopAction item in proActionList)
                    {
                        IMessage preMessage = item.PreProcess(methodInvoke, base.GetUnwrappedServer());
                        if (preMessage != null)
                        {
                            message = preMessage;
                        }
                    }

                    if (message != null)
                    {
                        return message;
                    }
                }

                message = Proessed(methodInvoke);

                // 后处理.
                proActionList = this.InitAopAction(customAttributeArray, AdviceType.Around);

                //// 方法执行结束时间
                executeEndTime = System.DateTime.Now;

                //// 方法执行结束记录日志
                if (proActionList != null && proActionList.Count > 0)
                {
                    foreach (IAopAction item in proActionList)
                    {
                        item.PostProcess(methodInvoke, message, base.GetUnwrappedServer(), executeStartTime, executeEndTime);
                    }
                }
            }
            catch (Exception ex)
            {
                //// 方法执行结束时间
                executeEndTime = System.DateTime.Now;

                // 异常处理.吃掉异常，不影响主业务
                List<IAopAction> proActionList = this.InitAopAction(customAttributeArray, AdviceType.Around);
                if (proActionList != null && proActionList.Count > 0)
                {
                    foreach (IAopAction item in proActionList)
                    {
                        item.ExceptionProcess(ex, methodInvoke, base.GetUnwrappedServer(), executeStartTime, executeEndTime);
                    }
                }
            }

            return message;
        }

        /// <summary>
        /// 处理方法执行.
        /// </summary>
        /// <param name="methodInvoke">代理目标方法</param>
        /// <returns>代理结果</returns>
        public virtual IMessage Proessed(IMessage methodInvoke)
        {
            IMessage message;
            if (methodInvoke is IConstructionCallMessage)
            {
                message = this.ProcessConstruct(methodInvoke);
            }
            else
            {
                message = this.ProcessInvoke(methodInvoke);
            }
            return message;
        }

        /// <summary>
        /// 普通代理方法执行.
        /// </summary>
        /// <param name="methodInvoke">代理目标方法</param>
        /// <returns>代理结果</returns>
        public virtual IMessage ProcessInvoke(IMessage methodInvoke)
        {
            IMethodCallMessage callMsg = methodInvoke as IMethodCallMessage;
            object[] args = callMsg.Args;   //方法参数                 
            object o = callMsg.MethodBase.Invoke(base.GetUnwrappedServer(), args);  //调用 原型类的 方法       

            return new ReturnMessage(o, args, args.Length, callMsg.LogicalCallContext, callMsg);   // 返回类型 Message
        }

        /// <summary>
        /// 构造函数代理方法执行.
        /// </summary>
        /// <param name="methodInvoke">代理目标方法</param>
        /// <returns>代理结果</returns>
        public virtual IMessage ProcessConstruct(IMessage methodInvoke)
        {
            IConstructionCallMessage constructCallMsg = methodInvoke as IConstructionCallMessage;
            IConstructionReturnMessage constructionReturnMessage = this.InitializeServerObject((IConstructionCallMessage)methodInvoke);
            RealProxy.SetStubData(this, constructionReturnMessage.ReturnValue);

            return constructionReturnMessage;
        }

        /// <summary>
        /// 代理包装业务处理.
        /// </summary>
        /// <param name="customAttributeArray">代理属性</param>
        /// <param name="adviceType">处理类型</param>
        /// <returns>结果.</returns>
        public virtual List<IAopAction> InitAopAction(object[] customAttributeArray, AdviceType adviceType)
        {
            List<IAopAction> actionList = new List<IAopAction>();
            if (customAttributeArray != null && customAttributeArray.Length > 0)
            {
                foreach (Attribute item in customAttributeArray)
                {
                    XYHMethodAttribute methodAdviceAttribute = item as XYHMethodAttribute;
                    if (methodAdviceAttribute != null && (methodAdviceAttribute.AdviceType == adviceType))
                    {
                        if (methodAdviceAttribute.ProcessType == ProcessType.None)
                        {
                            continue;
                        }

                        if (methodAdviceAttribute.ProcessType == ProcessType.Log)
                        {
                            actionList.Add(new LogAopActionImpl());
                            continue;
                        }
                    }
                }
            }

            return actionList;
        }
    }
}
