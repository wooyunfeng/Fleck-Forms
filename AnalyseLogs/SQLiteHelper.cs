using System;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Fleck_Forms
{
    class SQLiteHelper
    {
        public SQLiteConnection conn;
        private string DateString;
        public SQLiteHelper(string filename)
        {
            SQLite_Init(filename);
        }

        public void SQLite_Init(string filename)
        {
            string strSQLiteDB = Environment.CurrentDirectory;
            try
            {
                string dbPath = "Data Source=" + strSQLiteDB + "\\"+filename;
                conn = new SQLiteConnection(dbPath);//创建数据库实例，指定文件位置    
                conn.Open();                        //打开数据库，若文件不存在会自动创建    
                DateString = DateTime.Now.ToString("yyyyMMdd"); 
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

        public int SQLite_InsertCommand(string[] param)
        {
            try
            {
                SQLiteCommand cmdInsert = new SQLiteCommand(conn);
                cmdInsert.CommandText = String.Format("INSERT INTO chess" + "(_index, UUID, command) VALUES('{0}', '{1}','{2}')", param[0], param[1], param[2]);//插入几条数据    
                return cmdInsert.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public int SQLite_UpdateCommand(string uuid, string index, string result)
        {
            try
            {
                SQLiteCommand cmdInsert = new SQLiteCommand(conn);
                cmdInsert.CommandText = String.Format("UPDATE  chess set result = '{0}'  where uuid = '{1}' and _index = '{2}'", result, uuid, index);    
                return cmdInsert.ExecuteNonQuery();
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

        public SQLiteDataReader SQLite_QueryAddress(string address)
        {
            try
            {
                string sql = String.Format("select * from chess{0} where Address = '{1}'" ,DateString, address);
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

        internal string getCommand(string uuid, string index)
        {
            try
            {
                string sql = String.Format("select * from chess where _index = {0} and UUID = '{1}'", index, uuid);
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return reader["command"].ToString();    
                }
                return "";
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
