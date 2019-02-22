using System.Data.SqlClient;
using System;
using System.Data;
using System.Threading;

/// <summary>
/// 这个文件属于dbormCompiler项目的资源文件，
/// 在扩展修改后，需要重新把此文件拖放到dbormCompiler -> Properties -> Resources.resx 的资源文件中
/// 替换旧的同名文件，同时重新编译dbormCompiler项目生成新的dbormCompiler.dll
/// </summary>
/// 
namespace DataBaseOp
{
    public class ScDataBaseTableOpBase : IDisposable
    {
        public string dbName;
        public string conStr;
        public SqlConnection con;

        SqlHelper sqlHelper = new SqlHelper();

        public ScDataBaseTableOpBase (string dbName, string conStr)
        {
            this.dbName = dbName;
            this.conStr = conStr;
            con = new SqlConnection();
            con.ConnectionString = conStr;
        }

        ~ScDataBaseTableOpBase()
        {
        }

        /// <summary>
        /// 销毁连接池中的这个连接
        /// </summary>
        public virtual void Dispose()
        {          
            con.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Open()
        {
            if (con.State != ConnectionState.Open)
                ConnOpen(con);
        }


        /// <summary>
        /// 把连接放回连接池
        /// </summary>
        public void Close()
        {
            con.Close();
        }


        #region 事务
        public SqlTransaction BeginTransaction(string transName)
        {
            Open();
            return con.BeginTransaction(transName);
        }

        public SqlTransaction BeginTransaction()
        {
            Open();
            return con.BeginTransaction();
        }


        /// <summary>
        /// 把连接放回连接池
        /// </summary>
        public void EndTransaction()
        {
            Close();
        }
        #endregion


        #region 原始SQL操作

        public DataTable ExecuteDataTable(SqlTransaction trans, string sql, object[] parameters)
        {
            return sqlHelper.ExecuteDataTable(con, trans, sql, parameters);
        }

        public int ExecuteNoneQuery(SqlTransaction trans, string sql, object[] parameters)
        {
            return sqlHelper.ExecuteNoneQuery(con, trans, sql, parameters);
        }

        #endregion


        public void ResetMatchConnection(string[] dbNamesMatch, string[] conStrsMatch)
        {
            for(int i=0; i<dbNamesMatch.Length; i++)
            {
                if(dbNamesMatch[i] == dbName)
                {
                    conStr = conStrsMatch[i];
                    con.Dispose();
                    con = new SqlConnection();
                    con.ConnectionString = conStr;
                    return;
                }
            }
        }

        bool ConnOpen(SqlConnection con)
        {
            int flag = 0;
            while (flag < 2)
            {
                try
                {
                    con.Open();
                    return true;
                }
                catch
                {
                    Thread.Sleep(2000);
                    flag++;
                }
            }
            return false;
        }
    }
}

