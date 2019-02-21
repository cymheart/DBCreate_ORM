using App;
using ORM_DBOP_COMPLER;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBCreateDemo
{
    public partial class Demo : Form
    {
        /// <summary>
        /// 按下面步骤测试demo
        /// 1.首先建立两个数据库TestDataBase1，TestDataBase2，再在两个库中分别建立两张表
        /// 建立数据库:TestDataBase1  库中有两张表: table1, table2
        /// 建立数据库:TestDataBase2  库中有两张表: tableA, tableB
        /// 
        /// 2.如果没有生产数据库到程序模型的映射库，打开CreateDBOp()注释生成一下映射库ScDB.dll
        /// 此时再注释调这个函数,以后不再需要使用（只有在数据库表发生改变时再次调用，更新映射库）
        /// 
        /// 3.把映射库ScDB.dll添加到引用中， 打开Test()函数进行demo测试
        /// </summary>
        public Demo()
        {
            InitializeComponent();

            string temp = System.Environment.GetEnvironmentVariable("TEMP");
            int a;
            a = 1;

            //如果没有生产数据库到程序模型的映射库，打开注释生成一下
            //CreateDBOp();
        }

        /// <summary>
        ///   这个方法在没有生成数据库表和程序模型数据映射时调用一次,生成映射库，或者
        ///   在数据库表发生改变时再次调用，更新映射库，
        ///   映射库名为: ScDB.dll
        /// </summary>
        void CreateDBOp()
        {
            string[] dbnames = new string[] { "TestDataBase1", "TestDataBase2" };

            //修改数据库(ip和密码)连接字符串，适配自己服务器的连接
            string[] cons = new string[]
            {
                "server=1111;user id = sa; password=111111;database=TestDataBase1;Max Pool Size=50;Min Pool Size=2",
                "server=114.55.138.73;user id = sa; password=111111;database=TestDataBase2;Max Pool Size=50;Min Pool Size=2"
            };

            DCompilerDBOP.DynamicCompiler(dbnames, cons, true, null, true);
        }




      ///// <summary>
      ///// 连接数据库
      ///// </summary>
      //  void StartScDB()
      //  {
      //      ScDB.Init(dbnames, constrs);
      //  }


      //  /// <summary>
      //  /// 测试ScDB的增删改查
      //  /// </summary>
      //  void Test()
      //  {

      //      //获取对应数据库表的映射数据模型
      //      //获取方法：dataModel = new DB_<数据库名>_DT_<表名>()
      //      //DB_TestDataBase1_DT_table1 对应 DB_<数据库名>_DT_<表名> 中的
      //      //数据库名：TestDataBase1
      //      //表名：table1
      //      //获取对应表table1的数据model: modelTable1
      //      DB_TestDataBase1_DT_table1 modelTable1 = new DB_TestDataBase1_DT_table1();

      //      SqlStr sql = new SqlStr();
      //      SqlStr joinSql = new SqlStr();


      //      //获取对应数据库表的数据到程序中
      //      //获取方法:ScDB.DB_<数据库名>.DTOP_<表名>.数据库操作函数()
      //      DataTable dt = ScDB.DB_SQLServerConnection.DTOP_APP_CLOTHMODEL.QueryData("clothmodelcnname,clothmodelengname", null);

      //      //查询方法
      //      sql.sql = "ORDERCODE = '" + orderCode + "'";
      //      joinSql.sql = "INNER JOIN dbo.APP_CLOTHCLASS R ON R.CLOTHCLASSCODE = STYLE";
      //      dt = ScDB.DB_SQLServerConnection.DTOP_OD_ORDER.QueryData("dbo.OD_ORDER.*,R.CLOTHCLASSCNNAME STYLENAME", sql, joinSql);

      //  }


      //  /// <summary>
      //  /// 释放数据库连接
      //  /// </summary>
      //  static void Release()
      //  {
      //      ScDB.Dispose();
      //  }

    }
}
