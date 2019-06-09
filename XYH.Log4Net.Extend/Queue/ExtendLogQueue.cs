using log4net.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        /// 构造函数
        /// </summary>
        private ExtendLogQueue()
        {
            extendLogQue = new ConcurrentQueue<LogMessage>();
            extendLogMre = new ManualResetEvent(false);
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
            Thread t = new Thread(new ThreadStart(WriteLogDispatch));
            t.IsBackground = false;
            t.Start();
        }

        /// <summary>
        /// 从队列中写日志至磁盘
        /// </summary>
        private void WriteLogDispatch()
        {
            while (true)
            {

                //// 如果队列中还有待写日志，那么直接调用写日志
                if (extendLogQue.Count > 0)
                {
                    //// 根据队列写日志
                    WriteLog();

                    // 重新设置信号
                    extendLogMre.Reset();
                }

                //// 如果没有，那么等待信号通知
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
        public  void EnqueueMessage(LogMessage logMessage)
        {
            //// 日志入队列
            extendLogQue.Enqueue(logMessage);

            // 通知线程往磁盘中写日志
            extendLogMre.Set();
        }
    }
}
