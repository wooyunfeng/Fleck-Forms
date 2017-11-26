using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Collections;

namespace Fleck_Forms
{
    class SQLiteManage : IDataOperate
    {
        public SQLiteHelper historySQLite;
        public SQLiteHelper positionSQLite;
        public SQLiteHelper openbookSQLite;
        public SQLiteManage()
        {
            openbookSQLite = new SQLiteHelper("openbook.db");
            historySQLite = new SQLiteHelper(Setting.websocketPort + "history.db");
            string DateString = DateTime.Now.ToString("yyyyMMdd");
            string sql = "CREATE TABLE IF NOT EXISTS chess" + DateString + "(id integer PRIMARY KEY UNIQUE, revTime varchar(20),Address varchar(20), command varchar(255), dealTime varchar(20), dealType integer(1),result varchar(50));";//建表语句    
            historySQLite.SQLite_CreateTable(sql);
            string sqlLogin = "CREATE TABLE IF NOT EXISTS Login (id integer PRIMARY KEY UNIQUE, Address varchar(20), Connected DATETIME(20), Closed DATETIME(20));";//建表语句    
            historySQLite.SQLite_CreateTable(sqlLogin);
            positionSQLite = new SQLiteHelper("position.db");
            string sqlPosiotion = "CREATE TABLE IF NOT EXISTS board (id integer PRIMARY KEY UNIQUE, vkey integer, zobrist varchar(16), fen varchar(255), visit integer, lasttime DATETIME(20));";//建表语句    
            positionSQLite.SQLite_CreateTable(sqlPosiotion);
//             string sqlEngine = "CREATE TABLE IF NOT EXISTS Engine (id integer, depth integer, seldepth integer,multipv integer,score integer,nodes integer,nps integer,hashfull integer,tbhits integer,time integer,pv varchar(255));";//建表语句    
//             positionSQLite.SQLite_CreateTable(sqlEngine);
//             string sqlQueryall = "CREATE TABLE IF NOT EXISTS ChessDB (id integer, result varchar(4096), visit integer);";//建表语句    
//             positionSQLite.SQLite_CreateTable(sqlQueryall);
        }


        public void Login(string p)
        {
            historySQLite.SQLite_Login(p);
        }

        public void Logout(string p)
        {
            historySQLite.SQLite_Logout(p);
        }

        public void Insert(string[] param)
        {
            historySQLite.SQLite_InsertCommand(param);
        }
        public void InsertBoard(string board)
        {
            try
            {
                Zobrist zobrist = new Zobrist();
                ulong zobristKey = zobrist.getKey(board);
                string sql = String.Format("select * from board where zobrist = '{0}'", zobristKey.ToString("X"));
                SQLiteDataReader reader = positionSQLite.SQLite_ExecuteReader(sql);
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        //do something
                        int count = reader.GetInt32(4) + 1;
                        DateTime dealTime = DateTime.Now;
                        sql = String.Format("UPDATE  board set visit = '{0}', lasttime = '{1}' where zobrist = '{2}'", count, dealTime, zobristKey.ToString("X"));
                        positionSQLite.SQLite_ExecuteNonQuery(sql);
                    }
                    else
                    {
                        DateTime dealTime = DateTime.Now;
                        sql = String.Format("INSERT INTO board(vkey, zobrist, fen, visit, lasttime) VALUES('{0}', '{1}','{2}',1, '{3}')", zobristKey, zobristKey.ToString("X"), board, dealTime);//插入几条数据
                        positionSQLite.SQLite_ExecuteNonQuery(sql);
                    }
                }          
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void Update(int dealType, string result, string address, string command)
        {
            historySQLite.SQLite_UpdateCommand(dealType, result, address, command);
        }

        public object Query(string addr)
        {
            return historySQLite.SQLite_QueryAddress(addr);
        }

        public void InsertQueryall(string board, string strQueryall)
        {
        }

        public void setItemToDepthinfo(int id, string message)
        {

        }
        public int getBoardID(string board)
        {
            return 0;
        }
        public object getOpenBook(string board)
        {            
            try
            {
                Zobrist zobrist = new Zobrist();
                ulong zobristKey = zobrist.getKey(board);               
                SQLiteDataReader reader;
                double b_zobristKey = Convert.ToDouble(zobristKey);
                string sql = String.Format("select * from bhobk where vkey = '{0}'", zobristKey);
               // zobristKey = 2;
               // string sql = String.Format("select * from bhobk where id = '{0}'", zobristKey);
                SQLiteCommand command = new SQLiteCommand(sql, openbookSQLite.conn);
                reader = command.ExecuteReader();
                ArrayList list = new ArrayList();
                while (reader.Read())
                {
                    COpenBook openbook = new COpenBook();
                    openbook.id = (Int64)reader["id"];
                    object vkey = reader["vkey"];
                   // openbook.vkey = (Int64)reader["vkey"];
                    openbook.vmove = (Int64)reader["vmove"]-0x3333;
                    openbook.vscore = (Int64)reader["vscore"];
                    openbook.vwin = (Int64)reader["vwin"];
                    openbook.vdraw = (Int64)reader["vdraw"];
                    openbook.vlost = (Int64)reader["vlost"];
                    openbook.vvalid = (Int64)reader["vvalid"];
                 //   openbook.vmemo = reader["vmemo"].ToString();
                    openbook.vindex = (Int64)reader["vindex"];
                    list.Add(openbook);
                }
                reader.Close();
                return list;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
