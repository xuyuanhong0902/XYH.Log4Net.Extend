using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace XYH.Log4Net.Extend.Test.Controllers
{
    public class ApiTestController : ApiController
    {
        /// <summary>
        /// 获取所有资源数据 
        /// </summary>
        /// <returns>所有资源数据</returns>
        [HttpGet]
        [XYHMethod(ProcessType.Log)]
        public string GetResource()
        {
            return "333";
        }
    }
}
