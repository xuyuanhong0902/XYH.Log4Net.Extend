/**********************************************************************************
 * 类 名 称： LogLevel
 * 机器名称： LogLevel.cs
 * 命名空间： XYH.Log4Net.Extend
 * 文 件 名： LogLevel
 * 创建时间： 2019-06-09 
 * 作    者： 
 * 说    明：
 * ----------------------------------------------------------------------------------
 * 修改时间：
 * 修 改 人：
 * 修改说明:
 **********************************************************************************/namespace  XYH.Log4Net.Extend.Standard{
    /// <summary>
    /// 记录类型.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 基本信息日志
        /// </summary>
        Info,

        /// <summary>
        /// 调试日志
        /// </summary>
        Debug,

        /// <summary>
        /// 警告日志
        /// </summary>
        Warn,

        /// <summary>
        /// 错误日志
        /// </summary>
        Error,

        /// <summary>
        /// 监控日志
        /// </summary>
        Fatal,

        /// <summary>
        /// 不需要日志
        /// </summary>
        None
    }
}
