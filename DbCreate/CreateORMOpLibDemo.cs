using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORM_DBOP_COMPLER
{
    static class CreateORMOpLibDemo
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            string[] dbnames = new string[] { "TestDataBase1" };
            string[] cons = new string[]
            {
                "server=.;user id = sa; password=11111111;database=TestDataBase1;Max Pool Size=50;Min Pool Size=2",
              
            };
            int a;
            a = 3;
            DCompilerDBOP.DynamicCompiler(dbnames, cons, true, null, false);
        }
    }
}
