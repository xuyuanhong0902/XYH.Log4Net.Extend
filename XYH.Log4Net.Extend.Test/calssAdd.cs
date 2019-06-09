using XYH.Log4Net.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogOperationTest
{
    public class calssAdd : ContextBoundObject//放到特定的上下文中，该上下文外部才会得到该对象的透明代理
    {
        //public int AddNum(int num1, int num2)
        //{
        //    return num1 + num2;
        //}
    }
}