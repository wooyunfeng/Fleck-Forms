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


namespace RabbitMQ_Forms
{    
    class Comm
    {
        public Log log;
        public Setting setting;

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
            setting = new Setting();
            if (Setting.islog)
            {
                log = new Log();
            }
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
