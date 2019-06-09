背景：

　　随着公司的项目不断的完善，功能越来越复杂，服务也越来越多（微服务），公司迫切需要对整个系统的每一个程序的运行情况进行监控，并且能够实现对自动记录不同服务间的程序调用的交互日志，以及通一个服务或者项目中某一次执行情况的跟踪监控

       根据log4net的现有功能满足不了实际需求，所以需要以log4net为基础进行分装完善，现在分装出了一个基础的版本，如有不妥之处，多多指点
功能简介：
　　该组件是在log4net的基础上，进行了一定的扩展封装实现的自动记录交互日志功能
　　该组件的封装的目的是解决一下几个工作中的实际问题
　　1、对记录的日志内容格式完善
　　2、微服务项目中，程序自动记录不同服务间的调用关系，以及出参、入参、执行时间等
　　3、同一项目中，不同方法及其层之间的调用关系等信息
　　4、其最终目的就是，实现对系统的一个整体监控

主要封装扩展功能点：
1、通过对log4net进行扩展，能够自定义了一些日志格式颜色内容等
2、通过代理+特性的方式，实现程序自动记录不同服务间，以及同一程序间的相互调用的交互日志
3、采用队列的方式实现异步落地日志到磁盘文件



主要核心代码示例，具体的详细代码，我已经上传至githut开源项目中，如有需要可以下载了解

github源码地址：https://github.com/xuyuanhong0902/XYH.Log4Net.Extend.git

代理实现自动记录方法调用的详细日志

   /// <summary>
    /// XYH代理实现类.
    /// </summary>
    public class XYHAopProxy : RealProxy
    {
        /// <summary>
        /// 构造函数.
        /// </summary>
        /// <param name="target">目标类型.</param>
        public XYHAopProxy(Type target)
            : base(target)
        {
        }

        /// <summary>
        /// 重写代理实现.
        /// </summary>
        /// <param name="msg">代理函数</param>
        /// <returns>返回结果</returns>
        public override IMessage Invoke(IMessage methodInvoke)
        {
            //// 方法开始执行时间
            DateTime executeStartTime = System.DateTime.Now;

            //// 方法执行结束时间
            DateTime executeEndTime = System.DateTime.Now;

            IMessage message = null;
            IMethodCallMessage call = methodInvoke as IMethodCallMessage;
            object[] customAttributeArray = call.MethodBase.GetCustomAttributes(false);
            call.MethodBase.GetCustomAttributes(false);

            try
            {
                // 前处理.
                List<IAopAction> proActionList = this.InitAopAction(customAttributeArray, AdviceType.Before);

                //// 方法执行开始记录日志
                if (proActionList != null && proActionList.Count > 0  )
                {
                    foreach (IAopAction item in proActionList)
                    {
                        IMessage preMessage = item.PreProcess(methodInvoke, base.GetUnwrappedServer());
                        if (preMessage != null)
                        {
                            message = preMessage;
                        }
                    }

                    if (message != null)
                    {
                        return message;
                    }
                }

                message = Proessed(methodInvoke);

                // 后处理.
                proActionList = this.InitAopAction(customAttributeArray, AdviceType.Around);

                //// 方法执行结束时间
                executeEndTime = System.DateTime.Now;

                //// 方法执行结束记录日志
                if (proActionList != null && proActionList.Count > 0)
                {
                    foreach (IAopAction item in proActionList)
                    {
                        item.PostProcess(methodInvoke, message, base.GetUnwrappedServer(), executeStartTime, executeEndTime);
                    }
                }
            }
            catch (Exception ex)
            {
                //// 方法执行结束时间
                executeEndTime = System.DateTime.Now;

                // 异常处理.吃掉异常，不影响主业务
                List<IAopAction> proActionList = this.InitAopAction(customAttributeArray, AdviceType.Around);
                if (proActionList != null && proActionList.Count > 0)
                {
                    foreach (IAopAction item in proActionList)
                    {
                        item.ExceptionProcess(ex, methodInvoke, base.GetUnwrappedServer(), executeStartTime, executeEndTime);
                    }
                }
            }

            return message;
        }

        /// <summary>
        /// 处理方法执行.
        /// </summary>
        /// <param name="methodInvoke">代理目标方法</param>
        /// <returns>代理结果</returns>
        public virtual IMessage Proessed(IMessage methodInvoke)
        {
            IMessage message;
            if (methodInvoke is IConstructionCallMessage)
            {
                message = this.ProcessConstruct(methodInvoke);
            }
            else
            {
                message = this.ProcessInvoke(methodInvoke);
            }
            return message;
        }

        /// <summary>
        /// 普通代理方法执行.
        /// </summary>
        /// <param name="methodInvoke">代理目标方法</param>
        /// <returns>代理结果</returns>
        public virtual IMessage ProcessInvoke(IMessage methodInvoke)
        {
            IMethodCallMessage callMsg = methodInvoke as IMethodCallMessage;
            object[] args = callMsg.Args;   //方法参数                 
            object o = callMsg.MethodBase.Invoke(base.GetUnwrappedServer(), args);  //调用 原型类的 方法       

            return new ReturnMessage(o, args, args.Length, callMsg.LogicalCallContext, callMsg);   // 返回类型 Message
        }

        /// <summary>
        /// 构造函数代理方法执行.
        /// </summary>
        /// <param name="methodInvoke">代理目标方法</param>
        /// <returns>代理结果</returns>
        public virtual IMessage ProcessConstruct(IMessage methodInvoke)
        {
            IConstructionCallMessage constructCallMsg = methodInvoke as IConstructionCallMessage;
            IConstructionReturnMessage constructionReturnMessage = this.InitializeServerObject((IConstructionCallMessage)methodInvoke);
            RealProxy.SetStubData(this, constructionReturnMessage.ReturnValue);

            return constructionReturnMessage;
        }

        /// <summary>
        /// 代理包装业务处理.
        /// </summary>
        /// <param name="customAttributeArray">代理属性</param>
        /// <param name="adviceType">处理类型</param>
        /// <returns>结果.</returns>
        public virtual List<IAopAction> InitAopAction(object[] customAttributeArray, AdviceType adviceType)
        {
            List<IAopAction> actionList = new List<IAopAction>();
            if (customAttributeArray != null && customAttributeArray.Length > 0)
            {
                foreach (Attribute item in customAttributeArray)
                {
                    XYHMethodAttribute methodAdviceAttribute = item as XYHMethodAttribute;
                    if (methodAdviceAttribute != null && (methodAdviceAttribute.AdviceType == adviceType))
                    {
                        if (methodAdviceAttribute.ProcessType == ProcessType.None)
                        {
                            continue;
                        }

                        if (methodAdviceAttribute.ProcessType == ProcessType.Log)
                        {
                            actionList.Add(new LogAopActionImpl());
                            continue;
                        }
                    }
                }
            }

            return actionList;
        }
    }
　　类注解

  /// <summary>
    /// XYH代理属性[作用于类].
    /// ************************************
    /// [IankaAop(typeof(IankaInstance)]
    /// [DecorateSymbol] Class ClassName
    /// ************************************
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class XYHAopAttribute : ProxyAttribute
    {
        public XYHAopAttribute()
        {
        }

        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            XYHAopProxy realProxy = new XYHAopProxy(serverType);
            return realProxy.GetTransparentProxy() as MarshalByRefObject;
        }
    }
　　队列实现异步日志落地到磁盘文件

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
　　自定义扩展log4net日志格式内容

namespace XYH.Log4Net.Extend
{
    /// <summary>
    /// 自定义布局（对log2net日志组件的布局自定义扩展）.
    /// </summary>
    public class HandlerPatternLayout : PatternLayout
    {
        /// <summary>
        /// 构造函数.
        /// </summary>
        public HandlerPatternLayout()
        {
            ///// 机器名称
            this.AddConverter("LogMachineCode", typeof(LogMachineCodePatternConvert));

            //// 方法名称
            this.AddConverter("MethodName", typeof(LogMethodNamePatternConvert));

            //// 方法入参
            this.AddConverter("MethodParam", typeof(LogMethodParamConvert));

            //// 方法出参
            this.AddConverter("MethodResult", typeof(LogMethodResultConvert));

            //// 程序名称
            this.AddConverter("LogProjectName", typeof(LogProjectNamePatternConvert));

            //// IP 地 址
            this.AddConverter("LogIpAddress", typeof(LogServiceIpPatternConvert));

            //// 日志编号
            this.AddConverter("LogUniqueCode", typeof(LogUniquePatternConvert));

            //// 日志序列号
            this.AddConverter("LogSerialNumber", typeof(LogSerialNumberPatternConvert));

            //// 调用路径
            this.AddConverter("InvokeName", typeof(LogInvokeNamePatternConvert));

            //// 执行开始时间
            this.AddConverter("ExecuteStartTime", typeof(ExecuteStartTimePatternConvert));

            //// 执行结束时间
            this.AddConverter("ExecuteEndTime", typeof(ExecuteEndTimePatternConvert));

            //// 执行时间
            this.AddConverter("ExecuteTime", typeof(ExecuteTimePatternConvert));
        }
    }
}
　　



使用说明：
第一步：需要dll文件引用
需要引用两个dell文件：
jeson序列化：Newtonsoft.Json.dll
log4net组件：log4net.dll
log3net扩展组件：XYH.Log4Net.Extend.dll

第二步：log4配置文件配置
主要配置日志的存储地址，日志文件存储格式、内容等
下面，给一个参考配置文件，具体的配置可以根据实际需要自由配置，其配置方式很log4net本身的配置文件一样，在此不多说
<log4net>
  <root>
    <!-- 定义记录的日志级别[None、Fatal、ERROR、WARN、DEBUG、INFO、ALL]-->
    <level value="ALL"/>
    <!-- 记录到什么介质中-->
    <appender-ref ref="LogInfoFileAppender"/>
    <appender-ref ref="LogErrorFileAppender"/>
  </root>
  <!-- name属性指定其名称,type则是log4net.Appender命名空间的一个类的名称,意思是,指定使用哪种介质-->
  <appender name="LogInfoFileAppender" type="log4net.Appender.RollingFileAppender">
    <!-- 输出到什么目录-->
    <param name="File" value="Log\\LogInfo\\"/>
    <!-- 是否覆写到文件中-->
    <param name="AppendToFile" value="true"/>
    <!-- 单个日志文件最大的大小-->
    <param name="MaxFileSize" value="10240"/>
    <!-- 备份文件的个数-->
    <param name="MaxSizeRollBackups" value="100"/>
    <!-- 是否使用静态文件名-->
    <param name="StaticLogFileName" value="false"/>
    <!-- 日志文件名-->
    <param name="DatePattern" value="yyyyMMdd".html""/>
    <param name="RollingStyle" value="Date"/>
    <!--布局-->
    <layout type="XYH.Log4Net.Extend.HandlerPatternLayout">
      <param name="ConversionPattern" value="<HR COLOR=blue>%n%n
                                             日志编号：%property{LogUniqueCode}  <BR >%n
                                             日志序列：%property{LogSerialNumber} <BR>%n
                                             机器名称：%property{LogMachineCode} <BR>%n
                                             IP 地 址：%property{LogIpAddress} <BR>%n
                                             开始时间：%property{ExecuteStartTime} <BR>%n
                                             结束时间：%property{ExecuteEndTime} <BR>%n
                                             执行时间：%property{ExecuteTime} <BR>%n
                                             程序名称：%property{LogProjectName} <BR>%n
                                             方法名称：%property{MethodName} <BR>%n
                                             方法入参：%property{MethodParam} <BR>%n
                                             方法出参：%property{MethodResult} <BR>%n
                                             日志信息：%m <BR >%n
                                             日志时间：%d <BR >%n
                                             日志级别：%-5p <BR >%n
                                             异常堆栈：%exception <BR >%n
                                             <HR Size=1 >"/>
    </layout>
  </appender>
  <!-- name属性指定其名称,type则是log4net.Appender命名空间的一个类的名称,意思是,指定使用哪种介质-->
  <appender name="LogErrorFileAppender" type="log4net.Appender.RollingFileAppender">
    <!-- 输出到什么目录-->
    <param name="File" value="Log\\LogError\\"/>
    <!-- 是否覆写到文件中-->
    <param name="AppendToFile" value="true"/>
    <!-- 备份文件的个数-->
    <param name="MaxSizeRollBackups" value="100"/>
    <!-- 单个日志文件最大的大小-->
    <param name="MaxFileSize" value="10240"/>
    <!-- 是否使用静态文件名-->
    <param name="StaticLogFileName" value="false"/>
    <!-- 日志文件名-->
    <param name="DatePattern" value="yyyyMMdd".html""/>
    <param name="RollingStyle" value="Date"/>
    <!--布局-->
    <layout type="XYH.Log4Net.Extend.HandlerPatternLayout">
      <param name="ConversionPattern" value="<HR COLOR=red>%n
                                             日志编号：%property{LogUniqueCode}  <BR >%n
                                             日志序列：%property{LogSerialNumber} <BR>%n
                                             机器名称：%property{LogMachineCode} <BR>%n
                                             IP 地 址: %property{LogIpAddress} <BR>%n
                                             程序名称：%property{LogProjectName} <BR>%n
                                             方法名称：%property{MethodName}<BR>%n
                                             方法入参：%property{MethodParam} <BR>%n
                                             方法出参：%property{MethodResult} <BR>%n
                                             日志信息：%m <BR >%n
                                             日志时间：%d <BR >%n
                                             日志级别：%-5p <BR >%n
                                             异常堆栈：%exception <BR >%n
                                             <HR Size=1 >"/>
    </layout>
    <filter type="log4net.Filter.LevelRangeFilter">
      <levelMin value="ERROR"/>
      <levelMax value="FATAL"/>
    </filter>
  </appender>
</log4net>
　　


第三步：在Global.asax文件中注册消息队列
protected void Application_Start()
{
AreaRegistration.RegisterAllAreas();
FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
RouteConfig.RegisterRoutes(RouteTable.Routes);
BundleConfig.RegisterBundles(BundleTable.Bundles);

////注册日志队列
ExtendLogQueue.Instance().Register();
}

第四步：在Global.asax文件中生成处理日志序列号
/// <summary>
/// 每一个请求执行开始
/// </summary>
protected void Session_Start() {
//// 记录获取创建每一个请求的序列号
/// 如果调用放传递了序列号，那么就直接去调用放传递的序列号
/// 如果调用放未传递，那么则生成一个序列号
/// 这样，在一次请求的头部传递一个该请求的唯一序列号，并在以后的每一个请求都一直传递下去
/// 这样，就能够通过这个序列号把每一次请求之间的服务或者方法调用关系串联起来
String[] serialNumber = Request.Headers.GetValues("serialNumber");
if (serialNumber!=null && serialNumber.Length>0 && !string.IsNullOrEmpty(serialNumber[0]))
{
Session["LogSerialNumber"] = serialNumber[0];
}
else
{
Session["LogSerialNumber"] = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
}
}

第五步：在需要自动记录日志的方法类上加上对应的注解

//// 在需要自动记录日志的类上加上 XYHAop注解
[XYHAop]
public class Class2: calssAdd
{
//// 需要记录自动记录交互日志的方法注解 ProcessType.Log

//// 同时该类还必须继承ContextBoundObject

[XYHMethod(ProcessType.Log)]
public int AddNum(int num1, int num2)
{
}
//// 需要记录自动记录交互日志的方法注解 ProcessType.None，其实不加注解也不会记录日志
[XYHMethod(ProcessType.None)]
public int SubNum(int num1, int num2)
{
}
}


第六步：完成上面五步已经能够实现自动记录交互日志了，

 但是在实际使用中我们也会手动记录一些日志，本插件也支持手动记录日志的同样扩展效果

目前支持以下6中手动记录日志的重载方法

 /// <summary>
    /// 记录日志扩展入口
    /// </summary>
    public class XYHLogOperator
    {
        /// <summary>
        /// 添加日志.
        /// </summary>
        /// <param name="message">日志信息对象</param>
        public static void WriteLog(object message)
        {
            new MessageIntoQueue().WriteLog(message);
        }

        /// <summary>
        /// 添加日志.
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="level">日志信息级别</param>
        public static void WriteLog(object message, LogLevel level)
        {
            new MessageIntoQueue().WriteLog(message, level);
        }

        /// <summary>
        /// 添加日志.
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="level">日志信息级别</param>
        /// <param name="exception">异常信息对象</param>
        public static void WriteLog(object message, Exception exception)
        {
            new MessageIntoQueue().WriteLog(message, exception);
        }

        /// <summary>
        /// 添加日志.
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="methodParam">方法入参</param>
        /// <param name="methodResult">方法请求结果</param>
        public static void WriteLog(object message, string methodName, object methodParam, object methodResult)
        {
            new MessageIntoQueue().WriteLog(message, methodName, methodParam, methodResult);
        }

        /// <summary>
        /// 添加日志.
        /// </summary>
        /// <param name="message">日志信息对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="methodParam">方法入参</param>
        /// <param name="methodResult">方法请求结果</param>
        /// <param name="level">日志记录级别</param>
        public static void WriteLog(object message, string methodName, object methodParam, object methodResult, LogLevel level)
        {
            new MessageIntoQueue().WriteLog(message, methodName, methodParam, methodResult, level);
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="extendLogInfor">具体的日志消息model</param>
        public static void WriteLog(LogMessage extendLogInfor)
        {
            new MessageIntoQueue().WriteLog(extendLogInfor);
        }
    }
}
　　



