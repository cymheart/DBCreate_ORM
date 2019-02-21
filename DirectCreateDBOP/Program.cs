using DbInfoCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectCreateDBOP
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] dbnames = new string[] { "TestDataBase1" };
            string[] cons = new string[]
            {
                "server=.;user id = sa; password=11111111;database=TestDataBase1;Max Pool Size=50;Min Pool Size=2",

            };

            DbCreate dbCreate = new DbCreate();
            dbCreate.DbCreateInfos(dbnames, cons, true);
        }
    }
}
