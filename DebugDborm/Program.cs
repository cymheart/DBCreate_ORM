using DbInfoCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 此项目建立dbormCompiler项目需要动态编译的下面几个资源文件
/// DbCreate.cs，SqlHelper.cs，ScDataBaseTableOpBase.cs， DataBaseAccess.cs
/// 这几个文件需要拖放到dbormCompiler -> Properties -> Resources.resx 的资源文件中
/// 替换旧的同名文件，同时重新编译dbormCompiler项目生成新的dbormCompiler.dll
/// 不需要编译这个项目
/// </summary>
/// 

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
