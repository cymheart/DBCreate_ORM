using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

/// <summary>
/// 这个文件属于dbormCompiler项目的资源文件，
/// 在扩展修改后，需要重新把此文件拖放到dbormCompiler -> Properties -> Resources.resx 的资源文件中
/// 替换旧的同名文件，同时重新编译dbormCompiler项目生成新的dbormCompiler.dll
/// </summary>

namespace DataBaseOp
{

    public class DataBaseAccess<T>
    {
        public SqlConnection con;
        public SqlTransaction trans;
        public string tbName;
        public string[] fields;
        public Dictionary<string, object> fieldVaildValueDict = new Dictionary<string, object>();
        public List<DefaultCondField> defCondFieldList = new List<DefaultCondField>();
        public bool IsUsedDefaultCond = true;


        /// <summary>
        /// 查询类型，模糊或精确查询
        /// </summary>
        public PreciseOrFuzzyType searchType = PreciseOrFuzzyType.Fuzzy;

        /// <summary>
        /// 对应表字段的数据模型条件
        /// </summary>
        public T condObj = default(T);
        
        /// <summary>
        /// 在条件模型中排除一些字段，不按照指定的查询类型查询
        /// </summary>
        public string excludeField = null;

        /// <summary>
        /// 字段语句(ex1: column_name1,NAME,SEX      ex2: COUNT(column_name) )
        /// </summary>
        public string fieldSql = null;

        /// <summary>
        /// 条件语句(ex: ID = 'xxxx' and NAME = 'n')
        /// </summary>
        public SqlStr condSql = null;
        public string condSqlc = null;

        /// <summary>
        /// 连接语句（ex: LEFT JOIN TABLE2 ON TABLE1.ID=TABLE2.ID ）
        /// </summary>
        public SqlStr joinSql = null;
        public string joinSqlc = null;

        /// <summary>
        /// 条目起始位置
        /// </summary>
        public int startIdx = -1;

        /// <summary>
        /// 条目结束位置
        /// </summary>
        public int endIdx = -1;

        /// <summary>
        /// 所有条目根据指定字段排序(ex: ID asc)
        /// </summary>
        public string sortFieldSql = null;


        /// <summary>
        /// 对应表字段的数据模型更新数据
        /// </summary>
        public T updateObj = default(T);
        public SqlStr updateCondSql = null;
        public string updateCondSqlc = null;


        //
        public T deleteCondObj = default(T);
        public SqlStr deleteCondSql = null;
        public string deleteCondSqlc = null;

        SqlHelper sqlHelper = new SqlHelper();

        public DataBaseAccess()
        {

        }


        #region 重设表

        public void ResetTableInfo(string tbName, string tbType = "U")
        {
            SqlCommand com = new SqlCommand();
            com.Connection = con;
            com.CommandType = CommandType.Text;
            com.CommandText = "select a.name 表名,b.name 字段名,c.name 字段类型,c.length 字段长度 from sysobjects a,syscolumns b,systypes c where a.id=b.id and a.name = '" + tbName + "' and a.xtype = '" + tbType + "'and b.xtype = c.xtype";
            SqlDataReader dr = com.ExecuteReader();

            List<string> fieldList = new List<string>();

            while (dr.Read())
            {
                fieldList.Add(dr[1].ToString());
            }

            fields = fieldList.ToArray();

            com.Dispose();
        }

        public void ResetTableInfo(string tbName, Type type)
        {
            this.tbName = tbName;

            //
            FieldInfo[] fieldInfos = type.GetFields();
            List<string> fieldList = new List<string>();
            int i = 0;

            fields = new string[fieldInfos.Length];

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                fields[i++] = fieldInfo.Name;
            }
        }

        public void ResetTableInfo(string tbName, T modelObj)
        {
            Type type = modelObj.GetType(); //获取类型
            ResetTableInfo(tbName, type);
        }

        public void ResetTableInfo(string tbName, string[] fields)
        {
            this.tbName = tbName;
            this.fields = fields;
        }

        private void ResetSqlParams()
        {
            searchType = PreciseOrFuzzyType.Fuzzy;
            condObj = default(T);
            excludeField = null;
            fieldSql = null;
            condSql = null;
            condSqlc = null;
            joinSql = null;
            joinSqlc = null;
            startIdx = -1;
            endIdx = -1;
            sortFieldSql = null;
        }

        private void ResetUpdateSqlParams()
        {
            updateObj = default(T);
            updateCondSql = null;
            updateCondSqlc = null;
        }


        #endregion

        #region 查询

        public DataTable QueryData()
        {
            DataTable dt;

            if (condSqlc != null) {
                condSql = new SqlStr();
                condSql.sql = condSqlc;
            }

            if (joinSqlc != null)
            {
                joinSql = new SqlStr();
                joinSql.sql = joinSqlc;
            }

            if (condObj != null)
                dt = QueryData(searchType, fieldSql, condObj, excludeField, condSql, joinSql, startIdx, endIdx, sortFieldSql);
            else
                dt = QueryData(fieldSql, condSql, joinSql, startIdx, endIdx, sortFieldSql);

            ResetSqlParams();

            return dt;
        }

        /// <summary>
        /// 查询语句，支持分页，连接，条件查询
        /// </summary>
        /// <param name="fieldSqls">字段语句(ex1: column_name1,NAME,SEX  // ex2: COUNT(column_name) )</param>
        /// <param name="condSql">条件语句(ex: ID = 'xxxx' and NAME = 'n')</param>
        /// <param name="joinSql">连接语句（ex: LEFT JOIN TABLE2 ON TABLE1.ID=TABLE2.ID ）</param>
        /// <param name="startIdx">条目起始位置</param>
        /// <param name="endIdx">条目结束位置</param>
        /// <param name="sortFieldSql">所有条目根据指定字段排序(ex: ID asc)</param>
        /// <returns></returns>
        public DataTable QueryData(
            string fieldSql,
            SqlStr condSql = null,
            SqlStr joinSql = null,
            int startIdx = -1, int endIdx = -1, string sortFieldSql = null)
        {
            SqlStr[] sqls = null;
            if (condSql != null)
                sqls = new SqlStr[] { condSql };

            return QueryData(fieldSql, sqls, joinSql, startIdx, endIdx, sortFieldSql);
        }

        private DataTable QueryData(
            string fieldSql,
            SqlStr[] condSql = null,
            SqlStr joinSql = null,
            int startIdx = -1, int endIdx = -1, string sortFieldSql = null)
        {
            string cond = "";
            string join = "";
            List<object> condParams = new List<object>();
            List<object> joinParams = new List<object>();

            if (condSql != null)
            {
                for (int i = 0; i < condSql.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(cond))
                        cond += " " + condSql[i].logicalRelation + " ";

                    cond += "(" + condSql[i].sql + ")";

                    if (condSql[i].sqlParams != null)
                    {
                        for (int j = 0; j < condSql[i].sqlParams.Length; j++)
                            condParams.Add(condSql[i].sqlParams[j]);
                    }
                }
            }

            if (joinSql != null)
            {
                join = joinSql.sql;

                if (joinSql.sqlParams != null)
                {
                    for (int j = 0; j < joinSql.sqlParams.Length; j++)
                        joinParams.Add(joinSql.sqlParams[j]);
                }
            }

            return QueryData(fieldSql, cond, condParams.ToArray(), join, joinParams.ToArray(), startIdx, endIdx, sortFieldSql);
        }


        /// <summary>
        /// 查询语句，支持分页，连接，条件查询
        /// </summary>
        /// <param name="type">查询类型，模糊或精确查询</param>
        /// <param name="fieldSqls">字段语句(ex1: column_name1,NAME,SEX  // ex2: COUNT(column_name) )</param>
        /// <param name="condObj">对应表字段的数据模型条件</param>
        /// <param name="excludeField">在条件模型中排除一些字段，不按照指定的查询类型查询</param>
        /// <param name="condSql">条件语句(ex: ID = 'xxxx' and NAME = 'n')</param>
        /// <param name="joinSql">连接语句（ex: LEFT JOIN TABLE2 ON TABLE1.ID=TABLE2.ID ）</param>
        /// <param name="startIdx">条目起始位置</param>
        /// <param name="endIdx">条目结束位置</param>
        /// <param name="sortFieldSql">所有条目根据指定字段排序(ex: ID asc)</param>
        /// <returns></returns>
        /// <returns></returns>
        public DataTable QueryData(
                   PreciseOrFuzzyType type,
                   string fieldSql,
                   T condObj, string excludeField,
                   SqlStr condSql = null,
                   SqlStr joinSql = null,
                   int startIdx = -1, int endIdx = -1, string sortFieldSql = null)
        {
            SqlStr[] sqls = null;
            if (condSql != null)
                sqls = new SqlStr[] { condSql };

            return QueryData(type, fieldSql, condObj, excludeField, sqls, joinSql, startIdx, endIdx, sortFieldSql);
        }
        public DataTable QueryData(
            PreciseOrFuzzyType type,
            string fieldSql,
            T condObj, string excludeField,
            SqlStr[] condSqls = null,
            SqlStr joinSql = null,
            int startIdx = -1, int endIdx = -1, string sortFieldSql = null)
        {
            List<SqlStr> condSqlList = new List<SqlStr>();
            SqlStr[] newCondSqls = CreateCondObjectToSql(condObj, excludeField, type);

            if (newCondSqls != null && newCondSqls.Length > 0)
            {
                foreach (SqlStr condSql in newCondSqls)
                {
                    if (condSql != null)
                        condSqlList.Add(condSql);
                }
            }

            if (condSqls != null && condSqls.Length > 0)
            {
                foreach (SqlStr condSql in condSqls)
                {
                    if (condSql != null)
                        condSqlList.Add(condSql);
                }
            }

            return QueryData(fieldSql, condSqlList.ToArray(), joinSql, startIdx, endIdx, sortFieldSql);
        }


        /// <summary>
        ///  <para>select * from (select[fieldSqls] from TableName [joinSql]) where rowNumber between [startIdx] and [endIdx] and [condSql]  </para>
        /// <para>例: select * from (select ROW_NUMBER() over(order by id asc) as 'rowNumber', * from table1) as temp where rowNumber between 1 and 5 </para>
        /// </summary>
        /// <param name="fieldSqls">字段语句</param>
        /// <param name="condSql">条件语句</param>
        /// <param name="condParams">条件语句参数</param>
        /// <param name="joinSql">连接语句</param>
        /// <param name="joinSqlParams">连接语句参数</param>
        /// <param name="startIdx">条目起始位置</param>
        /// <param name="endIdx">条目结束位置</param>
        /// <param name="sortFieldSql">所有条目根据指定字段排序</param>
        /// <returns></returns>

        private DataTable QueryDataPage(
            string fieldSql,
            string condSql, object[] condSqlParams,
            int startIdx, int endIdx, string sortFieldSql,
            string joinSql = null, object[] joinSqlParams = null)
        {
            string sql;
            string tmpTbSql;

            if (fieldSql == null || fieldSql.Length == 0)
            {
                sql = @"SELECT * FROM ";
                tmpTbSql = "(select ROW_NUMBER() over(order by " + sortFieldSql + ") as \'rowNumber\', * from " + tbName;
            }
            else
            {
                string qfieldSql = "";
                if (!string.IsNullOrWhiteSpace(fieldSql))
                    qfieldSql = fieldSql;

                sql = @"SELECT * FROM ";
                tmpTbSql = "(select ROW_NUMBER() over(order by " + sortFieldSql + ") as \'rowNumber\', " + qfieldSql + " from " + tbName;
            }

            //        
            string tmpJoinSql = "";
            string tmpTbWhereSql = @" WHERE 1=1 ";
            string whereSql = @" WHERE rowNumber between " + startIdx + " and " + endIdx;
            ArrayList parameters = new ArrayList();
            string newCondSql = "";
            string defaultQueryCond;
            string newDefaultCondSql = "";
            DataTable dt;

            if (!string.IsNullOrWhiteSpace(joinSql))
            {
                tmpJoinSql = joinSql;

                if (joinSqlParams != null && joinSqlParams.Length > 0)
                {
                    foreach (object param in joinSqlParams)
                        parameters.Add(param);
                }
            }

            if (!string.IsNullOrWhiteSpace(condSql))
                newCondSql = "AND ( " + condSql + " ) ";

            if (condSql != null && condSqlParams != null && condSqlParams.Length > 0)
            {
                foreach (object param in condSqlParams)
                    parameters.Add(param);
            }

            defaultQueryCond = CreateDefaultQueryCond(parameters);

            if (!string.IsNullOrWhiteSpace(defaultQueryCond))
                newDefaultCondSql = " AND (" + defaultQueryCond + " )";


            tmpTbSql += " " + tmpJoinSql + tmpTbWhereSql + newCondSql + newDefaultCondSql + ") as temp ";
            sql += tmpTbSql + whereSql;
            dt = sqlHelper.ExecuteDataTable(con, trans, sql, parameters.ToArray());

            return dt;
        }


        /// <summary>
        /// select[fieldSqls] from TableName [joinSql] where [condSql]
        /// </summary>
        /// <param name="fieldSqls">字段语句</param>
        /// <param name="condSql">条件语句</param>
        /// <param name="condParams">条件语句参数</param>
        /// <param name="joinSql">连接语句</param>
        /// <param name="joinSqlParams">连接语句参数</param>  
        /// <returns></returns>
        private DataTable QueryData(
            string fieldSql,
            string condSql, object[] condSqlParams,
            string joinSql = null, object[] joinSqlParams = null)
        {
            string sql;

            if (fieldSql == null || fieldSql.Length == 0)
            {
                sql = @"SELECT * FROM " + tbName;
            }
            else
            {
                string qfieldSql = "";
                if (!string.IsNullOrWhiteSpace(fieldSql))
                    qfieldSql = fieldSql;

                sql = @"SELECT " + qfieldSql + @" FROM " + tbName;
            }

            string newJoinSql = "";
            string whereSql = @" WHERE 1=1 ";
            ArrayList parameters = new ArrayList();
            string newCondSql = "";
            string defaultQueryCond;
            string newDefaultCondSql = "";
            DataTable dt;

            if (!string.IsNullOrWhiteSpace(joinSql))
            {
                newJoinSql = joinSql;

                if (joinSqlParams != null && joinSqlParams.Length > 0)
                {
                    foreach (object param in joinSqlParams)
                        parameters.Add(param);
                }
            }

            if (!string.IsNullOrWhiteSpace(condSql))
                newCondSql = "AND ( " + condSql + " ) ";

            if (condSql != null && condSqlParams != null && condSqlParams.Length > 0)
            {
                foreach (object param in condSqlParams)
                    parameters.Add(param);
            }

            defaultQueryCond = CreateDefaultQueryCond(parameters);

            if (!string.IsNullOrWhiteSpace(defaultQueryCond))
                newDefaultCondSql = " AND (" + defaultQueryCond + " )";

            sql += " " + newJoinSql + whereSql + newCondSql + newDefaultCondSql;
            dt = sqlHelper.ExecuteDataTable(con, trans, sql, parameters.ToArray());

            return dt;
        }


        /// <summary>
        /// 查询语句，支持分页，连接，条件查询
        /// </summary>
        /// <param name="fieldSqls">字段语句(ex1: column_name1,NAME,SEX  // ex2: COUNT(column_name) )</param>
        /// <param name="condSql">条件语句(ex: ID = 'xxxx' and NAME = 'n')</param>
        /// <param name="condParams">条件语句参数</param>
        /// <param name="joinSql">连接语句（ex: LEFT JOIN TABLE2 ON TABLE1.ID=TABLE2.ID ）</param>
        /// <param name="joinSqlParams">连接语句参数</param>
        /// <param name="startIdx">条目起始位置</param>
        /// <param name="endIdx">条目结束位置</param>
        /// <param name="sortFieldSql">所有条目根据指定字段排序(ex: ID asc)</param>
        /// <returns></returns>
        private DataTable QueryData(
            string fieldSql = null,
            string condSql = null, object[] condSqlParams = null,
            string joinSql = null, object[] joinSqlParams = null,
            int startIdx = -1, int endIdx = -1, string sortFieldSql = null)
        {
            if (startIdx < 0 || endIdx < 0)
            {
                return QueryData(fieldSql, condSql, condSqlParams, joinSql, joinSqlParams);
            }
            else
            {
                return QueryDataPage(fieldSql, condSql, condSqlParams, startIdx, endIdx, sortFieldSql, joinSql, joinSqlParams);
            }
        }


        #endregion

        #region 删除

        public int DeleteData()
        {
            int ret = 0;

            if (deleteCondSqlc != null)
            {
                deleteCondSql = new SqlStr();
                deleteCondSql.sql = deleteCondSqlc;
            }

            if (deleteCondObj != null)
                ret = DeleteData(deleteCondObj);
            else
                ret = DeleteData(deleteCondSql);

            deleteCondSqlc = null;
            deleteCondSql = null;
            deleteCondObj = default(T);

            return ret;
        }

        public int DeleteData(T condObj)
        {
            SqlStr[] newAdditionalSqls = CreateCondObjectToSql(condObj, null, PreciseOrFuzzyType.Precise);
            return DeleteData(newAdditionalSqls);
        }

        public int DeleteData(SqlStr condSql)
        {
            SqlStr[] sqls = null;
            if (condSql != null)
                sqls = new SqlStr[] { condSql };

            return DeleteData(sqls);
        }

        private int DeleteData(SqlStr[] condSql)
        {
            string cond = "";
            List<object> condParams = new List<object>();

            if (condSql == null || condSql.Length <= 0)
                return 0;

            for (int i = 0; i < condSql.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(cond))
                    cond += " " + condSql[i].logicalRelation + " ";

                cond += "(" + condSql[i].sql + ")";

                if (condSql[i].sqlParams != null)
                {
                    for (int j = 0; j < condSql[i].sqlParams.Length; j++)
                        condParams.Add(condSql[i].sqlParams[j]);
                }
            }

            return DeleteData(cond, condParams.ToArray());
        }

        private int DeleteData(string cond, object[] condParams)
        {
            string sql = @"DELETE FROM " + tbName;
            string whereSql = @" WHERE 1=1 ";
            string newCondSql = "";
            string defaultCond;
            string newDefaultCondSql = "";

            ArrayList parameters = new ArrayList();

            if (condParams != null && condParams.Length > 0)
            {
                foreach (object param in condParams)
                {
                    if (param != null)
                        parameters.Add(param);
                }
            }

            if (!string.IsNullOrWhiteSpace(cond))
                newCondSql = "AND ( " + cond + " ) ";

            defaultCond = CreateDefaultQueryCond(parameters);

            if (!string.IsNullOrWhiteSpace(defaultCond))
                newDefaultCondSql = " AND (" + defaultCond + " )";


            sql += whereSql + newCondSql + newDefaultCondSql;
            int result = sqlHelper.ExecuteNoneQuery(con, trans, sql, parameters.ToArray());

            return result;
        }

 
      
        #endregion

        #region 更新

        public int UpdateData()
        {
            int ret = 0;

            if (updateCondSqlc != null)
            {
                updateCondSql = new SqlStr();
                updateCondSql.sql = updateCondSqlc;
            }

            if (updateObj != null)
               ret = UpdateData(updateObj, updateCondSql);

            ResetUpdateSqlParams();

            return ret;
        }

        public int UpdateData(T updateObj, SqlStr condSql)
        {
            SqlStr[] condSqls = null;
            if (condSql != null)
                condSqls = new SqlStr[] { condSql };

            return UpdateData(updateObj, condSqls);
        }


        private int UpdateData(string[] updateFields, object[] updateValues, string cond, object[] condParams)
        {
            string sql = @"UPDATE " + tbName + @" SET ";
            string whereSql = @" WHERE 1=1 ";
            string updateDataSql = "";
            int ret;
            string newCondSql = "";
            string defaultCond;
            string newDefaultCondSql = "";
            ArrayList parameters = new ArrayList();

            if (updateFields.Length != updateValues.Length)
                return 0;

            if (updateValues != null && updateValues.Length > 0)
            {
                foreach (object value in updateValues)
                    parameters.Add(value);
            }

            if (condParams != null && condParams.Length > 0)
            {
                foreach (object param in condParams)
                    parameters.Add(param);
            }

            for (int i = 0; i < updateFields.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(updateDataSql))
                    updateDataSql += " , ";

                updateDataSql += updateFields[i] + "=@" + updateFields[i];
            }


            if (!string.IsNullOrWhiteSpace(cond))
                newCondSql = "AND ( " + cond + " ) ";

            defaultCond = CreateDefaultQueryCond(parameters);

            if (!string.IsNullOrWhiteSpace(defaultCond))
                newDefaultCondSql = " AND (" + defaultCond + " )";


            sql += updateDataSql + whereSql + newCondSql + newDefaultCondSql;
            ret = sqlHelper.ExecuteNoneQuery(con, trans, sql, parameters.ToArray());

            return ret;
        }

        private int UpdateData(T updateObj, string cond, object[] condParams)
        {
            Type type = updateObj.GetType(); //获取类型
            FieldInfo fieldInfo;
            string field;
            object propValue;

            List<string> updateFieldList = new List<string>();
            List<object> updateValueList = new List<object>();

            for (int i = 0; i < fields.Length; i++)
            {
                field = fields[i];
                fieldInfo = type.GetField(field); //获取指定名称的属性
                propValue = fieldInfo.GetValue(updateObj);

                if (IsUsedPrecise(fieldInfo, propValue, field))
                {
                    updateFieldList.Add(field);
                    updateValueList.Add(propValue);
                }
            }

            return UpdateData(updateFieldList.ToArray(), updateValueList.ToArray(), cond, condParams);
        }

       
        private int UpdateData(T updateObj, SqlStr[] condSql)
        {
            if (updateObj == null)
                return 0;

            string cond = "";
            List<object> condParams = new List<object>();

            for (int i = 0; i < condSql.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(cond))
                    cond += " " + condSql[i].logicalRelation + " ";

                cond += "(" + condSql[i].sql + ")";


                if (condSql[i].sqlParams != null)
                {
                    for (int j = 0; j < condSql[i].sqlParams.Length; j++)
                        condParams.Add(condSql[i].sqlParams[j]);
                }
            }

            return UpdateData(updateObj, cond, condParams.ToArray());
        }


        #endregion

        #region 插入
        public int InsertData(T insertObj)
        {
            Type type = insertObj.GetType(); //获取类型
            FieldInfo fieldInfo;
            string field;
            object propValue;
            List<object> valueList = new List<object>();
            List<string> insertFieldList = new List<string>();
            List<object> insertValueList = new List<object>();

            for (int i = 0; i < fields.Length; i++)
            {
                field = fields[i];

                fieldInfo = type.GetField(field); //获取指定名称的属性
                propValue = fieldInfo.GetValue(insertObj);

                if (IsUsedPrecise(fieldInfo, propValue, field))
                {
                    insertFieldList.Add(field);
                    insertValueList.Add(propValue);
                }
            }

            return InsertData(insertFieldList.ToArray(), insertValueList.ToArray());
        }

        private int InsertData(string[] insertFields, object[] insertValues)
        {
            string sql = @"INSERT INTO " + tbName;
            string insertFieldsSql = "";
            string insertValuesSql = "";
            ArrayList parameters = new ArrayList();


            if (insertFields.Length == 0 ||
                insertFields.Length != insertValues.Length)
                return 0;


            insertFieldsSql += insertFields[0];
            insertValuesSql += "@" + insertFields[0];

            for (int i = 1; i < insertFields.Length; i++)
            {
                insertFieldsSql += " , " + insertFields[i];
                insertValuesSql += " , " + "@" + insertFields[i];
            }

            if (insertValues != null && insertValues.Length > 0)
            {
                foreach (object value in insertValues)
                    parameters.Add(value);
            }

            sql += " ( " + insertFieldsSql + " ) VALUES( " + insertValuesSql + " ) ";

            return sqlHelper.ExecuteNoneQuery(con, trans, sql, parameters.ToArray());
        }

        #endregion

       
        public void AddDefaultCondField(string field, object val1, object val2 = null, int type = 0)
        {
            DefaultCondField defField = new DefaultCondField();
            defField.field = field;
            defField.value1 = val1;
            defField.value2 = val2;
            defField.type = type;
            defCondFieldList.Add(defField);
        }

        private string CreateDefaultQueryCond(ArrayList parameters)
        {
            string defaultCond = "";

            if (!IsUsedDefaultCond)
                return defaultCond;

            for (int i = 0; i < defCondFieldList.Count; i++)
            {
                if (FindField(defCondFieldList[i].field) == true)
                {
                    defaultCond += " " + defCondFieldList[i].field + " = @" + defCondFieldList[i].field + "_DEFAULT ";
                    parameters.Add(defCondFieldList[i].value1);
                }
            }

            return defaultCond;
        }


        private bool FindField(string findField)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                if (findField == fields[i])
                    return true;
            }

            return false;
        }


        private int FindDefaultCondFieldByType(int type)
        {
            for (int i = 0; i < defCondFieldList.Count; i++)
            {
                if (type == defCondFieldList[i].type)
                    return i;
            }

            return -1;
        }


        /// <summary>
        /// 生成精确或模糊SQL条件语句
        /// </summary>
        /// <param name="condObj"></param>
        /// <param name="excludeFields">个别字段相反的查询模式</param>
        /// <param name="preciseOrFuzzy">生成精确或模糊SQL条件语句 0：精确， 1：模糊</param>
        /// <returns></returns>
        private SqlStr[] CreateCondObjectToSql(T condObj, string excludeField, PreciseOrFuzzyType preciseOrFuzzy)
        {
            if (condObj == null)
                return null;

            string field;
            Type type = condObj.GetType(); //获取类型
            FieldInfo fieldInfo;
            object propValue;
            SqlStr additionalSql;
            List<SqlStr> additionalSqlList = new List<SqlStr>();
            bool isExclude = false;

            string[] excludeFields = null;
            if (excludeField != null)
                excludeFields = excludeField.Split(',');

            for (int i = 0; i < fields.Length; i++)
            {
                field = fields[i];
                fieldInfo = type.GetField(field); //获取指定名称的属性
                propValue = fieldInfo.GetValue(condObj);


                //判断当前查询字段是否在模糊字段列表中
                if (excludeFields != null && excludeFields.Length > 0)
                {
                    for (int j = 0; j < excludeField.Length; j++)
                    {
                        if (field.Equals(excludeField[j]))
                        {
                            isExclude = true;
                            break;
                        }
                    }
                }

                if (isExclude) //被排除在查询模式之外
                {
                    if (preciseOrFuzzy == PreciseOrFuzzyType.Precise) //精确查询类型
                    {
                        string cond = IsUsedFuzzy(fieldInfo, propValue, field);
                        if (cond != null)
                        {
                            additionalSql = new SqlStr();
                            additionalSql.sql = cond;
                            additionalSqlList.Add(additionalSql);
                            cond = null;
                        }
                    }
                    else  //模糊查询类型
                    {
                        if (IsUsedPrecise(fieldInfo, propValue, field))
                        {
                            additionalSql = new SqlStr();
                            additionalSql.sql = field + " =@" + field;
                            additionalSql.sqlParams = new object[1] { propValue };
                            additionalSqlList.Add(additionalSql);
                        }
                    }

                    isExclude = false;
                }
                else //未被排除
                {
                    if (preciseOrFuzzy == PreciseOrFuzzyType.Precise) //精确查询类型
                    {
                        if (IsUsedPrecise(fieldInfo, propValue, field))
                        {
                            additionalSql = new SqlStr();
                            additionalSql.sql = field + " =@" + field;
                            additionalSql.sqlParams = new object[1] { propValue };
                            additionalSqlList.Add(additionalSql);
                        }
                    }
                    else
                    {
                        string cond = IsUsedFuzzy(fieldInfo, propValue, field);
                        if (cond != null)
                        {
                            additionalSql = new SqlStr();
                            additionalSql.sql = cond;
                            additionalSqlList.Add(additionalSql);
                            cond = null;
                        }
                    }
                }
            }

            return additionalSqlList.ToArray();
        }

        /// <summary>
        /// 字段是否可以进行精确SQL语句或者字段是否可以被使用
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="propValue"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private bool IsUsedPrecise(FieldInfo fieldInfo, object propValue, string field)
        {
            if (fieldVaildValueDict.ContainsKey(field) == false)
            {
                if (fieldInfo.FieldType == typeof(string))
                {
                    string value = (string)propValue;

                    if (!string.IsNullOrWhiteSpace(value))
                        return true;
                }
                else if (fieldInfo.FieldType == typeof(int))
                {
                    int value = (int)propValue;

                    if (value < int.MaxValue && value > int.MinValue)
                        return true;
                }
                else if (fieldInfo.FieldType == typeof(long))
                {
                    long value = (long)propValue;

                    if (value < long.MaxValue && value > long.MinValue)
                        return true;
                }
                else if (fieldInfo.FieldType == typeof(decimal))
                {
                    decimal value = (decimal)propValue;

                    if (value < decimal.MaxValue && value > decimal.MinValue)
                        return true;
                }
                else if (fieldInfo.FieldType == typeof(DateTime))
                {
                    DateTime value = (DateTime)propValue;

                    if (value != DateTime.MinValue && value != DateTime.MaxValue)
                        return true;
                }
            }
            else
            {
                object value = fieldVaildValueDict[field];
                if (propValue != value)
                    return true;
            }

            return false;
        }


        /// <summary>
        /// 字段是否可以进行模糊查询
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="propValue"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private string IsUsedFuzzy(FieldInfo fieldInfo, object propValue, string field)
        {
            string cond = null;

            if (fieldVaildValueDict.ContainsKey(field) == false)
            {
                if (fieldInfo.FieldType == typeof(string))
                {
                    string value = (string)propValue;

                    if (!string.IsNullOrWhiteSpace(value))
                        cond = field + " LIKE " + "'%" + value + "%'";

                    return cond;
                }
                else if (fieldInfo.FieldType == typeof(int))
                {
                    int value = (int)propValue;

                    if (value < int.MaxValue && value > int.MinValue)
                        cond = field + " LIKE " + "'%" + value + "%'";

                    return cond;
                }
                else if (fieldInfo.FieldType == typeof(long))
                {
                    long value = (long)propValue;

                    if (value < long.MaxValue && value > long.MinValue)
                        cond = field + " LIKE " + "'%" + value + "%'";

                    return cond;
                }
                else if (fieldInfo.FieldType == typeof(decimal))
                {
                    decimal value = (decimal)propValue;

                    if (value < decimal.MaxValue && value > decimal.MinValue)
                        cond = field + " LIKE " + "'%" + value + "%'";

                    return cond;
                }
                else if (fieldInfo.FieldType == typeof(DateTime))
                {
                    DateTime value = (DateTime)propValue;

                    if (value != DateTime.MinValue && value != DateTime.MaxValue)
                        cond = field + " = " + value;

                    return cond;
                }
            }
            else
            {
                object value = fieldVaildValueDict[field];
                if (propValue != value)
                    cond = field + " = " + value;
            }

            return cond;
        }


        private object GetDefaultPropertyValue(FieldInfo fieldInfo)
        {
            object defaultValue = null;

            if (fieldInfo.FieldType == typeof(string))
            {
                defaultValue = null;
            }
            else if (fieldInfo.FieldType == typeof(int))
            {
                defaultValue = int.MinValue;
            }
            else if (fieldInfo.FieldType == typeof(decimal))
            {
                defaultValue = decimal.MinValue;
            }
            else if (fieldInfo.FieldType == typeof(DateTime))
            {
                defaultValue = DateTime.MinValue;
            }

            return defaultValue;
        }

    }

    public class SqlStr
    {
        public string sql;
        public object[] sqlParams;
        public string logicalRelation = "and";
    }

    public class DefaultCondField
    {
        public string field;
        public object value1;
        public object value2;
        public int type = 0;
    }


    /// <summary>
    /// 模糊查询，精确查询
    /// </summary>
    public enum PreciseOrFuzzyType
    {
        /// <summary>
        /// 精确查询
        /// </summary>
        Precise,

        /// <summary>
        /// 模糊查询
        /// </summary>
        Fuzzy
    }

}

