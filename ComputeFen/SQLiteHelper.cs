using System;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace ComputeFen
{
    class SQLiteHelper
    {
        public SQLiteConnection conn;
        public SQLiteHelper(string filename)
        {
            SQLite_Init(filename);
        }

        public void SQLite_Init(string filename)
        {
            string strSQLiteDB = Environment.CurrentDirectory;
            try
            {
                string dbPath = "Data Source=" + filename;
                conn = new SQLiteConnection(dbPath);//创建数据库实例，指定文件位置    
                conn.Open();                        //打开数据库，若文件不存在会自动创建    
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void SQLite_CreateTable(string sql)
        {
            try
            {
                SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
                cmdCreateTable.ExecuteNonQuery();//如果表不存在，创建数据表 
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

       
        public int SQLite_ExecuteNonQuery(string sql)
        {
            try
            {               
                SQLiteCommand cmdInsert = new SQLiteCommand(conn);
                cmdInsert.CommandText = sql;
                return cmdInsert.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public SQLiteDataReader SQLite_ExecuteReader(string sql)
        {
            try
            {              
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                return reader;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
       
       
    }
}
