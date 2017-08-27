using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Serialization.Formatters;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using System.Net.Sockets;

namespace NetRemotingClient
{   
    public  partial class Client : Form
    {
        public delegate void AddMsgItem(string[] message);
        public AddMsgItem addMsgDelegate;
        static public string port { get; set; }
        static public string level { get; set; }
        static public bool isSupportCloudApi { get; set; }
        static public string engine { get; set; }
        static public string serveraddress { get; set; }
        static public string engineRedis_writer { get; set; }
        static public int serverport { get; set; }
        NewMsg currentMsg;
        DateTime starttime;
        RedisManage redis;
        bool bConnect;
        public Client()
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

            redis = new RedisManage(engineRedis_writer);
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
        private void Client_Load(object sender, EventArgs e)
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
                Thread thread = new Thread(OnTCP);
                thread.IsBackground = true;
                thread.Start(); 
            }
            catch (Exception ex)
            {
                strInfo = ex.Message; 
            }
        }

        private static byte[] result = new byte[4096];
        Socket serverSocket;
        bool bRun;
        string strInfo;
        bool bdealing = false;
        int dealcount = 0;
        DateTime startdeal;

        private void OnTCP()  
        {            
            try  
            {
                //设定服务器IP地址  
                IPAddress ip = IPAddress.Parse(serveraddress);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                bRun = true;
                socket.Connect(new IPEndPoint(ip, serverport)); //配置服务器IP与端口  
                strInfo = "连接服务器" + serveraddress + ":" + serverport + "成功! 本机地址" + socket.LocalEndPoint.ToString();
                serverSocket = socket;
                Thread.Sleep(100);
                bConnect = true;
               
            }
            catch (Exception ex)
            {
                bConnect = false;
                for (int i = 10; i > 0 && bRun; i--)
                {
                    strInfo = "连接服务器" + serveraddress + "失败，" + i + "秒后将重连！";
                    Thread.Sleep(1000);
                }
                if (bRun)
                {
                    OnTCP();
                }
               
                return;  
            }  
            //通过clientSocket接收数据  
            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start(serverSocket);  
        }

        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="clientSocket"></param>  
        private void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (bRun)
            {
                try
                {
                    //通过clientSocket接收数据  
                    int receiveNumber = myClientSocket.Receive(result);
                    string info = Encoding.ASCII.GetString(result, 0, receiveNumber);
                    if (info.IndexOf("exit") != -1)
                    {
                        strInfo = "服务器退出！";
                        bRun = false;
                        break;
                    }
                    else
                    {
                        currentMsg = new NewMsg(info);
                        deal(currentMsg);  
                    }                                     
                }
                catch (Exception ex)
                {
                    myClientSocket.Close();
                    strInfo = ex.Message;
                    break;
                }
            }
        }

        private void deal(NewMsg currentMsg)
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
        }  
  
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (bConnect)
                {
                    serverSocket.Send(Encoding.ASCII.GetBytes("exit"));
                    serverSocket.Close();
                }
                
                bRun = false;
                KillPipeThread();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // 检查一个Socket是否可连接  
        private bool IsSocketConnected(Socket client)
        {
            bool blockingState = client.Blocking;
            try
            {
                byte[] tmp = new byte[1];
                client.Blocking = false;
                client.Send(tmp, 0, 0);
                return false;
            }
            catch (SocketException e)
            {
                // 产生 10035 == WSAEWOULDBLOCK 错误，说明被阻止了，但是还是连接的  
                if (e.NativeErrorCode.Equals(10035))
                    return false;
                else
                    return true;
            }
            finally
            {
                client.Blocking = blockingState;    // 恢复状态  
            }
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
            resetEngine();
        }

        public void resetEngine()
        {
            SendtoServer("restart");
            KillPipeThread();
            //启动管道线程
            StartPipeThread();
            Thread.Sleep(1000);
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
               strInfo = ex.Message;
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
                                SendtoServer(line);
                            }

                            listinfo[intDepth-1] = line;
  
                        }

                        if (line.IndexOf("bestmove") != -1)
                        {
                            bdealing = false;
                            dealcount++;
                            SendtoServer(line);
                            string board = currentMsg.GetBoard();

                            for (int i = 0; i < nLevel; i++)
                            {
                                redis.setItemToList(board, listinfo[i]);
                            }                                

                            Array.Clear(listinfo, 0, listinfo.Length);

                            
                            SendtoServer("list");
                            linearray[0] = currentMsg.GetBoard();
                            linearray[1] = " depth " + intDepth.ToString() + " " + line;
                            AddMsg(linearray);
                        }
                        Thread.Sleep(10);
                    }
                }
            }
            catch (System.Exception ex)
            {
                strInfo = ex.Message;
            }
        }

        private void SendtoServer(string line)
        {
            try
            {
                if (bConnect)
                {
                    line += "#";
                    lock (serverSocket)
                    {
                        serverSocket.Send(Encoding.ASCII.GetBytes(line));
                    }
                }  
            }
            catch (System.Exception ex)
            {
                strInfo = ex.Message;
                bRun = false;
                Thread.Sleep(100);
                OnTCP();                
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

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (bRun &&  bConnect && !bdealing)
            {
                SendtoServer("list");
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            strInfo = "重连服务器";
            OnTCP();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            strInfo = "断开连接";
            serverSocket.Close();
            bRun = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            strInfo = "重启引擎";
            resetEngine();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }
    }
}
