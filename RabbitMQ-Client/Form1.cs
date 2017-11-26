using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using NetRemotingClient;
using System.Collections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.IO;
using Fleck_Forms;
using System.Xml;

namespace RabbitMQ_Client
{
    public partial class Form1 : Form
    {
       public delegate void AddMsgItem(string[] message);
        public AddMsgItem addMsgDelegate;
        static public string port { get; set; }
        static public string level { get; set; }
        static public bool isSupportCloudApi { get; set; }
        static public string engine { get; set; }
        static public string serveraddress { get; set; }
        static public bool isEngineRedis { get; set; }
        static public string engineRedis_writer { get; set; }
        static public int serverport { get; set; }
        NewMsg currentMsg;
        DateTime starttime;
        RedisManage redis;
        Queue countQueue;
        bool bConnect;
        ConnectionFactory factory;
        IConnection connection;
        IModel send_channel;
        IModel recv_channel;
        BasicDeliverEventArgs ea;
        public Form1()
        {
            LoadXml();          
            addMsgDelegate = new AddMsgItem(AddMsgItemMethod);
            InitializeComponent();
            InitListView();
            starttime = DateTime.Now;
            cpuCounter = new PerformanceCounter();

            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            countQueue = new Queue();
            if (isEngineRedis)
            {
                redis = new RedisManage(engineRedis_writer);
            }
        }

         public void AddMsgItemMethod(string[] message)
        {
            string[] info = new string[message.Length + 1];
            info[0] = DateTime.Now.ToLongTimeString();
            message.CopyTo(info, 1);

            AddListViewItem(listView1, info);
            Thread.Sleep(1);
        }

         private void AddListViewItem(ListView listView, string[] array, int showLines = 20)
         {
             if (listView.Items.Count > showLines)
             {
                 listView.Items.Clear();
             }

             listView.BeginUpdate();
             ListViewItem lvItem;
             ListViewItem.ListViewSubItem lvSubItem;
             lvItem = new ListViewItem();
             lvItem.Text = array[0];
             listView.Items.Add(lvItem);

             for (int x = 1; x < array.Length; x++)
             {
                 lvSubItem = new ListViewItem.ListViewSubItem();
                 lvSubItem.Text = array[x];
                 lvItem.SubItems.Add(lvSubItem);
             }
             listView.EndUpdate();
         }

        private void InitListView()
        {
            listView1.GridLines = true;
            //单选时,选择整行
            listView1.FullRowSelect = true;
            //显示方式
            listView1.View = View.Details;
            //没有足够的空间显示时,是否添加滚动条
            listView1.Scrollable = true;
            //是否可以选择多行
            listView1.MultiSelect = false;

            listView1.View = View.Details;
            listView1.Columns.Add("时间", 60);
            listView1.Columns.Add("输入", 500);
            listView1.Columns.Add("输出", 250);
        }

         private void AddMsg(string[]  param)
         {
             try
             {
                 this.Invoke(this.addMsgDelegate, new Object[] { param });
             }
             catch (System.Exception ex)
             {
                 strInfo = ex.Message;
             }
         }
        /// <summary>
        /// 注册通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                textDepth.Text = level;
                labelstarttime.Text = starttime.ToString();
                DateTime currentTime = System.DateTime.Now;
                TimeSpan span = currentTime.Subtract(starttime);
                labelruntime.Text = span.Days + "天" + span.Hours + "时" + span.Minutes + "分" + span.Seconds + "秒";
                labelCPU.Text = getCurrentCpuUsage();
                labelMemory.Text = getAvailableRAM();
                strInfo = labelinfo.Text;
                labelcount.Text = dealcount.ToString();
                StartPipeThread();
                initRabbit();
                Thread.Sleep(1000);
                recvThread();
             
            }
            catch (Exception ex)
            {
                strInfo = "Form1_Load:" + ex.Message; 
            }
        }        

        bool bRun;
        string strInfo;
        bool bdealing = false;
        int dealcount = 0;
        DateTime startdeal;        

        private void deal(NewMsg currentMsg)
        {
            try
            {
                if (currentMsg.GetCommand().IndexOf("position") != -1)
                {
                    bdealing = true;
                    startdeal = DateTime.Now;
                    PipeWriter.Write(currentMsg.GetCommand() + "\r\n");
                    string depth = currentMsg.GetDepth();
                    if (depth == null)
                    {
                        PipeWriter.Write("go depth " + level + "\r\n");
                    }
                    else
                    {
                        PipeWriter.Write("go depth " + depth + "\r\n");
                    }
                }
                else
                {
                    recv_channel.BasicAck(ea.DeliveryTag, false);
                }
            }
            catch (System.Exception ex)
            {
                strInfo = "deal:" + ex.Message;
            }
           
        }  
  
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }
       
        Process pProcess;
        DateTime EngineRunTime;
        private static StreamWriter PipeWriter { get; set; }
        Thread pipeThread;
        StreamReader reader;
        bool bPipeRun;
        public void StartPipeThread()
        {
            pipeThread = new Thread(new ThreadStart(PipeThread));
            pipeThread.IsBackground = true;
            pipeThread.Start();
            bPipeRun = true;
        }

        private void PipeInit(string strFile, string arg)
        {
            pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo = new System.Diagnostics.ProcessStartInfo();
            pProcess.StartInfo.FileName = strFile;
            pProcess.StartInfo.Arguments = arg;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardInput = true;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.EnableRaisingEvents = true;
            pProcess.Exited += new EventHandler(exep_Exited);
            pProcess.Start();
        }

        //exep_Exited事件处理代码，这里外部程序退出后激活，可以执行你要的操作
        void exep_Exited(object sender, EventArgs e)
        {
            Restart();
        }

        private void KillPipeThread()
        {
            try
            {
                if (pProcess != null)
                {
                    pProcess.Kill();
                    pProcess.Close();
                }
                PipeWriter.Close();
                reader.Close();
                bPipeRun = false;
            }
            catch (System.Exception ex)
            {
                strInfo = "KillPipeThread:" + ex.Message;
            }
        }

        public void PipeThread()
        {
            EngineRunTime = System.DateTime.Now;
            int intDepth = 0;
            string EnginePath = engine;
            
            string line = "";
            string[] linearray = new string[3];

            int nLevel = Int32.Parse(level);
            string[] listinfo = new string[32];
            int ncout = 1;
            try
            {
                //管道参数初始化
                PipeInit(EnginePath, "");
                //截取输出流
                reader = pProcess.StandardOutput;
                //截取输入流
                PipeWriter = pProcess.StandardInput;
                //每次读取一行
                line = reader.ReadLine();
                linearray[0] = line;
                AddMsg(linearray);
                line = reader.ReadLine();
                linearray[0] = line;
                AddMsg(linearray);

                while (true)
                {
                    line = reader.ReadLine();
                    
                    if (line != null)
                    {
                        string[] sArray = line.Split(' ');
                        /* 消息过滤
                         * info depth 14 seldepth 35 multipv 1 score 19 nodes 243960507 nps 6738309 hashfull 974 tbhits 0 time 36205 
                         * pv h2e2 h9g7 h0g2 i9h9 i0h0 b9c7 h0h4 h7i7 h4h9 g7h9 c3c4 b7a7 b2c2 c9e7 c2c6 a9b9 b0c2 g6g5 a0a1 h9g7 
                         */
                        if (sArray.Length > 3 && sArray[1] == "depth" && sArray[3] == "seldepth")
                        {
                            intDepth = Int32.Parse(sArray[2]);
                            if (bdealing)
                            {
                                currentMsg.result = line;
                                sendtoRabbit(currentMsg.GetJson());
                            }
                            if (intDepth > 0 && intDepth < 32)
                            {
                                listinfo[intDepth - 1] = line;  
                            }                            
                        }

                        if (line.IndexOf("bestmove") != -1)
                        {
                            bdealing = false;
                            dealcount++;
                             string board = currentMsg.GetBoard();
                            Zobrist zobrist = new Zobrist();
                            UInt64 boardKey = zobrist.getKey(board);
                            for (int i = 0; i < nLevel && isEngineRedis; i++)
                            {
                                redis.setItemToList(boardKey.ToString("X8"), listinfo[i]);
                            }                                

                            Array.Clear(listinfo, 0, listinfo.Length);
                            linearray[0] = currentMsg.GetBoard();
                            linearray[1] = " depth " + intDepth.ToString() + " " + line;
                            AddMsg(linearray);
                            currentMsg.result = line;
                            string sendmsg = currentMsg.GetJson();
                            sendtoRabbit(sendmsg);
                            recv_channel.BasicAck(ea.DeliveryTag, false);
                        }
                        Thread.Sleep(10);
                    }
                    else
                    {
                        recv_channel.BasicAck(ea.DeliveryTag, false);
                        Restart();
                    }
                }
            }
            catch (System.Exception ex)
            {
                bdealing = false;
                Restart();
                strInfo = "PipeThread:" + ex.Message;
            }
        }
        
        private void initRabbit()
        {
            factory = new ConnectionFactory();
            factory.HostName = "47.96.28.91";
            factory.UserName = "chd1219";
            factory.Password = "jiao19890228";
            connection = factory.CreateConnection();
            send_channel = connection.CreateModel();

        }

        private void sendtoRabbit(string message)
        {
            send_channel.QueueDeclare("recv-queue", true, false, false, null);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = send_channel.CreateBasicProperties();
            send_channel.BasicPublish("", "recv-queue", properties, body);
        }

        private void recvThread()
        {
            Thread thread = new Thread(new ThreadStart(revfromRabbit));
            thread.Start();
        }

        private void revfromRabbit()
        {
            using (recv_channel = connection.CreateModel())
            {
                recv_channel.QueueDeclare("send-queue", true, false, false, null);
                recv_channel.BasicQos(0, 1, false);

                var consumer = new QueueingBasicConsumer(recv_channel);
                recv_channel.BasicConsume("send-queue", false, consumer);

                while (true)
                {
                    ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    if (message.IndexOf("id") != -1)
                    {
                        currentMsg = new NewMsg(message);
                        deal(currentMsg);       
                    }
                    else
                    {
                        recv_channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }
        }

        private void textDepth_TextChanged(object sender, EventArgs e)
        {
            level = textDepth.Text;
        }

        //查找进程、结束进程
        public void checkwerfault()
        {
            Process[] pro = Process.GetProcesses();//获取已开启的所有进程
            //遍历所有查找到的进程
            for (int i = 0; i < pro.Length; i++)
            {
                //判断此进程是否是要查找的进程
                if (pro[i].ProcessName.ToString().ToLower() == "werfault")
                {
                    pro[i].Kill();//结束进程
                }
            }
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
                if (xe.GetAttribute("key").ToString() == "Port")
                {
                    port = xe.GetAttribute("value").ToString();
                }
                if (xe.GetAttribute("key").ToString() == "Level")
                {
                    level = xe.GetAttribute("value").ToString();
                }
                if (xe.GetAttribute("key").ToString() == "CloudAPI")
                {
                    isSupportCloudApi = Convert.ToBoolean(xe.GetAttribute("value"));
                }
                if (xe.GetAttribute("key").ToString() == "EnginePath")
                {
                    engine = xe.GetAttribute("value").ToString();
                }
                if (xe.GetAttribute("key").ToString() == "ServerAddress")
                {
                    serveraddress = xe.GetAttribute("value").ToString();
                } 
                if (xe.GetAttribute("key").ToString() == "ServerPort")
                {
                    serverport = Int32.Parse(xe.GetAttribute("value").ToString());
                }
                if (xe.GetAttribute("key").ToString() == "EngineRedisWriter")
                {
                    engineRedis_writer = xe.GetAttribute("value").ToString();
                }
                if (xe.GetAttribute("key").ToString() == "EngineRedis")
                {
                    isEngineRedis = Convert.ToBoolean(xe.GetAttribute("value"));
                }
            }
        }

        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;

        public string getCurrentCpuUsage()
        {
            int cpu = (int)cpuCounter.NextValue();
            checkCPU(cpu);         
            return cpu + "%";
        }

        public string getAvailableRAM()
        {
            return ramCounter.NextValue() / 1024 + " GB";
        } 

        private void timer1_Tick(object sender, EventArgs e)
        {
            //监视werfault.exe
            checkwerfault();

            labelstarttime.Text = starttime.ToString();

            DateTime currentTime = System.DateTime.Now;
            TimeSpan span = currentTime.Subtract(starttime);
            labelruntime.Text = span.Days + "天" + span.Hours + "时" + span.Minutes + "分" + span.Seconds + "秒";

            labelCPU.Text = getCurrentCpuUsage();
            labelMemory.Text = getAvailableRAM();
            labelinfo.Text = strInfo;

            labelcount.Text = dealcount.ToString();
            //监视超时
            //checkTimeOut();
        }

        private void checkCPU(int cpu)
        {
            countQueue.Enqueue(cpu);
            int nCount = 0;
            if (countQueue.Count > 29)
            {
                countQueue.Dequeue();
                foreach (int ncpu in countQueue)
                {
                    nCount += ncpu;
                } 
                //一分钟内CPU平均值大于90，将level降为0，否则读取配置
                if (nCount/30 > 90)
                {
                    Restart();
                }              
            }
        }

        private void Restart()
        {
            Application.ExitThread();
            Application.Exit();
            Application.Restart();
            Process.GetCurrentProcess().Kill();
        }

        private bool checkTimeOut()
        {
            if (bdealing)
            {
                DateTime currentTime = System.DateTime.Now;
                TimeSpan span = currentTime.Subtract(startdeal);
                if (span.Seconds > 30)
                {
                    bdealing = false;
                    return true;
                }
            }
            return false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Restart();
        }
    }
}
