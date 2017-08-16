using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Fleck_Forms
{
    class MySqlManage : IDataOperate
    {
        private MySqlHelper mychessdb;
        private string DateString;
        public MySqlManage()
        {
            String connectionString = "server = rm-bp17csiteni8i84o3.mysql.rds.aliyuncs.com; database = mychessdb; user id = root; password = Jiao19890228;";
            mychessdb = new MySqlHelper(connectionString);
            DateString = DateTime.Now.ToString("yyyyMMdd");
            string sql = "CREATE TABLE IF NOT EXISTS chess" + DateString + "(id int(11) NOT NULL AUTO_INCREMENT, revTime varchar(20) DEFAULT NULL, address varchar(20) DEFAULT NULL,command varchar(255) DEFAULT NULL,dealTime varchar(20) DEFAULT NULL,dealType int(1) DEFAULT NULL,result varchar(50) DEFAULT NULL, PRIMARY KEY (id)) ENGINE=InnoDB DEFAULT CHARSET=utf8";
            mychessdb.ExecuteCommand(sql);//如果表不存在，创建数据表  
        }

        public void Login(string address)
        {
            try
            {
                DateTime dealTime = DateTime.Now;
                int result;
                String sql = String.Format("INSERT INTO login(address, login) VALUES('{0}', '{1}')", address, dealTime);//   
                result = mychessdb.ExecuteCommand(sql);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void Logout(string address)
        {
            try
            {
                DateTime dealTime = DateTime.Now;
                int result;
                String sql = String.Format("UPDATE  login set logout = '{0}' where address = '{1}'", dealTime, address);
                result = mychessdb.ExecuteCommand(sql);                
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void Insert(string[] param)
        {
            if (DateString != DateTime.Now.ToString("yyyyMMdd"))
            {
                DateString = DateTime.Now.ToString("yyyyMMdd");
                string sql = "CREATE TABLE IF NOT EXISTS chess" + DateString + "(id int(11) NOT NULL AUTO_INCREMENT, revTime varchar(20) DEFAULT NULL, address varchar(20) DEFAULT NULL,command varchar(255) DEFAULT NULL,dealTime varchar(20) DEFAULT NULL,dealType int(1) DEFAULT NULL,result varchar(50) DEFAULT NULL, PRIMARY KEY (id)) ENGINE=InnoDB DEFAULT CHARSET=utf8";
                mychessdb.ExecuteCommand(sql);//如果表不存在，创建数据表  
            }

            try
            {
                string sql = String.Format("INSERT INTO chess" + DateString + "(revTime, address, command) VALUES('{0}', '{1}','{2}')", param[0], param[1], param[2]);//插入几条数据    
                mychessdb.ExecuteCommand(sql);                
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void InsertBoard(string board)
        {
            try
            {
                string sqlquery = String.Format("select * from board where `key`='{0}'", board);
                MySqlDataReader reader = mychessdb.GetReader(sqlquery);
                
                string sql;
                int visit = 1;
                if (reader.Read())
                {
                    int id = (int)reader["id"];
                    visit = (int)reader["visit"]+1;
                    sql = String.Format("update board  set `visit` = {0} where `id` = {1}", visit, id);  
                }
                else
                {
                    sql = String.Format("INSERT INTO board (`key`, `visit`) VALUES('{0}', {1})", board, visit);  
                }
                reader.Close();
                mychessdb.ExecuteCommand(sql);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public int getBoardID(string board)
        {
            try
            {
                string sqlquery = String.Format("select * from board where `key`='{0}'", board);
                MySqlDataReader reader = mychessdb.GetReader(sqlquery);

                int id = 0;
                if (reader.Read())
                {
                    id = (int)reader["id"];
                }
                reader.Close();
                return id;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void InsertQueryall(string board, string strQueryall)
        {
            try
            {
                string sqlquery = String.Format("select * from queryall where `key`='{0}'", board);
                MySqlDataReader reader = mychessdb.GetReader(sqlquery);

                string sql;
                int visit = 1;
                if (reader.Read())
                {
                    int id = (int)reader["id"];
                    visit = (int)reader["visit"] + 1;
                    sql = String.Format("update queryall  set `visit` = {0} where `id` = {1}", visit, id);
                }
                else
                {
                    sql = String.Format("insert into queryall (`key`, `value`, `visit`) VALUES('{0}', '{1}', {2})", board, strQueryall, visit);
                }
                reader.Close();
                mychessdb.ExecuteCommand(sql);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void setItemToDepthinfo(int board_id, string message)
        {
            try
            {
                depthInfo depthinfo = new depthInfo(message);
                string sqlquery = String.Format("select * from depthinfo where `board_id`='{0}' and `depth` = {1}", board_id, depthinfo.depth);
                MySqlDataReader reader = mychessdb.GetReader(sqlquery);

                string sql = "";
                if (reader.Read())
                {
                    reader.Close();
                    return;
                }
                else
                {
                    sql = String.Format("insert into depthinfo (`board_id`,`depth`, `seldepth`, `multipv`, `score`, `nodes`, `nps`, `hashfull`, `tbhits`, `time`, `pv`) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9},'{10}')",
                    board_id, depthinfo.depth, depthinfo.seldepth, depthinfo.multipv, depthinfo.score, depthinfo.nodes, depthinfo.nps, depthinfo.hashfull, depthinfo.tbhits, depthinfo.time, depthinfo.pv);
                }
                reader.Close();
                mychessdb.ExecuteCommand(sql);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void Update(int dealType, string result, string address, string command)
        {
            try
            {
                string dealTime = DateTime.Now.ToLongTimeString();
                String sql = String.Format("UPDATE  chess{0} set dealType = {1},result = '{2}' , dealTime = '{3}' where address = '{4}' and command = '{5}'", DateString, dealType, result, dealTime, address, command);
                mychessdb.ExecuteCommand(sql);    
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            
        }

        public object Query(string addr)
        {
            return null;
        }
    }
}
