using System.Collections.Generic; 
 using System.Data.SqlClient; 

namespace DataBaseOp
{ 
    public class dborm
    { 
         static public Dictionary<string, ScDataBaseTableOpBase> TableOpDict = new Dictionary<string, ScDataBaseTableOpBase>();

        static public dbormTestDataBase1 DB_TestDataBase1; 
        static public dbormTestDataBase2 DB_TestDataBase2; 

        static public void Init(string[] dbNamesMatch, string[] conStrsMatch) 
        {
            DB_TestDataBase1 = new dbormTestDataBase1("TestDataBase1","server=.;user id = sa; password=11111111;database=TestDataBase1;Max Pool Size=50;Min Pool Size=2"); 
            TableOpDict.Add(DB_TestDataBase1.dbName ,DB_TestDataBase1); 
            DB_TestDataBase1.ResetMatchConnection(dbNamesMatch, conStrsMatch); 
            DB_TestDataBase1.CreateDataTableOp(); 

            DB_TestDataBase2 = new dbormTestDataBase2("TestDataBase2","server=.;user id = sa; password=11111111;database=TestDataBase2;Max Pool Size=50;Min Pool Size=2"); 
            TableOpDict.Add(DB_TestDataBase2.dbName ,DB_TestDataBase2); 
            DB_TestDataBase2.ResetMatchConnection(dbNamesMatch, conStrsMatch); 
            DB_TestDataBase2.CreateDataTableOp(); 

        }

        static public void Dispose() 
        {
            DB_TestDataBase1.Dispose(); 
            DB_TestDataBase2.Dispose(); 
        }

    }
 }

