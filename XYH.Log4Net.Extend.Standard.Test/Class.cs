using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XYH.Log4Net.Extend.Standard.Test
{
    /// <summary>
    /// 自定义用户请求监控管道
    /// 监控每一次请求，并且根据实际情况做相应的日志记录等操作
    /// </summary>
    public class xxxActionFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 重写 OnResultExecuting
        /// </summary>
        /// <param name="context">context</param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            string ss2 = "3455";
        }

        /// <summary>
        /// Action执行开始
        /// </summary>
        /// <param name="context">请求上下文</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string ss1 ="333";
        }
    }
}
