using System.Data.SqlClient;
using System;
using System.Data;
using System.Threading;

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


