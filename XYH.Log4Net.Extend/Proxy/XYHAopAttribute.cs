/**********************************************************************************
 * 类 名 称： IankaAopAttribute
 * 机器名称： IankaAopAttribute.cs
 * 命名空间： LogOperationService
 * 文 件 名： IankaAopAttribute
 * 创建时间： 2016-11-15 
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
    /// 安卡代理属性[作用于类].
    /// ************************************
    /// [IankaAop(typeof(IankaInstance)]
    /// [DecorateSymbol] Class ClassName
    /// ************************************
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IankaAopAttribute : ProxyAttribute
    {
        public IankaAopAttribute()
        {
        }

        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            IankaAopProxy realProxy = new IankaAopProxy(serverType);
            return realProxy.GetTransparentProxy() as MarshalByRefObject;
        }
    }
}
