using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace NetRemotingClient
{
    class RedisManage
    {
        public RedisHelper engineredis_writer;

        public RedisManage(string engineRedis_writer)
        {
            engineredis_writer = new RedisHelper(engineRedis_writer, "jiao19890228");
        }

        internal void setItemToList(string key, string message)
        {      
            try
            {
                depthInfo depthinfo = new depthInfo(message);
                if (depthinfo.depth > 0 && depthinfo.pv != "")
                {
                    int intDepth = depthinfo.depth;
                    for (int i = engineredis_writer.getAllItems(key).Count; i < intDepth; i++)
                    {
                        engineredis_writer.addItemToListRight(key, "");
                    }

                    engineredis_writer.setItemToList(key, intDepth - 1, message);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        internal void setItemToList(string key, string[] listinfo)
        {
            try
            {
                string str = "";
                if (!engineredis_writer.ContainsKey(key))
                {
                    engineredis_writer.Remove(key);
                }

                foreach(var message in listinfo) 
                {
                    if (message != null)
                    {
                       // engineredis_writer.addItemToListRight(list, message);
                        depthInfo depthinfo = new depthInfo(message);

                        if (depthinfo.depth > 0 && depthinfo.pv != "")
                        {
                            str += depthinfo.depth.ToString() + ',' + depthinfo.score.ToString() + "," + depthinfo.pv + "|";
                        }
                    }                   
                }
                str = str.Substring(0, str.Length - 1);
                engineredis_writer.addItemToListRight(key, str);               
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal void setRangeToList(string board, List<string> listinfo)
        {
            if (!engineredis_writer.ContainsKey(board))
            {
                engineredis_writer.setRangeToList(board, listinfo);
            }
        }
    }
}
