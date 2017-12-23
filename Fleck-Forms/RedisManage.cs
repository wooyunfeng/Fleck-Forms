using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Fleck_Forms
{
    class RedisManage
    {
        public RedisHelper cloudredis;
        public RedisHelper engineredis_reader;
        public RedisHelper engineredis_writer;
        public RedisHelper countredis;

        public RedisManage()
        {
            engineredis_writer = new RedisHelper(Setting.engineRedis_writer, "jiao19890228");
            engineredis_reader = new RedisHelper(Setting.engineRedis_reader, "jiao19890228");
            cloudredis = new RedisHelper(Setting.cloudRedisPath, "jiao19890228");
            countredis = new RedisHelper(Setting.countRedis, "jiao19890228");
        }

        internal bool getItemFromList(string list, int index)
        {
            if (engineredis_reader.ContainsKey(list))
            {
                string value = engineredis_reader.getItemFromList(list, index - 1);
                if (value != null && value.Length > 0)
                {
                    return true;
                }
            }
            return false;
        }

        internal void setItemToList(string list, string message)
        {
            depthInfo depthinfo = new depthInfo(message);

            if (depthinfo.depth > 0 && depthinfo.pv != "")
            {
                int intDepth = depthinfo.depth;
                for (int i = engineredis_writer.getAllItems(list).Count; i < intDepth; i++)
                {
                    engineredis_writer.addItemToListRight(list, "");
                }

                engineredis_writer.setItemToList(list, intDepth - 1, message);
            }
        }

        internal string getbestmoveFromList(NewMsg msg)
        {
            List<string> list = engineredis_reader.getAllItems(msg.GetBoard());
            string strmsg = "";
            int nlevel = Int32.Parse(msg.GetDepth());
            if (list.Count >= nlevel)
            {
                //过滤空消息
                for (int i = 0; i < nlevel; i++)
                {
                    if (list[i].Length > 0)
                    {
                        strmsg = list[i];
                        msg.Send(strmsg);
                    }
                }
                if (strmsg.Length > 0)
                {
                    string[] infoArray = strmsg.Split(' ');
                    for (int j = 0; j < infoArray.Length; j++)
                    {
                        if (infoArray[j] == "pv")
                        {
                            string line = "bestmove " + infoArray[j + 1];                           
                            return msg.Send(line);
                        }
                    }
                }
            }
            return "";
        }

        public string QueryallFromCloud(string board)
        {
            string serverResult = "";
            try
            {
                serverResult = cloudredis.getValueString(board);
                if (serverResult == null)
                {
                    string serverUrl = "http://api.chessdb.cn:81/chessdb.php?action=queryall&board=" + board;
                    string postData = "";
                    serverResult = HttpPostConnectToServer(serverUrl, postData);

                    if (serverResult != null)
                    {
                        serverResult = serverResult.Replace("move:", "");//替换为空
                        serverResult = serverResult.Replace("score:", "");//替换为空
                        serverResult = serverResult.Replace("rank:", "");//替换为空
                        serverResult = serverResult.Replace("note:", "");//替换为空
                        serverResult = serverResult.Replace("\0", "");//替换为空                       
                        cloudredis.setValueString(board, serverResult);
                    }
                    else
                    {
                        serverResult = "";
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("[error] QueryallFromCloud " + ex.Message);
            }
            return serverResult;
        }

        public string HttpPostConnectToServer(string serverUrl, string postData)
        {
            var dataArray = Encoding.UTF8.GetBytes(postData);
            //创建请求  
            var request = (HttpWebRequest)HttpWebRequest.Create(serverUrl);
            request.Method = "POST";
            request.ContentLength = dataArray.Length;
            //设置上传服务的数据格式  
            request.ContentType = "application/x-www-form-urlencoded";
            //请求的身份验证信息为默认  
            request.Credentials = CredentialCache.DefaultCredentials;
            //请求超时时间  
            request.Timeout = 10000;
            //创建输入流  
            Stream dataStream;
            try
            {
                dataStream = request.GetRequestStream();
            }
            catch (Exception)
            {
                return null;//连接服务器失败  
            }
            //发送请求  
            dataStream.Write(dataArray, 0, dataArray.Length);
            dataStream.Close();
            //读取返回消息  
            string res = "";
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[error] HttpPostConnectToServer " + ex.Message);
            }
            return res;
        }

    }
}
