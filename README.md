# XYH.Log4Net.Extend
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

使用说明：
第一步：需要dll文件引用
       需要引用两个dell文件：
       jeson序列化：Newtonsoft.Json.dll
       log4net组件：log4net.dll
       log3net扩展组件：XYH.Log4Net.Extend.dll
      
 第二步：log4配置文件配置
       主要配置日志的存储地址，日志文件存储格式、内容等
       下面，给一个参考配置文件，具体的配置可以根据实际需要自由配置，其配置方式很log4net本身的配置文件一样，在此不多说
     
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
    //// 同时该类还必须继承ContextBoundObject
    [XYHAop]
    public class Class2: ContextBoundObject
    {
        //// 需要记录自动记录交互日志的方法注解 ProcessType.Log
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
    
       
    
    
    
    
    
