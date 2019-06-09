/**********************************************************************************
 * 类 名 称： AdviceType
 * 机器名称： AdviceType.cs
 * 命名空间： XYH.Log4Net.Extend
 * 文 件 名： AdviceType
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

namespace XYH.Log4Net.Extend
{
    /// <summary>
    /// 代理处理方式.
    /// </summary>
    public enum AdviceType
    {
        /// <summary>
        /// 不需要处理
        /// </summary>
        None = 0,

        /// <summary>
        /// 调用方法前处理
        /// </summary>
        Before = 1,

        /// <summary>
        /// 监控执行全过程
        /// </summary>
        Around = 2
    }
}
