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
using System.Timers;
using Newtonsoft.Json;
using Fleck;

namespace Fleck_Forms
{    
    class Comm
    {
        public Log log;
        public Setting setting;
        public User user;

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
        
        public string getDepth()
        {
            return Setting.level;
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
            if (Setting.islog)
            {
                log = new Log();
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
    }

}
