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
        public RedisHelper engineredis;

        public RedisManage()
        {
            engineredis = new RedisHelper(Setting.engineRedisPath, "jiao19890228");
            cloudredis = new RedisHelper(Setting.cloudRedisPath, "jiao19890228");
        }

        internal bool getItemFromList(string list, int index)
        {
            if (engineredis.ContainsKey(list))
            {
                List<string> listarray = engineredis.getAllItems(list);
                string value = engineredis.getItemFromList(list, index - 1);
                if (value != null && value.Length > 0)
                {
                    return true;
                }
            }
            else
            {
                //初始化list
                for (int i = 0; i < 20; i++)
                {
                    engineredis.addItemToListRight(list, "");
                }
            }
            return false;
        }

        internal void setItemToList(string list, string message)
        {
            string[] sArray = message.Split(' ');
            /* 消息过滤
             * info depth 14 seldepth 35 multipv 1 score 19 nodes 243960507 nps 6738309 hashfull 974 tbhits 0 time 36205 
             * pv h2e2 h9g7 h0g2 i9h9 i0h0 b9c7 h0h4 h7i7 h4h9 g7h9 c3c4 b7a7 b2c2 c9e7 c2c6 a9b9 b0c2 g6g5 a0a1 h9g7 
             */
            if (sArray.Length > 3 && sArray[1] == "depth" && sArray[3] == "seldepth")
            {
                int intDepth = Int32.Parse(sArray[2]);
                for (int i = engineredis.getAllItems(list).Count; i < intDepth; i++)
                {
                    engineredis.addItemToListRight(list, "");
                }

                engineredis.setItemToList(list, intDepth - 1, message);
            }
        }

        internal string getbestmoveFromList(NewMsg msg)
        {
            List<string> list = engineredis.getAllItems(msg.GetBoard());
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
                        serverResult = "null";
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
