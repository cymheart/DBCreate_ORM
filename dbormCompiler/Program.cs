using ORM_DBOP_COMPLER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    /// <summary>
    /// 此代码调试用，要生成dborm.dll,请生成运行compile_dborm项目
    /// 
    /// 动态编译出和自己项目数据库相匹配的dborm.dll库
    /// 比如我下面编译的是自己本机数据库，TestDataBase1，TestDataBase1
    /// 对应的dborm.dll
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            //数据库名称
            string[] dbnames = new string[] { "TestDataBase1", "TestDataBase2" };

            //修改数据库(ip和密码)连接字符串，适配自己服务器的连接
            string[] cons = new string[]
            {
                "server=.;user id = sa; password=11111111;database=TestDataBase1;Max Pool Size=50;Min Pool Size=2",  
                 "server=.;user id = sa; password=11111111;database=TestDataBase2;Max Pool Size=50;Min Pool Size=2",
            };

            dbormCompiler.DynamicCompiler(dbnames, cons, true, null, true);
        }  
    }
}