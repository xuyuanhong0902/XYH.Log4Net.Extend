# XYH.Log4Net.Extend
基于Log4Net日志组件的扩展，实现微服务的监控日志组件，主要内容包括：不同服务间的调用交互日志，同一个程序内不同方法调用日志。在实现上：在log4net的基础上，通过代理的方式实现自动记日志，通过队列的方式，实现异步记录日志。

使用说明：
第一步：需要dll文件引用
       需要引用两个dell文件：
       jeson序列化：Newtonsoft.Json.dll
       log4net组件：log4net.dll
       log3net扩展组件：XYH.Log4Net.Extend.dll
      
 第二步：log4配置文件配置
       主要配置日志的存储地址，日志文件存储格式、内容等
       下面，给一个参考配置文件，具体的配置可以根据实际需要自由配置，其配置方式很log4net本身的配置文件一样，在此不多说
     
