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
        object getOpenBook(string p);
    }     
}
