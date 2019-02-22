using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataBaseOp;

namespace DBCreateDemo
{
    /// <summary>
    /// 按下面步骤测试demo
    /// 1.首先建立两个数据库TestDataBase1，TestDataBase2，再在两个库中分别建立两张表
    /// 建立数据库:TestDataBase1  库中有两张表: table1, table2
    /// 建立数据库:TestDataBase2  库中有两张表: tableA, tableB
    /// 
    /// 2.注意添加映射库dborm.dll到引用中， Test() 进行demo测试
    /// 如果当前dborm不是与自己项目数据相关联的，请首先打开项目compile_dborm,配置好项目数据库数据
    /// 重新生成新的对应的映射库dborm.dll
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {

            StartDBORM();
            Test();
            ReleaseDBORM();
        }

        /// <summary>
        /// 启动DBORM
        /// </summary>
        static void StartDBORM()
        {
            string[] dbnames = new string[] { "TestDataBase1", "TestDataBase2" };

            //修改数据库(ip和密码)连接字符串，适配自己服务器的连接
            string[] cons = new string[]
            {
                "server=.;user id = sa; password=11111111;database=TestDataBase1;Max Pool Size=50;Min Pool Size=2",
                 "server=.;user id = sa; password=11111111;database=TestDataBase2;Max Pool Size=50;Min Pool Size=2",
            };

            dborm.Init(dbnames, cons);
        }


        public class Comb
        {
            public int id;
            public int age;
            public string name;
            public string ids;
        }

        /// <summary>
        /// 测试dborm的增删改查
        /// </summary>
        static void Test()
        {
            //对表table1操作
            //获取对应数据库表的数据到程序中
            //获取方法:dborm.DB_<数据库名>.DTOP_<表名>.数据库操作函数()
            var opTable1 = dborm.DB_TestDataBase1.DTOP_TABLE1;
            var opTable2 = dborm.DB_TestDataBase1.DTOP_TABLE2;


            #region 插入数据
            //1.插入操作
            InsertData(1);

            //2.事务插入操作
            opTable1.trans = dborm.DB_TestDataBase1.BeginTransaction();
            try
            {
                InsertData(1);
                InsertData(1);
                InsertData(1);
                InsertData(3);

                opTable1.trans.Commit();
            }
            catch(Exception ex)
            {
                opTable1.trans.Rollback();
            }
            finally
            {
                dborm.DB_TestDataBase1.EndTransaction();
            }


            //对表table2操作
          
            var insertModel2 = new DB_TestDataBase1_DT_TABLE2();
            insertModel2.id = "lucking";
            insertModel2.price = 1223;
            insertModel2.num = 3;
            opTable2.InsertData(insertModel2);
            #endregion


            #region 查询数据

            //查询方法1
            var selectModel1 = new DB_TestDataBase1_DT_TABLE1();
            DataTable dt1 = opTable1.QueryData("id ,age", null);
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                selectModel1.id = (int)dt1.Rows[i]["id"];
                selectModel1.age = (int)dt1.Rows[i]["age"];
            }

            //查询方法2
            var selectModel2 = new DB_TestDataBase1_DT_TABLE1();
            DataTable dt2 = opTable1.QueryData();
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                selectModel2.id = (int)dt2.Rows[i]["id"];
                selectModel2.age = (int)dt2.Rows[i]["age"];
                selectModel2.name = (string)dt2.Rows[i]["name"];
            }




            //查询方法3
            Comb comb = new Comb();

            SqlStr sql = new SqlStr();
            SqlStr joinSql = new SqlStr();
            sql.sql = "dbo.TABLE1.id = 3 ";
            joinSql.sql = "INNER JOIN dbo.TABLE2 R ON R.num = dbo.TABLE1.id";
            DataTable dt3 = dborm.DB_TestDataBase1.DTOP_TABLE1.QueryData("dbo.TABLE1.*,R.id ids", sql, joinSql);



            for (int i = 0; i < dt3.Rows.Count; i++)
            {
                comb.id = (int)dt3.Rows[i]["id"];
                comb.age = (int)dt3.Rows[i]["age"];
                comb.name = (string)dt3.Rows[i]["name"];
                comb.ids = (string)dt3.Rows[i]["ids"];
            }

            #endregion




        }

        static void InsertData(int id)
        {
            var opTable1 = dborm.DB_TestDataBase1.DTOP_TABLE1;

            //获取对应数据库表的映射数据模型
            //获取方法：dataModel = new DB_<数据库名>_DT_<表名>()
            //DB_TestDataBase1_DT_table1 对应 DB_<数据库名>_DT_<表名> 中的
            //数据库名：TestDataBase1
            //表名：table1
            //获取对应表table1的数据model: modelTable1
            var insertModel = new DB_TestDataBase1_DT_TABLE1();
            insertModel.id = id;
            insertModel.age = 15;
            insertModel.name = "测试文字";

            //插入数据
            opTable1.InsertData(insertModel);
        }

        /// <summary>
        /// 释放DBORM
        /// </summary>
        static void ReleaseDBORM()
        {
            dborm.Dispose();
        }

    }
}
