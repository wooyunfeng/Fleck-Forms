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
    class Comm
    {
        public Log log;
        public Setting setting;
        public Queue DealSpeedQueue;
        public RedisManage redis;        
        public IDataOperate sqlOperate;        
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
            redis = new RedisManage();
            sqlOperate = new SQLiteManage();
        }

        public string DealQueryallMessage(string board)
        {
            string str = "";
            if (board.Length > 0 && Setting.isSupportCloudApi)
            {
                str = redis.QueryallFromCloud(board);
            }
            return str;
        }
        
        public bool getItemFromList(NewMsg msg)
        {
            string list =  msg.GetBoard();
            int index = Int32.Parse(msg.GetDepth());
            
            return getItemFromList(list,index);
        }

        public bool getItemFromList(string list, int index)
        {
            if (!bRedis)
            {
                return false;
            }
            try
            {
                return redis.getItemFromList(list, index);
               
            }
            catch (System.Exception ex)
            {
            	 return false;
            }            
        }

        public void setItemToList(string list, string message)
        {
            if (!bRedis)
            {
                return;
            }
            redis.setItemToList(list, message);            
        }
       
        public string getbestmoveFromList(NewMsg msg)
        {
            if (!bRedis)
            {
                return "";
            }
            string bestmove = redis.getbestmoveFromList(msg);
            sqlOperate.Update(0, bestmove, msg.GetAddr(), msg.GetMessage());
            return bestmove;
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
    }

}
