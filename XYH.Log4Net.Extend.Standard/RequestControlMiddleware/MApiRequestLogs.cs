using HS.Public.Tools;
using HS.Public.Tools.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYH.Log4Net.Extend;

namespace XYH.Log4Net.Extend.Standard
{
    /// <summary>
    /// API请求日志
    /// </summary>
    public class MApiRequestLogs
    {
        /// <summary>
        /// 主键ID 
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 请求IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Token 
        /// </summary>
        public string TOKEN { get; set; }

        /// <summary>
        /// 请求的域名地址 
        /// </summary>
        public string HOST { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 请求的API接口
        /// </summary>
        public string API { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string REQUEST_INFOR { get; set; }

        /// <summary>
        /// 请求大小
        /// </summary>
        public long? CENTENTLENGTH { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>
        public string CENTENTTYPE { get; set; }

        /// <summary>
        /// 请求头部信息
        /// </summary>
        public string HEADERS { get; set; }

        /// <summary>
        /// 请求Method
        /// </summary>
        public string METHOD { get; set; }

        /// <summary>
        /// 请求返回值
        /// </summary>
        public string RESPONSE_INFOR { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime REQUEST_TIME { get; set; }

        /// <summary>
        /// 返回时间
        /// </summary>
        public DateTime RESPONSE_TIME { get; set; }

        /// <summary>
        /// 总共处理时间 单位毫秒
        /// </summary>
        public long TOTALMILLISECONDS { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CREAT_TIME { get; set; }

        /// <summary>
        /// 日志类型
        /// Debug = 0,
        /// Info = 1,
        /// Warn = 2,
        /// Error = 3,
        /// Fatal = 4
        /// </summary>
        public LogLevel LOGLEVEL { get; set; }
    }
}