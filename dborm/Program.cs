using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM_DBOP_COMPLER;

namespace DB_ORM
{
    class Program
    {
        static void Main(string[] args)
        {

            CreateDBORM();
        }

        /// <summary>
        ///   这个方法在没有生成数据库表和程序模型数据映射时调用一次,生成映射库，或者
        ///   在数据库表发生改变时再次调用，更新映射库，
        ///   映射库名为: ScDB.dll
        /// </summary>
        static void CreateDBORM()
        {
            string[] dbnames = new string[] { "TestDataBase1"};

            //修改数据库(ip和密码)连接字符串，适配自己服务器的连接
            string[] cons = new string[]
            {
                "server=.;user id = sa; password=11111111;database=TestDataBase1;Max Pool Size=50;Min Pool Size=2",
            };

            dbormCompiler.DynamicCompiler(dbnames, cons, true, null, false);
        }
    }
}
