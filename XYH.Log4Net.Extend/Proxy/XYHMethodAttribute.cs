/**********************************************************************************
 * 类 名 称： IankaMethodAttribute
 * 机器名称： IankaMethodAttribute.cs
 * 命名空间： LogOperationService
 * 文 件 名： IankaMethodAttribute
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
using System.Text;
using System.Threading.Tasks;

namespace XYH.Log4Net.Extend
{
    /// <summary>
    /// 日志记录属性[作用于方法].
    /// 用法[AdviceType取值：None、Before、After、Around]
    ///     [ProcessType取值：None、Log]：
    /// ************************************
    /// [IankaMethod(ProcessType.None,AdviceType.Before)]
    /// [DecorateSymbol] ReturnType MethodName([parameter])
    /// ************************************
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class IankaMethodAttribute : Attribute
    {
        /// <summary>
        /// 代理处理方式.
        /// </summary>
        private AdviceType adviceType = AdviceType.None;

        /// <summary>
        /// 代理处理类型.
        /// </summary>
        private ProcessType processType = ProcessType.None;

        /// <summary>
        /// 构造函数(需要记录日志，并且是记录入参、出参数日志)
        /// </summary>
        /// <param name="adviceType"></param>
        public IankaMethodAttribute()
            : this(ProcessType.Log, AdviceType.Around)
        {
        }

        /// <summary>
        /// 构造函数(自定义设置是否需要记录日志，如果需要记录那么：记录入参、出参数日志)
        /// </summary>
        /// <param name="adviceType"></param>
        public IankaMethodAttribute(ProcessType processType)
            : this(processType, AdviceType.Around)
        {
        }

        /// <summary>
        /// 构造函数（自定义设置是否需要记录日志，并且记录日志的方式也自定义设置）
        /// </summary>
        /// <param name="adviceType">代理处理类型.</param>
        public IankaMethodAttribute(ProcessType processType, AdviceType adviceType)
        {
            this.processType = processType;
            this.adviceType = adviceType;
        }

        /// <summary>
        /// 代理处理方式.
        /// </summary>
        public AdviceType AdviceType
        {
            get { return this.adviceType; }
        }

        /// <summary>
        /// 代理处理类型.
        /// </summary>
        public ProcessType ProcessType
        {
            get { return this.processType; }
        }
    }
}
