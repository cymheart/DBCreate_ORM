using System.Data.SqlClient; 

namespace DataBaseOp
{ 
    public class dbormTestDataBase1 : ScDataBaseTableOpBase
    { 
        public DataBaseAccess<DB_TestDataBase1_DT_TABLE1> DTOP_TABLE1; 
        public DataBaseAccess<DB_TestDataBase1_DT_TABLE2> DTOP_TABLE2; 

        public dbormTestDataBase1(string dbName, string conStr) 
        :base(dbName, conStr) 
        {
        }


        public void CreateDataTableOp() 
        {
            DTOP_TABLE1 = new DataBaseAccess<DB_TestDataBase1_DT_TABLE1>(); 
            DTOP_TABLE1.ResetTableInfo("TABLE1", typeof(DB_TestDataBase1_DT_TABLE1)); 
            DTOP_TABLE1.con = con; 

            DTOP_TABLE2 = new DataBaseAccess<DB_TestDataBase1_DT_TABLE2>(); 
            DTOP_TABLE2.ResetTableInfo("TABLE2", typeof(DB_TestDataBase1_DT_TABLE2)); 
            DTOP_TABLE2.con = con; 

        }

     }
}

