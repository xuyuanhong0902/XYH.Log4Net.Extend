using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  XYH.Log4Net.Extend.Standard
{
    /// <summary>
    /// 日志模板信息设置
    /// </summary>
    public class MLogTemplateSet
    {
        /// <summary>
        /// 日志模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 日志相对路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 扩展一个节点：最大日志文件总个数，当日志文件大于该值时，组件自动删除最开始创建的文件
        /// </summary>
        public int ExtendMaximumFiles { get; set; }

        /// <summary>
        /// 日志类型最小枚举值
        /// </summary>
        public LogLevel FilterLevelMin { get; set; }

        /// <summary>
        /// 日志类型最大枚举值
        /// </summary>
        public LogLevel FilterLevelMax { get; set; }
    }
}
