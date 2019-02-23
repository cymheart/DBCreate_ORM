using System.Collections.Generic; 
 using System.Data.SqlClient; 

namespace DataBaseOp
{ 
    public class ScDB
    { 
         static public Dictionary<string, ScDataBaseTableOpBase> TableOpDict = new Dictionary<string, ScDataBaseTableOpBase>();

        static public ScDBTestDataBase1 DB_TestDataBase1; 

        static public void Init(string[] dbNamesMatch, string[] conStrsMatch) 
        {
            DB_TestDataBase1 = new ScDBTestDataBase1("TestDataBase1","server=.;user id = sa; password=11111111;database=TestDataBase1;Max Pool Size=50;Min Pool Size=2"); 
            TableOpDict.Add(DB_TestDataBase1.dbName ,DB_TestDataBase1); 
            DB_TestDataBase1.ResetMatchConnection(dbNamesMatch, conStrsMatch); 
            DB_TestDataBase1.CreateDataTableOp(); 

        }

        static public void Dispose() 
        {
            DB_TestDataBase1.Dispose(); 
        }

    }
 }

