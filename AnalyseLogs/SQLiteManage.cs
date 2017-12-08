using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Collections;

namespace Fleck_Forms
{
    class SQLiteManage
    {
        public SQLiteHelper historySQLite;
        public SQLiteHelper positionSQLite;
        public SQLiteManage()
        {
            historySQLite = new SQLiteHelper("history.db");
            string sql = "CREATE TABLE IF NOT EXISTS chess" + "(id integer PRIMARY KEY UNIQUE, _index varchar(20), UUID varchar(50), command varchar(255), result varchar(50));";//建表语句    
            historySQLite.SQLite_CreateTable(sql);
        }

        public void Insert(string[] param)
        {
            historySQLite.SQLite_InsertCommand(param);
        }

        public void Update(string uuid, string index, string result)
        {
            historySQLite.SQLite_UpdateCommand(uuid, index, result);
        }

        public object Query(string addr)
        {
            return historySQLite.SQLite_QueryAddress(addr);
        }

        internal string getCommand(string uuid, string index)
        {
            return historySQLite.getCommand(uuid, index);
        }
    }
}
