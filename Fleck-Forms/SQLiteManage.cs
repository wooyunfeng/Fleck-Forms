﻿using System;
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
            string sqlPosiotion = "CREATE TABLE IF NOT EXISTS Position (id integer PRIMARY KEY UNIQUE, Board varchar(255), visit integer);";//建表语句    
            positionSQLite.SQLite_CreateTable(sqlPosiotion);
            string sqlEngine = "CREATE TABLE IF NOT EXISTS Engine (id integer, depth integer, seldepth integer,multipv integer,score integer,nodes integer,nps integer,hashfull integer,tbhits integer,time integer,pv varchar(255));";//建表语句    
            positionSQLite.SQLite_CreateTable(sqlEngine);
            string sqlQueryall = "CREATE TABLE IF NOT EXISTS ChessDB (id integer, result varchar(4096), visit integer);";//建表语句    
            positionSQLite.SQLite_CreateTable(sqlQueryall);
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
            getOpenBook(board);
        }
        public void Update(int dealType, string result, string address, string command)
        {
            historySQLite.SQLite_UpdateCommand(dealType, result, address, command);
        }

        public object Query(string addr)
        {
            return historySQLite.SQLite_Query(addr);
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
                zobristKey = 2;
                SQLiteDataReader reader;
                double b_zobristKey = Convert.ToDouble(zobristKey);
               // string sql = String.Format("select * from bhobk where vkey = '{0}'", zobristKey);
                string sql = String.Format("select * from bhobk where id = '{0}'", zobristKey);
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
