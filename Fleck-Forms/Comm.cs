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
using Fleck_Forms;

namespace Fleck.aiplay
{    
    class Comm : SQLiteHelper
    {
        public Log log;
        public Setting setting;
        public Queue DealSpeedQueue;
        public RedisHelper redis;
        public User user;
        public string Port { get; set; }
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
            log = new Log(Port);
            DealSpeedQueue = new Queue();                
            redis = new RedisHelper();
            SQLite_Init();
         }

        public string DealQueryallMessage(string message)
        {
            string str = "";
            if (message.Length > 0 && Setting.isSupportCloudApi)
            {
                str = redis.QueryallFromCloud(message);
            }
            return str;
        }

        public void redisPushItemToList(string p, string line)
        {
            if (!bRedis)
            {
                return;
            }
            redis.PushItemToList(p, line);
        }


        public bool redisContainsKey(string p)
        {
            if (!bRedis)
            {
                return false;
            }
            return redis.ContainsKey(p);
        }

        public void DealQueryallMessage(IWebSocketConnection socket, string message)
        {
            if (!bRedis)
            {
                return;
            }
            string str = redis.QueryallFromCloud(message);
            if (str != null)
            {
                socket.Send(str);
            }
        }

        public string getFromList(NewMsg msg)
        {
            if (!bRedis)
            {
                return "";
            }
            List<string> list = redis.GetAllItemsFromList(msg.GetCommand());
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

        public void getFromList(string message)
        {
            Console.Write("0");
            List<string> list = redis.GetAllItemsFromList(message);
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
                            //Console.WriteLine("depth " + infoArray[2] + " bestmove " + infoArray[j + 1]);
                            return;
                        }
                    }
                }
            }
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
