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
                apiRequestLogsM.TOTALMILLISECONDS = apiRequestLogsM.REQUEST_TIME.Subtract(apiRequestLogsM.RESPONSE_TIME).Milliseconds;

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
                apiRequestLogsM.REQUEST_INFOR = $"{GetAllQueryParameters(httpContext.Request)}{httpContext.Request.Headers["requestParameters"]}";

                //List<MRequestParameter> allQueryParameters = httpContext.Request.GetAllQueryParameters();
                //apiRequestLogsM.REQUEST_INFOR = allQueryParameters == null ? string.Empty : JsonConvert.SerializeObject(allQueryParameters);
                //从文件流中读取传递测参数
                //using (var ms = new MemoryStream())
                //{
                //    context.HttpContext.Request.Body.Seek(0, 0);//将读取指针迻到开始位置
                //    context.HttpContext.Request.Body.CopyTo(ms);
                //    var b = ms.ToArray();
                //    apiRequestLogsM.REQUEST_INFOR = Encoding.UTF8.GetString(b);
                //}

                //// 接收
                //Stream inputstream = HttpContext.Current.Request.InputStream;

                //Stream inputstream = context.HttpContext.Request.InputStream;
                //byte[] b = new byte[inputstream.Length];
                //inputstream.Read(b, 0, (int)inputstream.Length);
                //string inputstr = UTF8Encoding.UTF8.GetString(b);


                // 请求大小
                apiRequestLogsM.CENTENTLENGTH = httpContext.Request.ContentLength;

                // ContentType
                apiRequestLogsM.CENTENTTYPE = httpContext.Request.ContentType;

                // 请求头部信息
                apiRequestLogsM.HEADERS = JsonConvert.SerializeObject(httpContext.Request.Headers);

                // 请求 Method
                apiRequestLogsM.METHOD = httpContext.Request.Method;

                // 返回信息
                if (context.Result is ObjectResult)
                {
                    var objectResult = context.Result as ObjectResult;
                    if (objectResult.Value != null)
                    {
                        apiRequestLogsM.RESPONSE_INFOR = JsonConvert.SerializeObject(objectResult.Value);
                    }
                }

                // 4、落地一条日志信息
                LogMessage extendLogInfor = new LogMessage()
                {
                    ExecuteEndTime = apiRequestLogsM.REQUEST_TIME,
                    ExecuteStartTime = apiRequestLogsM.RESPONSE_TIME,
                    InvokeName = apiRequestLogsM.METHOD,
                    IpAddress = apiRequestLogsM.IP,

                    Level = LogLevel.Debug,
                    LogMachineCode = Dns.GetHostName(),
                    LogProjectName = apiRequestLogsM.URL,
                    LogRecordTime = Tools.GetSysDateTimeNow(),

                    LogSerialNumber = serialNumber,
                    LogUniqueCode = Guid.NewGuid().ToString().Replace("-", "").ToUpper(),
                    MethodName = apiRequestLogsM.API,
                    //  Message = JsonConvert.SerializeObject(apiRequestLogsM),

                    MethodParam = apiRequestLogsM.REQUEST_INFOR,
                    MethodResult = apiRequestLogsM.RESPONSE_INFOR
                };

                // 落地一条文本日志
                XYHLogOperator.WriteLog(extendLogInfor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Action执行开始
        /// </summary>
        /// <param name="context">请求上下文</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isDefined = false;
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                isDefined = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                  .Any(a => a.GetType().Equals(typeof(XYHLog4Attribute)));
            }
            if (!isDefined) return;


            // 为了便于记录交互日志，在每一个Action执行开始时，在请求的头部追加一个执行请求时间
            context.HttpContext.Request.Headers.Add("startTime", Tools.GetSysDateTimeNow().ToString("yyyy-MM-dd HH:mm:ss"));

            // 由于.net core大部分项目都是用作分布式微服务架构，为了讲每一次请求的所有日志都有序的串联起来，那么在请求开始的同步都传递一个随机的日志序列号，并一直传递下去，如果没有检查到日志序列号，那么就在请求头部初始化一个日志序列号
            if (context.HttpContext.Request.Headers == null
                  || context.HttpContext.Request.Headers.Count < 1
                  || !context.HttpContext.Request.Headers.ContainsKey("serialNumber")
                  || string.IsNullOrEmpty(context.HttpContext.Request.Headers["serialNumber"]))
            {
                context.HttpContext.Request.Headers.Add("serialNumber", Tools.GetDateRandomString());
            }

            // 把方法入参保存起来，在action结束时，记日志使用 requestParameters
            if (context.ActionArguments != null)
            {
                context.HttpContext.Request.Headers.Add("requestParameters", JsonConvert.SerializeObject(context.ActionArguments));
            }
        }

        /// <summary>
        /// 获取请求的所有参数键值对集合
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns>所有参数键值对字典集合</returns>
        public string GetAllQueryParameters(HttpRequest request)
        {
            // 请求参数字典集合
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();

            try
            {
                if (request.Query.Keys != null)
                {
                    // 获取url请求参数
                    foreach (string key in request.Query.Keys)
                    {
                        if (!queryParameters.ContainsKey(key))
                        {
                            queryParameters.Add(key, request.Query[key]);
                        }
                    }
                }

                if (request.Form.Keys != null)
                {
                    // 获取表单参数
                    foreach (string key in request.Form.Keys)
                    {
                        if (!queryParameters.ContainsKey(key))
                        {
                            queryParameters.Add(key, request.Form[key]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            if (queryParameters.Count>0)
            {
                return JsonConvert.SerializeObject(queryParameters);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}