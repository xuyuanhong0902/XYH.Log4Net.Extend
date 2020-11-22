using log4net.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace XYH.Log4Net.Extend
{
    /// <summary>
    /// 通过队列的方式实现异步记录日志
    /// </summary>
    public sealed class ExtendLogQueue
    {
        /// <summary>
        /// 记录消息 队列
        /// </summary>
        private readonly ConcurrentQueue<LogMessage> extendLogQue;

        /// <summary>
        /// 信号
        /// </summary>
        private readonly ManualResetEvent extendLogMre;

        /// <summary>
        /// 日志
        /// </summary>
        private static ExtendLogQueue _flashLog = new ExtendLogQueue();

        /// <summary>
        /// 队里消费启动线程 （记录日志）
        /// </summary>
        Thread consumeThread = null;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        // LogInfoFileAppender 日志最大保留个数
        private static int logInfoMaximumFiles;

        /// <summary>
        /// LogInfoFileAppender 日志路径
        /// </summary>
        private static string logInfoFilePath;

        // LogErrorFileAppender 日志最大保留个数
        private static int errorInfoMaximumFiles;

        /// <summary>
        /// LogErrorFileAppender 日志路径
        /// </summary>
        private static string logErrorFilePath;

        /// <summary>
        /// 构造函数
        /// </summary>
        private ExtendLogQueue()
        {
            extendLogQue = new ConcurrentQueue<LogMessage>();
            extendLogMre = new ManualResetEvent(false);

            // 初始化数据
            InitData();
        }

        /// <summary>
        /// 单例实例
        /// </summary>
        /// <returns></returns>
        public static ExtendLogQueue Instance()
        {
            return _flashLog;
        }

        /// <summary>
        /// 另一个线程记录日志，只在程序初始化时调用一次
        /// </summary>
        public void Register()
        {
            //// 如果为空，那么就加锁，创建实例
            if (consumeThread == null)
            {
                lock (locker)
                {
                    //// 枷锁成功后，在做一次非空判断，避免在加锁期间以创建了实例而导致重复创建
                    if (consumeThread == null)
                    {
                        consumeThread = new Thread(new ThreadStart(WriteLogDispatch));
                        consumeThread.IsBackground = false;
                        consumeThread.Start();
                    }
                }
            }
        }

        /// <summary>
        /// 从队列中写日志至磁盘
        /// </summary>
        private void WriteLogDispatch()
        {
            while (true)
            {

                // 如果队列中还有待写日志，那么直接调用写日志
                if (extendLogQue.Count > 0)
                {
                    // 根据队列写日志
                    WriteLog();

                    // 重新设置信号
                    extendLogMre.Reset();
                }

                // 每次写完一次日志后，先根据配置的日志文件最大个数，所一个历史日志清理检测


                // 如果没有，那么等待信号通知
                extendLogMre.WaitOne();
            }
        }

        /// <summary>
        /// 具体调用log4日志组件实现
        /// </summary>
        private void WriteLog()
        {
            LogMessage msg;
            // 判断是否有内容需要如磁盘 从列队中获取内容，并删除列队中的内容
            while (extendLogQue.Count > 0 && extendLogQue.TryDequeue(out msg))
            {
                new LogHandlerImpl(LogHandlerManager.GetILogger(msg.LogSerialNumber)).WriteLog(msg);
            }
        }

        /// <summary>
        /// 日志入队列
        /// </summary>
        /// <param name="message">日志文本</param>
        /// <param name="level">等级</param>
        /// <param name="ex">Exception</param>
        public void EnqueueMessage(LogMessage logMessage)
        {
            //// 日志入队列
            extendLogQue.Enqueue(logMessage);

            // 通知线程往磁盘中写日志
            extendLogMre.Set();

            // 尝试开启消费线程
            Register();

            // 尝试删除历史日志
            if (logMessage.Level==LogLevel.Error)
            {
                DeleteHistoryLogs(logErrorFilePath, errorInfoMaximumFiles);
            }
            else
            {
                DeleteHistoryLogs(logInfoFilePath, logInfoMaximumFiles);
            }
        }

        /// <summary>
        /// 根据节点关系获取XML节点对应的Value属性值
        /// </summary>
        /// <param name="listNodes">node集合</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="childNodeName">下一节点名称</param>
        /// <returns>获取到的值</returns>
        private string RecursionGetXmlNodeValue(XmlNodeList listNodes, string nodeName, string childNodeName)
        {
            foreach (XmlNode node in listNodes)
            {
                if (node.Attributes != null &&
                    node.Attributes["name"] != null &&
                    node.Attributes["name"].Value == nodeName)
                {
                    // childNodeName 为空，代表已经是最底层节点
                    if (string.IsNullOrEmpty(childNodeName))
                    {
                        return node.Attributes["value"] == null ? string.Empty : node.Attributes["value"].Value;
                    }

                    // 递归子节点
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        return RecursionGetXmlNodeValue(node.ChildNodes, childNodeName, string.Empty);
                    }

                    return string.Empty;
                }
            }

            return string.Empty;
        }

        /// <summary>
        ///  初始化数据
        /// </summary>
        private void InitData()
        {

            // 获取配置文件对应的每一种日志的最大文件个数
            string path = HttpRuntime.AppDomainAppPath + "\\log4net.config";

            XmlDocument doc = new XmlDocument();

            doc.Load(path);

            XmlElement root = null;
            root = doc.DocumentElement;

            XmlNodeList listNodes = null;
            listNodes = root.SelectNodes("/log4net/appender");

            // LogInfoFileAppender 日志最大保留个数
            int.TryParse(RecursionGetXmlNodeValue(listNodes, "LogInfoFileAppender", "ExtendMaximumFiles"), out logInfoMaximumFiles);
            int.TryParse(RecursionGetXmlNodeValue(listNodes, "LogErrorFileAppender", "ExtendMaximumFiles"), out errorInfoMaximumFiles);

            logInfoFilePath = RecursionGetXmlNodeValue(listNodes, "LogInfoFileAppender", "File");
            logErrorFilePath = RecursionGetXmlNodeValue(listNodes, "LogErrorFileAppender", "File");
           
            // 如果是相对路径，那么需要跟上绝对路径
            if (!logInfoFilePath.Contains(":"))
            {
                logInfoFilePath = HttpRuntime.AppDomainAppPath + "\\"+ logInfoFilePath;
            }

            if (!logErrorFilePath.Contains(":"))
            {
                logErrorFilePath = HttpRuntime.AppDomainAppPath + "\\" + logErrorFilePath;
            }
        }

        /// <summary>
        /// 删除历史日志数据
        /// </summary>
        /// <param name="filePath">日志文件路径</param>
        /// <param name="maximumFiles">最多保留日志数</param>
        private void DeleteHistoryLogs(string filePath,int maximumFiles)
        {
            // 如果文件个数不限制，那么直接返回
            if (maximumFiles < 1)
            {
                return;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            DirectoryInfo dicInfo = new DirectoryInfo(filePath);
            if (dicInfo.Exists)
            {
                // 获取全部文件
                FileInfo[] infos = dicInfo.GetFiles();

                if (infos!=null && infos.Length> maximumFiles)
                {
                    List<FileInfo> infosList = infos.ToList();
                    infosList.Sort((a, b) => b.LastWriteTime.CompareTo(a.LastWriteTime));
                    for (int i = maximumFiles; i < infosList.Count; i++)
                    {
                        infosList[i].Delete();
                    }
                }
            }
        }
    }
}