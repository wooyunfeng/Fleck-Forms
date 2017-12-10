using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Fleck_Forms
{
    class Setting
    {
        static public string title { get; set; }
        static public string websocketPort { get; set; }
        static public string level { get; set; }
        static public bool islog { get; set; }

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
                if (xe.GetAttribute("key").ToString() == "Title")
                {
                    Setting.title = xe.GetAttribute("value").ToString();
                }
                if (xe.GetAttribute("key").ToString() == "WebSocketPort")
                {
                    Setting.websocketPort = xe.GetAttribute("value").ToString();
                }
                if (xe.GetAttribute("key").ToString() == "Level")
                {
                    Setting.level = xe.GetAttribute("value").ToString();
                }
                 if (xe.GetAttribute("key").ToString() == "Log")
                {
                    Setting.islog = Convert.ToBoolean(xe.GetAttribute("value"));
                }               
            }
        }
    }    
}
