using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace XYH.Log4Net.Extend.Proxy
{
    /// <summary>
    /// Action过滤器
    /// </summary>
    public class CustomActionFilterAttribute : ActionFilterAttribute
    {
        
        /// <summary>
        /// Action执行开始
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            HttpContext.Current.Request.Headers.Set("ActionStartTime", DateToTicks(System.DateTime.Now).ToString());
        }

        /// <summary>
        /// action执行以后
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionContext)
        {
            try
            {
                // 获取 startTimeStr
                DateTime startTime = System.DateTime.Now;
                string startTimeStr = HttpContext.Current.Request.Headers.GetValues("ActionStartTime") == null ? string.Empty :
                              HttpContext.Current.Request.Headers.GetValues("ActionStartTime")[0];
                if (!string.IsNullOrEmpty(startTimeStr))
                {
                    startTime = TicksToDate(startTimeStr);
                }

                // 返回信息
                var objectContent = actionContext.Response.Content as ObjectContent;
                var returnValue = objectContent.Value;
               
                XYHLogOperator.WriteLog(new LogMessage()
                {
                    MethodName = actionContext.Request.RequestUri.AbsolutePath,
                    MethodParam = actionContext.Request.RequestUri.Query,
                    MethodResult = returnValue.ToString(),
                    LogProjectName = actionContext.Request.RequestUri.AbsoluteUri,
                    ExecuteEndTime = System.DateTime.Now,
                    ExecuteStartTime = startTime,
                    Level = LogLevel.Info
                });
            }
            catch (Exception ex)
            {

            }
        }

        //DateTime类型转换为时间戳(毫秒值)
        public long DateToTicks(DateTime? time)
        {
            return ((time.HasValue ? time.Value.Ticks : DateTime.Parse("1990-01-01").Ticks) - 621355968000000000) / 10000;
        }

        //时间戳(毫秒值)String转换为DateTime类型转换
        public DateTime TicksToDate(string time)
        {
            return new DateTime((Convert.ToInt64(time) * 10000) + 621355968000000000);
        }
    }
}
