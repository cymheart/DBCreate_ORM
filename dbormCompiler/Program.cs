using ORM_DBOP_COMPLER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] dbnames = new string[] { "TestDataBase1", "TestDataBase2" };

            //修改数据库(ip和密码)连接字符串，适配自己服务器的连接
            string[] cons = new string[]
            {
                "server=1111;user id = sa; password=111111;database=TestDataBase1;Max Pool Size=50;Min Pool Size=2",
                "server=114.55.138.73;user id = sa; password=111111;database=TestDataBase2;Max Pool Size=50;Min Pool Size=2"
            };

            dbormCompiler.DynamicCompiler(dbnames, cons, true, null, true);
        }

      
    }
}