using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Fleck_Forms
{
    class Setting
    {
        static public string websocketPort { get; set; }
        static public string level { get; set; }
        static public bool isSupportCloudApi { get; set; }
        static public string enginePath { get; set; }
        static public string tcpServerAddress { get; set; }
        static public int tcpServeraPort { get; set; }
        static public int thinktimeout { get; set; }
        static public string cloudRedisPath { get; set; }
        static public string engineRedisPath { get; set; }

        public Setting()
        {
            LoadXml();
        }

        public void LoadXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(".\\config.xml");
            XmlNode xn = doc.SelectSingleNode("configuration");
            XmlNodeList xnl = xn.ChildNodes;
            foreach (XmlNode xn1 in xnl)
            {
                XmlElement xe = (XmlElement)xn1;
                if (xe.GetAttribute("key").ToString() == "WebSocketPort")
                {
                    Setting.websocketPort = xe.GetAttribute("value").ToString();
                }
                if (xe.GetAttribute("key").ToString() == "Level")
                {
                    Setting.level = xe.GetAttribute("value").ToString();
                }
                if (xe.GetAttribute("key").ToString() == "TimeOut")
                {
                    Setting.thinktimeout = int.Parse(xe.GetAttribute("value").ToString());
                }
                if (xe.GetAttribute("key").ToString() == "CloudAPI")
                {
                    Setting.isSupportCloudApi = Convert.ToBoolean(xe.GetAttribute("value"));
                }
                if (xe.GetAttribute("key").ToString() == "EnginePath")
                {
                    Setting.enginePath = xe.GetAttribute("value").ToString();
                }
                if (xe.GetAttribute("key").ToString() == "ServerAddress")
                {
                    Setting.tcpServerAddress = xe.GetAttribute("value").ToString();
                }
                if (xe.GetAttribute("key").ToString() == "ServerPort")
                {
                    Setting.tcpServeraPort = Int32.Parse(xe.GetAttribute("value").ToString());
                }
                if (xe.GetAttribute("key").ToString() == "CloudRedisPath")
                {
                    Setting.cloudRedisPath = xe.GetAttribute("value").ToString();
                }
                if (xe.GetAttribute("key").ToString() == "EngineRedisPath")
                {
                    Setting.engineRedisPath = xe.GetAttribute("value").ToString();
                }
            }
        }
    }    
}
