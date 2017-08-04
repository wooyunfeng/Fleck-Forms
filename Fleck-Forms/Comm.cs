using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Xml;
using System.Net;
using ServiceStack.Redis;
using RedisStudy;
using System.Timers;
using Newtonsoft.Json;
using System.Data.SQLite;
using Fleck;

namespace Fleck_Forms
{    
    class Comm : SQLiteHelper
    {
        public Log log;
        public Setting setting;
        public Queue DealSpeedQueue;
        public RedisHelper cloudredis;
        public RedisHelper engineredis;

        public User user;
        private static int nMsgQueuecount { get; set; }
        public bool bRedis { get; set; }
        public void WriteInfo(string message, bool isOutConsole = false)
        {
            if (log == null)
            {
                return;
            }
            log.WriteInfo(message);
            if (isOutConsole)
            {
                Console.WriteLine(message);
            }
        }
       
        public int getDealspeed()
        {
            return nMsgQueuecount;
        }

        public string getDepth()
        {
            return Setting.level;
        }

        public string getThinktimeout()
        {
            return Setting.thinktimeout.ToString();
        }

        public bool getSupportCloudApi()
        {
            return Setting.isSupportCloudApi;
        }

        public void LoadXml()
        {
            setting.LoadXml();
        }

      
        public void Init()
        {
            cpuCounter = new PerformanceCounter();

            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            user = new User();
            setting = new Setting();
            log = new Log();
            DealSpeedQueue = new Queue();
            engineredis = new RedisHelper(Setting.engineredispath,"jiao19890228");
            cloudredis = new RedisHelper(Setting.cloudredispath,"jiao19890228");
            SQLite_Init();
         }

        public string DealQueryallMessage(string board)
        {
            string str = "";
            if (board.Length > 0 && Setting.isSupportCloudApi)
            {
                str = QueryallFromCloud(board);
            }
            return str;
        }

        public bool getItemFromList(string list, int index)
        {
            if (!bRedis)
            {
                return false;
            }
            if (engineredis.ContainsKey(list))
            {
                string value = engineredis.getItemFromList(list, index);
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

        public void setItemToList(string list, string message)
        {
            if (!bRedis)
            {
                return;
            }
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
       
        public string getbestmoveFromList(NewMsg msg)
        {
            if (!bRedis)
            {
                return "";
            }
            List<string> list = engineredis.getAllItems(msg.GetCommand());
            string strmsg = "";
            int nlevel = Int32.Parse(Setting.level);
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
                            SQLite_UpdateCommand(0, line, msg.GetAddr(), msg.GetMessage());
                            return line;
                        }
                    }
                }
            }

            return "";
        }

        public void getbestmoveFromList(string message)
        {
            Console.Write("0");
            List<string> list = engineredis.getAllItems(message);
            string strmsg = "";
            int nlevel = Int32.Parse(Setting.level);
            if (list.Count >= nlevel)
            {
                //过滤空消息
                for (int i = 0; i < nlevel; i++)
                {
                    if (list[i].Length > 0)
                    {
                        strmsg = list[i];
                    }
                }
                if (strmsg.Length > 0)
                {
                    string[] infoArray = strmsg.Split(' ');
                    for (int j = 0; j < infoArray.Length; j++)
                    {
                        if (infoArray[j] == "pv")
                        {
                            return;
                        }
                    }
                }
            }
        }

//         public string QuerybestFromCloud(string board)
//         {
//             if (!Setting.isSupportCloudApi)
//             {
//                 return null;
//             }
//             string serverResult = "";
//             try
//             {
//                 serverResult = cloudredis.getValueString("Querybest:" + board);
//                 if (serverResult == null)
//                 {
//                     string serverUrl = "http://api.chessdb.cn:81/chessdb.php?action=querybest&board=" + board;
//                     string postData = "";
//                     serverResult = HttpPostConnectToServer(serverUrl, postData);
//                     if (serverResult != null)
//                     {
//                         cloudredis.setValueString("Querybest:" + board, serverResult);
//                     }
//                     else
//                     {
//                         serverResult = "";
//                     }
//                 }
//             }
//             catch (System.Exception ex)
//             {
//                 Console.WriteLine("[error] QuerybestFromCloud " + ex.Message);
//             }
//             return serverResult;
//         }

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
                        cloudredis.setValueString( board, serverResult);
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

        public Msg Json2Msg(string jsonStr)
        {  
            Msg msg = null;       
            try
            {               
                JavaScriptObject jsonObj = JavaScriptConvert.DeserializeObject<JavaScriptObject>(jsonStr);
                msg = new Msg(jsonObj["index"].ToString(), jsonObj["command"].ToString());               
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return msg;
        }

        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;

        public string getCurrentCpuUsage()
        {
            return (int)cpuCounter.NextValue() + "%";
        }

        public string getAvailableRAM()
        {
            return ramCounter.NextValue()/1024 + " GB";
        } 

        public void ReadFile(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                Console.WriteLine(line.ToString());
            }
        }     
    }

}
