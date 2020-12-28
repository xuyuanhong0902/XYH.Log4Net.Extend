using HS.Public.Tools.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYH.Log4Net.Extend;

namespace XYH.Log4Net.Extend.Standard
{
    /// <summary>
    /// 自定义用户请求监控管道
    /// 监控每一次请求，并且根据实际情况做相应的日志记录等操作
    /// </summary>
    public class XYHLog4ActionFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 重写 OnResultExecuting
        /// </summary>
        /// <param name="context">context</param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // 当接口只有添加了记录日志注解才记录日志 
            var isDefined = false;
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                isDefined = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                  .Any(a => a.GetType().Equals(typeof(XYHLog4Attribute)));
            }
            if (!isDefined) return;

            // 记录是否

            HttpContext httpContext = context.HttpContext;

            // 构建一个日志数据模型
            DateTime startTime = Tools.GetSysDateTimeNow();
            DateTime.TryParse(httpContext.Request.Headers["startTime"], out startTime);
            MApiRequestLogs apiRequestLogsM = new MApiRequestLogs()
            {
                REQUEST_TIME = startTime,
                RESPONSE_TIME = Tools.GetSysDateTimeNow()
            };

            string serialNumber = string.Empty;
            try
            {
                // 由于.net core大部分项目都是用作分布式微服务架构，为了讲每一次请求的所有日志都有序的串联起来，那么在请求开始的同步都传递一个随机的日志序列号，并一直传递下去，如果没有检查到日志序列号，那么就在请求头部初始化一个日志序列号
                if (httpContext.Request.Headers == null
                    || httpContext.Request.Headers.Count < 1
                    || !httpContext.Request.Headers.ContainsKey("serialNumber")
                    || string.IsNullOrEmpty(httpContext.Request.Headers["serialNumber"]))
                {
                    serialNumber = Tools.GetDateRandomString();
                }
                else
                {
                    serialNumber = httpContext.Request.Headers["serialNumber"];
                }

                // 1、统计总共执行时间
                apiRequestLogsM.TOTALMILLISECONDS = apiRequestLogsM.RESPONSE_TIME.Subtract(apiRequestLogsM.REQUEST_TIME).Milliseconds;

                // 2、根据请求报文，获取对于的请求相关信息：入参、api、日志序列号等系信息

                // IP地址
                apiRequestLogsM.IP = httpContext.GetIPAddress();

                // 请求的完整URL地址
                apiRequestLogsM.URL = $"{httpContext.Request.Host.Value}{httpContext.Request.Path.Value}";

                // 请求的相对地址（API）
                // 改数据不是那么准确，因为用有可能会带有参数路径
                apiRequestLogsM.API = httpContext.Request.Path.Value;

                // 请求入参
                // 获取所有请求参数
                List<MRequestParameter> allQueryParameters = httpContext.Request.GetAllQueryParameters();
                apiRequestLogsM.REQUEST_INFOR = allQueryParameters == null ? string.Empty : JsonConvert.SerializeObject(allQueryParameters);

                // 请求大小
                apiRequestLogsM.CENTENTLENGTH = httpContext.Request.ContentLength;

                // ContentType
                apiRequestLogsM.CENTENTTYPE = httpContext.Request.ContentType;

                // 请求头部信息
                apiRequestLogsM.HEADERS = JsonConvert.SerializeObject(httpContext.Request.Headers);

                // 请求 Method
                apiRequestLogsM.METHOD = httpContext.Request.Method;

                // 返回信息
                var objectContent = context.Result;
                apiRequestLogsM.RESPONSE_INFOR = objectContent.ToString();

                // 4、落地一条日志信息
                LogMessage extendLogInfor = new LogMessage()
                {
                    ExecuteEndTime = apiRequestLogsM.REQUEST_TIME,
                    ExecuteStartTime = apiRequestLogsM.RESPONSE_TIME,
                    InvokeName = apiRequestLogsM.METHOD,
                    IpAddress = apiRequestLogsM.IP,

                    Level = LogLevel.Debug,
                    LogMachineCode = Dns.GetHostName(),
                    LogProjectName = apiRequestLogsM.API,
                    LogRecordTime = System.DateTime.Now,

                    LogSerialNumber = serialNumber,
                    LogUniqueCode = Guid.NewGuid().ToString().Replace("-", "").ToUpper(),
                    MethodName = apiRequestLogsM.METHOD,
                    Message = JsonConvert.SerializeObject(apiRequestLogsM),

                    MethodParam = apiRequestLogsM.REQUEST_INFOR,
                    MethodResult = apiRequestLogsM.RESPONSE_INFOR
                };

                // 落地一条文本日志
                XYHLogOperator.WriteLog(extendLogInfor);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Action执行开始
        /// </summary>
        /// <param name="context">请求上下文</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 为了便于记录交互日志，在每一个Action执行开始时，在请求的头部追加一个执行请求时间
            context.HttpContext.Request.Headers.Add("startTime", Tools.GetSysDateTimeNow().ToString("yyyy-MM-dd HH:mm:ss ffffff"));

            // 由于.net core大部分项目都是用作分布式微服务架构，为了讲每一次请求的所有日志都有序的串联起来，那么在请求开始的同步都传递一个随机的日志序列号，并一直传递下去，如果没有检查到日志序列号，那么就在请求头部初始化一个日志序列号
            if (context.HttpContext.Request.Headers == null
                  || context.HttpContext.Request.Headers.Count < 1
                  || !context.HttpContext.Request.Headers.ContainsKey("serialNumber")
                  || string.IsNullOrEmpty(context.HttpContext.Request.Headers["serialNumber"]))
            {
                context.HttpContext.Request.Headers.Add("serialNumber", Tools.GetDateRandomString());
            }
        }
    }
}