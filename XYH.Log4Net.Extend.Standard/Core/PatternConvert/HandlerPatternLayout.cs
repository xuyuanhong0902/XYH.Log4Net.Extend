using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYH.Log4Net.Extend.Standard
{
    /// <summary>
    /// 自定义布局（对log2net日志组件的布局自定义扩展）.
    /// </summary>
    public class HandlerPatternLayout : PatternLayout
    {
        /// <summary>
        /// 构造函数.
        /// </summary>
        public HandlerPatternLayout()
        {
            ///// 机器名称
            this.AddConverter("LogMachineCode", typeof(LogMachineCodePatternConvert));

            //// 方法名称
            this.AddConverter("MethodName", typeof(LogMethodNamePatternConvert));

            //// 方法入参
            this.AddConverter("MethodParam", typeof(LogMethodParamConvert));

            //// 方法出参
            this.AddConverter("MethodResult", typeof(LogMethodResultConvert));

            //// 程序名称
            this.AddConverter("LogProjectName", typeof(LogProjectNamePatternConvert));

            //// IP 地 址
            this.AddConverter("LogIpAddress", typeof(LogServiceIpPatternConvert));

            //// 日志编号
            this.AddConverter("LogUniqueCode", typeof(LogUniquePatternConvert));

            //// 日志序列号
            this.AddConverter("LogSerialNumber", typeof(LogSerialNumberPatternConvert));

            //// 调用路径
            this.AddConverter("InvokeName", typeof(LogInvokeNamePatternConvert));

            //// 执行开始时间
            this.AddConverter("ExecuteStartTime", typeof(ExecuteStartTimePatternConvert));

            //// 执行结束时间
            this.AddConverter("ExecuteEndTime", typeof(ExecuteEndTimePatternConvert));

            //// 执行时间
            this.AddConverter("ExecuteTime", typeof(ExecuteTimePatternConvert));
        }
    }
}
