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
    /// 请求参数集合模型
    /// </summary>
    public class MRequestParameter
    {
        /// <summary>
        /// 请求参数类型 
        /// 1：get 方式提交的参数
        /// 2：post 方式提交的参数
        /// </summary>
        public int pushType { get; set; }

        /// <summary>
        /// 请求参数类型 
        /// 1：get 方式提交的参数
        /// 2：post 方式提交的参数
        /// </summary>
        public string pushTypeDes { get; set; }

        /// <summary>
        /// 参数键
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string value { get; set; }
    }
}