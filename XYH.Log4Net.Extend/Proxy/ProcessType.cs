/**********************************************************************************
 * 类 名 称： ProcessType
 * 机器名称： ProcessType.cs
 * 命名空间： XYH.Log4Net.Extend
 * 文 件 名： ProcessType
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
    /// 代理处理类型.
    /// </summary>
    public enum ProcessType
    {
        /// <summary>
        /// 不需要处理.
        /// </summary>
        None = 0,

        /// <summary>
        /// 记录日志.
        /// </summary>
        Log = 1,
    }
}
