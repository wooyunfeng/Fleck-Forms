using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fleck_Forms
{
    class MySqlManage
    {
        private MySqlHelper mychessdb;
        public MySqlManage()
        {
            String connectionString = "server = rm-bp17csiteni8i84o3.mysql.rds.aliyuncs.com; database = mychessdb; user id = root; password = Jiao19890228;";
            mychessdb = new MySqlHelper(connectionString);
        }
    }
}
