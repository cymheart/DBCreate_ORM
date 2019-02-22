using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM_DBOP_COMPLER;

namespace COMPLER_DBORM
{
    /// 使用dbormCompiler.dll动态编译出和自己项目数据库相匹配的dborm.dll映射库，
    /// dborm.dll即是在自己项目中正式引用的映射库，可以打开dbormDemo项目测试dborm.dll的功能
    /// 
    /// 比如我下面编译的是自己本机数据库，TestDataBase1，TestDataBase1 对应的dborm.dll
    /// 如果数据库有改动需要运行这个项目重新编译一份对应的dborm.dll
    /// 
    /// 如果dbormCompiler项目有改动，请先重新编译dbormCompiler项目生成最新的dbormCompiler.dll，
    /// 并添加dbormCompiler.dll到此项目的引用中
    class Program
    {
        static void Main(string[] args)
        {

            CreateDBORM();
        }

        /// <summary>
        ///   这个方法在没有生成数据库表和程序模型数据映射时调用一次,生成映射库，或者
        ///   在数据库表发生改变时再次调用，更新映射库，
        ///   映射库名为: dborm.dll
        /// </summary>
        static void CreateDBORM()
        {

            //数据库名称
            string[] dbnames = new string[] { "TestDataBase1", "TestDataBase2" };

            //修改数据库(ip和密码)连接字符串，适配自己服务器的连接
            string[] cons = new string[]
            {
                "server=.;user id = sa; password=11111111;database=TestDataBase1;Max Pool Size=50;Min Pool Size=2",
                 "server=.;user id = sa; password=11111111;database=TestDataBase2;Max Pool Size=50;Min Pool Size=2",
            };

            dbormCompiler.DynamicCompiler(dbnames, cons, true, null, false);

            
        }
    }
}
