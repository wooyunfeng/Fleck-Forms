using System;
using System.Collections.Generic;
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
            catch
            {
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
                throw;
            }
        }

        public int SQLite_InsertCommand(string[] param)
        {
            if (DateString != DateTime.Now.ToString("yyyyMMdd"))
            {
                DateString =DateTime.Now.ToString("yyyyMMdd");
                string sql = "CREATE TABLE IF NOT EXISTS chess" + DateString + "(id integer PRIMARY KEY UNIQUE, revTime varchar(20),Address varchar(20), command varchar(255), dealTime varchar(20),dealType integer(1),result varchar(50));";//建表语句    
                SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
                cmdCreateTable.ExecuteNonQuery();//如果表不存在，创建数据表  
            }

            try
            {
                SQLiteCommand cmdInsert = new SQLiteCommand(conn);
                cmdInsert.CommandText = String.Format("INSERT INTO chess" + DateString + "(revTime, Address, command) VALUES('{0}', '{1}','{2}')", param[0], param[1], param[2]);//插入几条数据    
                return cmdInsert.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public int SQLite_UpdateCommand(int dealType, string result, string address, string command)
        {
            try
            {
                string dealTime = DateTime.Now.ToLongTimeString();
                SQLiteCommand cmdInsert = new SQLiteCommand(conn);
                cmdInsert.CommandText = String.Format("UPDATE  chess{0} set dealType = {1},result = '{2}' , dealTime = '{3}' where Address = '{4}' and command = '{5}'", DateString, dealType, result, dealTime, address, command);    
                return cmdInsert.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public int SQLite_Login(string address)
        {
            try
            {
                DateTime dealTime = DateTime.Now;
                SQLiteCommand cmdInsert = new SQLiteCommand(conn);
                cmdInsert.CommandText = String.Format("INSERT INTO Login(Address, Connected) VALUES('{0}', '{1}')", address, dealTime);//插入几条数据    
                return cmdInsert.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public int SQLite_Logout(string address)
        {
            try
            {
                DateTime dealTime = DateTime.Now;
                SQLiteCommand cmdInsert = new SQLiteCommand(conn);
                cmdInsert.CommandText = String.Format("UPDATE  Login set Closed = '{0}' where Address = '{1}'", dealTime, address);
                return cmdInsert.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public SQLiteDataReader SQLite_Query(string address)
        {
            try
            {
                string sql = String.Format("select * from chess{0} where Address = '{1}'" ,DateString, address); ;
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
