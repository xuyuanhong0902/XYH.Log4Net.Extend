using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XYH.Log4Net.Extend.Test.Controllers
{
    public class DefaultController : Controller
    {
        [XYHMethod(ProcessType.Log)]
        public ActionResult Index()
        {
            return View();
        }
    }
}