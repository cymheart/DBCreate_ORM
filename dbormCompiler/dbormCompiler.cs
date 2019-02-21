using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using App;

namespace ORM_DBOP_COMPLER
{
    public class dbormCompiler
    {
        static string[] dbNames;
        static string[] conStrs;

        static string temp = Directory.GetCurrentDirectory(); //System.Environment.GetEnvironmentVariable("TEMP");
        static string programDirPath = temp + @"\programDB\";
        static string dataBaseOpDirPath = temp + @"\programDB\DataBaseOp\";
        static string dbCreateDirPath = temp + @"\programDB\DbCreate\";

        static string appDir = System.AppDomain.CurrentDomain.BaseDirectory;

        static bool isRebulidTableModel = false;
        static string dllPath = temp + "\\";

        static FileAttributes fileAttributes =  FileAttributes.Normal;
        public static void DynamicCompiler(string[] dbnames, string[] constrs, bool _isRebulidTableModel = false, string createFilePath = null, bool isDeleteFile = true)
        {
            isRebulidTableModel = _isRebulidTableModel;
            dbNames = dbnames;
            conStrs = constrs;

            if (createFilePath != null)
                dllPath = createFilePath;


            string[] files = GetFileNames(dataBaseOpDirPath);

            if (files != null)
            {
                foreach (string fi in files)
                    DeleteFile(fi);

                if (isRebulidTableModel)
                {
                    DeleteFile(dataBaseOpDirPath);
                }
            }

            if (!Directory.Exists(programDirPath))
                Directory.CreateDirectory(programDirPath);
            File.SetAttributes(programDirPath, fileAttributes);

            if (!Directory.Exists(dataBaseOpDirPath))
                Directory.CreateDirectory(dataBaseOpDirPath);
            File.SetAttributes(dataBaseOpDirPath, fileAttributes);

            if (!Directory.Exists(dbCreateDirPath))
                Directory.CreateDirectory(dbCreateDirPath);
            File.SetAttributes(dbCreateDirPath, fileAttributes);

   
            string scDbTableOpBase = App.Properties.Resources.ScDataBaseTableOpBase;
            string dataBaseAccess = App.Properties.Resources.DataBaseAccess;
            string dbCreate = App.Properties.Resources.DbCreate;
            string sqlHelper = App.Properties.Resources.SqlHelper;

            WriteStreamToFile(scDbTableOpBase, dataBaseOpDirPath + "ScDataBaseTableOpBase.cs");
            WriteStreamToFile(dataBaseAccess, dataBaseOpDirPath + "DataBaseAccess.cs");
            WriteStreamToFile(sqlHelper, dataBaseOpDirPath + "SqlHelper.cs");
            WriteStreamToFile(dbCreate, dbCreateDirPath + "DbCreate.cs");
       
            Compiler(null);
            Compiler2(null);

            if (isDeleteFile)
            {
                DeleteFile(dbCreateDirPath + "DbCreate.cs");
                files = GetFileNames(dataBaseOpDirPath);
                foreach (string fi in files)
                    DeleteFile(fi);
            }
        }



        static void Compiler(string[] args)
        {
            // 1.CSharpCodePrivoder
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();

            // 2.CompilerParameters
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Data.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;

            // 3.CompilerResults
            CompilerResults cr = objCSharpCodePrivoder.CompileAssemblyFromFile(objCompilerParameters, dbCreateDirPath + @"DbCreate.cs");

            if (cr.Errors.HasErrors)
            {
                Console.WriteLine("编译错误：");
                foreach (CompilerError err in cr.Errors)
                {
                    Console.WriteLine(err.ErrorText);
                }
            }
            else
            {
                // 通过反射，调用实例
                Assembly objAssembly = cr.CompiledAssembly;
                object objDbCreate = objAssembly.CreateInstance("DbInfoCreate.DbCreate");
                MethodInfo objMI = objDbCreate.GetType().GetMethod("DbCreateInfos");
                objMI.Invoke(objDbCreate, new object[] { dbNames, conStrs, isRebulidTableModel});
            }
        }

     
        static void Compiler2(string[] args)
        {
            // 1.CSharpCodePrivoder
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();

            // 2.CompilerParameters
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Data.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.XML.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.XML.Serialization.dll");
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = false;
            objCompilerParameters.OutputAssembly = dllPath + "dborm.dll";

            Console.WriteLine(objCompilerParameters.OutputAssembly);


            string[] codefiles = CreateCodeFileList();
          //  string[] codes = GenerateAllCode(codefiles);

            // 3.CompilerResults
            CompilerResults cr = objCSharpCodePrivoder.CompileAssemblyFromFile(objCompilerParameters, codefiles);


            if (cr.Errors.HasErrors)
            {
                Console.WriteLine("编译错误：");
                foreach (CompilerError err in cr.Errors)
                {
                    Console.WriteLine(err.ErrorText);
                }
            }  
        }

        static string[] CreateCodeFileList()
        {
            string directoryPath = dataBaseOpDirPath;
            string[] fileNames = null;
            List<string> codeFilePathList = new List<string>();

            if (Directory.Exists(directoryPath))
            {
                //判断是否存在文件  
                fileNames = GetFileNames(directoryPath);
            }

            codeFilePathList.AddRange(fileNames);
            CreateChildDirFile(codeFilePathList, directoryPath);

            return codeFilePathList.ToArray();
        }

        static string[] GenerateAllCode(string[] codefiles)
        {
            List<string> codeList = new List<string>();

            foreach(string codefile in codefiles)
            {
                codeList.Add(GenerateCode(codefile));
            }

            return codeList.ToArray();
        }


        static string GenerateCode(string codefile)
        {
            StreamReader sr = new StreamReader(codefile, Encoding.Default);
            string line;
            string code = "";
            while ((line = sr.ReadLine()) != null)
            {
                code += line.ToString() + "\r\n";
            }

            return code;
        }

        static void CreateChildDirFile(List<string> filepaths, string childDir)
        {
            string[] childDirs;
            string[] childFiles;
            childDirs = Directory.GetDirectories(childDir);

            foreach(string dir in childDirs)
            {
                childFiles = GetFileNames(dir);
                filepaths.AddRange(childFiles);
                CreateChildDirFile(filepaths, dir);
            }
        }


        static string[] GetFileNames(string directoryPath)
        {
            //如果目录不存在，则抛出异常  
            if (!Directory.Exists(directoryPath))
            {
                return null;
            }

            //获取文件列表  
            return Directory.GetFiles(directoryPath);
        }

        static public void DeleteFile(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if (attr.HasFlag(FileAttributes.Directory) && Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else if (!attr.HasFlag(FileAttributes.Directory) && File.Exists(path))
            {
                File.Delete(path);
            }
        }

        static void WriteStreamToFile(string stream, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            sw.WriteLine(stream);
            sw.Close();
        }
    }
}
