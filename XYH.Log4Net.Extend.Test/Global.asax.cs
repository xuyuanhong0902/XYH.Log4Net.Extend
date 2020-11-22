using XYH.Log4Net.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
namespace LogOperationTest
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //   FilterConfig.RegisterGlobalFilters(XYHAopAttribute);
        }

        /// <summary>
        /// 每一个请求执行开始
        /// </summary>
        protected void Session_Start()
        {
            //// 记录获取创建每一个请求的序列号
            /// 如果调用放传递了序列号，那么就直接去调用放传递的序列号
            /// 如果调用放未传递，那么则生成一个序列号
            /// 这样，在一次请求的头部传递一个该请求的唯一序列号，并在以后的每一个请求都一直传递下去
            /// 这样，就能够通过这个序列号把每一次请求之间的服务或者方法调用关系串联起来
            String[] serialNumber = Request.Headers.GetValues("serialNumber");
            if (serialNumber != null && serialNumber.Length > 0 && !string.IsNullOrEmpty(serialNumber[0]))
            {
                Session["LogSerialNumber"] = serialNumber[0];
            }
            else
            {
                Session["LogSerialNumber"] = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            }
        }

        /// <summary>
        /// Application_Error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Exception lastException = Server.GetLastError();
                if (lastException == null)
                {
                    return;
                }

                Exception baseException = lastException.GetBaseException();
                if (baseException == null)
                {
                    return;
                }

                //// 记录异常日志
                XYHLogOperator.WriteLog("全局异常捕获", baseException);
                //// WriteLog(baseException);

                if (baseException is HttpRequestValidationException)
                {
                    Response.Write("请不要攻击我，我很脆弱的！！！");
                }
            }
            finally
            {
                if (System.Configuration.ConfigurationManager.AppSettings["ShowErrorOnPage"] == null)
                {
                    Server.ClearError();
                }
            }
        }
    }
}
