using XYH.Log4Net.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace LogOperationTest
{
    [XYHAop]
    public class Class2: calssAdd
    {
        [XYHMethod(ProcessType.Log)]
        public int AddNum(int num1, int num2)
        {
            ////Thread.Sleep(5000);

            int num = 19;
            num++;

            //// 测试异常日志记录
             Convert.ToInt32("ajk89");

            return num1 + num2;
        }
        [XYHMethod(ProcessType.None)]
        public int SubNum(int num1, int num2)
        {
            ////Thread.Sleep(5000);

            int num = 19;
            num++;
            return num1 + num2;
        }
    }
}