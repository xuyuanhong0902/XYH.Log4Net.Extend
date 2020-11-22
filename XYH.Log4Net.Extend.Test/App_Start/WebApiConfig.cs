using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using XYH.Log4Net.Extend.Proxy;

namespace LogOperationTest
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();
           config.Filters.Add(new CustomActionFilterAttribute());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //GlobalConfiguration.Configuration.
            // Formatters.JsonFormatter.SerializerSettings.Converters.Add(new IsoDateTimeConverter
            // {
            //     DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss"
            // });
        }
    }
}
