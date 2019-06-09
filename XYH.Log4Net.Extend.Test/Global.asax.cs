using XYH.Log4Net.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LogOperationTest
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ////ע����־����
            ExtendLogQueue.Instance().Register();
        }

        /// <summary>
        /// ÿһ������ִ�п�ʼ
        /// </summary>
        protected void Session_Start() {
            //// ��¼��ȡ����ÿһ����������к�
            /// ������÷Ŵ��������кţ���ô��ֱ��ȥ���÷Ŵ��ݵ����к�
            /// ������÷�δ���ݣ���ô������һ�����к�
            /// ��������һ�������ͷ������һ���������Ψһ���кţ������Ժ��ÿһ������һֱ������ȥ
            /// ���������ܹ�ͨ��������кŰ�ÿһ������֮��ķ�����߷������ù�ϵ��������
            String[] serialNumber = Request.Headers.GetValues("serialNumber");
            if (serialNumber!=null && serialNumber.Length>0 && !string.IsNullOrEmpty(serialNumber[0]))
            {
                Session["LogSerialNumber"] = serialNumber[0];
            }
            else
            {
                Session["LogSerialNumber"] = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            }
        }
    }
}
