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
    /// http请求操作
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// 获取请求的所有参数键值对集合
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns>所有参数键值对字典集合</returns>
        public static List<MRequestParameter> GetAllQueryParameters(this HttpRequest request)
        {
            // 请求参数字典集合
            List<MRequestParameter> queryParameters = null;

            // 获取url请求参数(get提交的参数)
            if (request.QueryString.HasValue)
            {
                // 如果参数存储对象为空，那么初始化一次
                queryParameters = queryParameters ?? new List<MRequestParameter>();

                foreach (string key in request.Query.Keys)
                {
                    if (!queryParameters.Exists(x => x.key == key))
                    {
                        queryParameters.Add(new MRequestParameter()
                        {
                            key = key,
                            value = request.Query[key],
                            pushType = 1,
                            pushTypeDes = "get"
                        });
                    }
                }
            }


            // 获取表单参数(post)
            if (request.Method.ToUpper() == "POST")
            {
                try
                {
                    if (request.Form != null && request.Form.Keys != null && request.Form.Keys.Count > 0)
                    {
                        // 如果参数存储对象为空，那么初始化一次
                        queryParameters = queryParameters ?? new List<MRequestParameter>();

                        foreach (string key in request.Form.Keys)
                        {
                            if (!queryParameters.Exists(x => x.key == key))
                            {
                                queryParameters.Add(new MRequestParameter()
                                {
                                    key = key,
                                    value = request.Form[key],
                                    pushType = 2,
                                    pushTypeDes = "post"
                                });
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return queryParameters;
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <returns>ip</returns>
        public static string GetIPAddress(this HttpContext context)
        {
            var ip = string.Empty;
            try
            {
                ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (string.IsNullOrEmpty(ip))
                {
                    ip = context.Connection.RemoteIpAddress.ToString();
                }

                if (ip == "::1")
                {
                    ip = "127.0.0.1";
                }

                if (string.IsNullOrEmpty(ip) || !IsIP(ip))
                {
                    ip = "127.0.0.1";
                }
            }
            catch (Exception)
            {
                ip = "127.0.0.1";
            }

            return ip;
        }

        /// <summary>
        /// 检查IP地址格式 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns>检查结果</returns>
        private static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 获取请求响应数据
        /// </summary>
        /// <param name="response">请求响应对象</param>
        /// <returns>请求响应数据</returns>
        public static string GetResponseValue(this HttpResponse response)
        {
            try
            {
                if (response.Body.Length > 0)
                {
                    response.Body.Seek(0, SeekOrigin.Begin);
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var retStr = ReadStreamAsync(response.Body, Encoding.GetEncoding("UTF-8"), false).ConfigureAwait(false);
                    return retStr.ToString();
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// ReadStreamAsync
        /// </summary>
        /// <param name="stream">stream</param>
        /// <param name="encoding">encoding</param>
        /// <param name="forceSeekBeginZero">forceSeekBeginZero</param>
        /// <returns>Task</returns>
        private static async Task<string> ReadStreamAsync(Stream stream, Encoding encoding, bool forceSeekBeginZero = true)
        {
            using (StreamReader sr = new StreamReader(stream, encoding, true, 1024, true))//这里注意Body部分不能随StreamReader一起释放
            {
                var str = await sr.ReadToEndAsync();
                if (forceSeekBeginZero)
                {
                    stream.Seek(0, SeekOrigin.Begin);//内容读取完成后需要将当前位置初始化，否则后面的InputFormatter会无法读取
                }
                return str;
            }
        }
    }
}