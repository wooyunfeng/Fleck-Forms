using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fleck_Forms
{
    public interface IDataOperate
    {
        void Login(string p);
        void Logout(string p);
        void Insert(string[] param);
        void Update(int dealType, string result, string address, string command);
        void InsertBoard(string board);
        object Query(string addr);
        int getBoardID(string board);        
        void InsertQueryall(string board, string strQueryall);
        void setItemToDepthinfo(int id, string message);
    }
    //类封装，接口作为成员字段，实现接口统一访问
    //工厂模式
//     public class DataOperate
//     {
//         private IDataOperate iDataOperate;
// 
//         public DataOperate(string dataBaseName)
//         {
//             switch (dataBaseName)
//             {
//                 case "mysql":
//                     iDataOperate = new DoMySql();//实例化为接口类
//                     break;
//                 case "sqlite3":
//                     iDataOperate = new DoSqlite3();//实例化为接口类
//                     break;
//             }
// 
//         }
//         public string Insert() 
//         {
//             return iDataOperate.Insert(); 
//         }
//     }
// 
//     public class DoSqlite3 : IDataOperate
//     {
//         public SQLiteManage sqlite3;
// 
//         public DoSqlite3()
//         {
//             sqlite3 = new SQLiteManage();
//         }
//         public string Insert()
//         {
//             //实现ACCESS插入的代码;
//             return "access";
//         }
//     }
// 
//     public class DoMySql : IDataOperate
//     {
//         public MySqlManage mysql;
// 
//         public DoMySql()
//         {
//             mysql = new MySqlManage();
//         }
//         public string Insert()
//         {
//             //实现SQLServer插入的代码;
//             return "sql";
//         }
//     }
     
}
