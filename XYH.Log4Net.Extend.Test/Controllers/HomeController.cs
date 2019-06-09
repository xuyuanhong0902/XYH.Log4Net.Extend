using XYH.Log4Net.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogOperationTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            object message = "一个参数日志记录单元测试"; // TODO: 初始化为适当的值
            XYHLogOperator.WriteLog(message);
            Class2 calssAdd = new Class2();
            //calssAdd calssAdd = new calssAdd();
            calssAdd.SubNum(1, 22);
            calssAdd.AddNum(1,11);
           
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}