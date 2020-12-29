using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XYH.Log4Net.Extend.Standard.Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [XYHLog4Attribute]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Get1")]
        [XYHLog4]
        public IEnumerable<WeatherForecast> Get1()
        {

            for (int i = 0; i < 10; i++)
            {
                XYHLogOperator.WriteLog("新增一条功能模块记录", "ModuleService/AddOneModule", "dsdsdjsdhskh", "ddsds", LogLevel.Info);

                XYHLogOperator.WriteLog("新增一条功能模块记录,系统异常", "ModuleService/AddOneModule", "dsdsdjsdhskh", "ddsds", LogLevel.Error);

                XYHLogOperator.WriteLog(new LogMessage()
                {
                    MethodName = "1",
                    MethodParam = "11",
                    MethodResult = "111",
                    LogProjectName = "1111",
                    ExecuteEndTime = System.DateTime.Now,
                    ExecuteStartTime = System.DateTime.Now.AddMilliseconds(-1),
                    Level = LogLevel.Info,
                    LogContent="33"
                });
            }

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [Route("Get2")]
        [HttpPost]
        [XYHLog4]
        public bb Get2(aa a)
        {
            // 接收
           // System.IO.Stream inputstream = HttpContext.Current.Request.InputStream;

            //System.IO.Stream inputstream = this.Request.InputStream;
            //byte[] b = new byte[inputstream.Length];
            //inputstream.Read(b, 0, (int)inputstream.Length);
            //string inputstr = System.Text.UTF8Encoding.UTF8.GetString(b);

            return new bb()
            {
                name="ces"
            };
        }

        [HttpGet("Get3")]
        [XYHNoLog4]
        public string Get3(aa a)
        {
            return "我是测试";
        }
    }

    public class aa { 
   public string id { get; set; }
    }

    public class bb
    {
        public string name { get; set; }
    }
}
