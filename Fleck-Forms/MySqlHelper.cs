using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Fleck_Forms
{
    /// <summary>
    /// MySqlHelper操作类
    /// </summary>
    public sealed partial class MySqlHelper
    {
        private MySqlConnection Connection;

        public MySqlHelper(String connectionString)
        {
            Connection = new MySqlConnection(connectionString);
            Connection.Open();    
        }

        public int ExecuteCommand(String sql)
        {
            MySqlCommand cmd = new MySqlCommand(sql, Connection);
            Console.WriteLine(sql);
            int result = cmd.ExecuteNonQuery();

            return result;
        }

        public int ExecuteCommand(String sql, MySqlParameter[] values)
        {
            MySqlCommand cmd = new MySqlCommand(sql, Connection);

            cmd.Parameters.AddRange(values);

            return cmd.ExecuteNonQuery();
        }

        public MySqlDataReader GetReader(String sql)
        {
            MySqlCommand cmd = new MySqlCommand(sql, Connection);

            MySqlDataReader reader = cmd.ExecuteReader();

            return reader;
        }


        public MySqlDataReader GetReader(String sql, MySqlParameter[] values)
        {
            MySqlCommand cmd = new MySqlCommand(sql, Connection);

            cmd.Parameters.AddRange(values);

            MySqlDataReader reader = cmd.ExecuteReader();

            return reader;
        }
    }
}