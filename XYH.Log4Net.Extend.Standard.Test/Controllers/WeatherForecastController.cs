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

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
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
    }
}
