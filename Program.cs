using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            string[] dbnames = new string[] { "SaleManageSPDB", "ProductionSystemDB" };
            string[] cons = new string[]
            {
                "server=114.55.138.73;user id = sa; password=ZYRScrljsyc060822;database=ZYRS_SaleManage_DataBase_SP_Test;Max Pool Size=50;Min Pool Size=2",
                "server=114.55.138.73;user id = sa; password=ZYRScrljsyc060822;database=ZYRS_ProductionSystem_DataBase;Max Pool Size=50;Min Pool Size=2"
            };

            DynamicCompilerDBOP.DynamicCompiler(dbnames, cons, false);


        }
    }
}
