using System.Data.SqlClient; 

namespace DataBaseOp
{ 
    public class dbormTestDataBase2 : ScDataBaseTableOpBase
    { 
        public DataBaseAccess<DB_TestDataBase2_DT_TABLEA> DTOP_TABLEA; 
        public DataBaseAccess<DB_TestDataBase2_DT_TABLEB> DTOP_TABLEB; 

        public dbormTestDataBase2(string dbName, string conStr) 
        :base(dbName, conStr) 
        {
        }


        public void CreateDataTableOp() 
        {
            DTOP_TABLEA = new DataBaseAccess<DB_TestDataBase2_DT_TABLEA>(); 
            DTOP_TABLEA.ResetTableInfo("TABLEA", typeof(DB_TestDataBase2_DT_TABLEA)); 
            DTOP_TABLEA.con = con; 

            DTOP_TABLEB = new DataBaseAccess<DB_TestDataBase2_DT_TABLEB>(); 
            DTOP_TABLEB.ResetTableInfo("TABLEB", typeof(DB_TestDataBase2_DT_TABLEB)); 
            DTOP_TABLEB.con = con; 

        }

     }
}

