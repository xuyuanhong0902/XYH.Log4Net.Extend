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
        /// ÿһ������ִ�п�ʼ
        /// </summary>
        protected void Session_Start()
        {
            //// ��¼��ȡ����ÿһ����������к�
            /// ������÷Ŵ��������кţ���ô��ֱ��ȥ���÷Ŵ��ݵ����к�
            /// ������÷�δ���ݣ���ô������һ�����к�
            /// ��������һ�������ͷ������һ���������Ψһ���кţ������Ժ��ÿһ������һֱ������ȥ
            /// ���������ܹ�ͨ��������кŰ�ÿһ������֮��ķ�����߷������ù�ϵ��������
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

                //// ��¼�쳣��־
                XYHLogOperator.WriteLog("ȫ���쳣����", baseException);
                //// WriteLog(baseException);

                if (baseException is HttpRequestValidationException)
                {
                    Response.Write("�벻Ҫ�����ң��Һܴ����ģ�����");
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
