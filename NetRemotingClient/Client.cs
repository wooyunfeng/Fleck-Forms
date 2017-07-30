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
        public delegate void AddMsgItem(string message);
        public AddMsgItem addMsgDelegate;
        static public string port { get; set; }
        static public string level { get; set; }
        static public bool isSupportCloudApi { get; set; }
        static public string engine { get; set; }
        NewMsg currentMsg;

        public Client()
        {
            LoadXml();          
            addMsgDelegate = new AddMsgItem(AddMsgItemMethod);
            InitializeComponent();
            InitListView();
        }

         public void AddMsgItemMethod(string  message)
        {
            string[] names = { DateTime.Now.ToLongTimeString(), message, "" };
            AddListViewItem(listView1, names);
            System.Threading.Thread.Sleep(1);
        }


         private void AddListViewItem(ListView listView, string[] array, int showLines = 28)
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
            listView1.Columns.Add("结果", 500);
            listView1.Columns.Add("状态", 73);
        }

         private void AddMsg(string  param)
         {
             try
             {
                 this.Invoke(this.addMsgDelegate, new Object[] { param });
             }
             catch (System.Exception ex)
             {
                 MessageBox.Show(ex.Message);
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
               
                StartPipeThread();
                Thread.Sleep(1000);
                OnTCP();
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message); 
            }
        }

        private static byte[] result = new byte[1024];
        Socket serverSocket;
        private void OnTCP()  
        {  
            //设定服务器IP地址  
            IPAddress ip = IPAddress.Parse("118.190.46.210");  
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
            try  
            {  
                serverSocket.Connect(new IPEndPoint(ip, 8885)); //配置服务器IP与端口  
                Console.WriteLine("连接服务器成功");  
            }  
            catch  
            {  
                Console.WriteLine("连接服务器失败，请按回车键退出！");  
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
            while (serverSocket != null)
            {
                try
                {
                    //通过clientSocket接收数据  
                    int receiveNumber = myClientSocket.Receive(result);
                    string info = Encoding.ASCII.GetString(result, 0, receiveNumber);
                    currentMsg = new NewMsg(info);
                    deal(currentMsg);
                   
                }
                catch (Exception ex)
                {
   //                 myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }

        private void deal(NewMsg currentMsg)
        {
            if (currentMsg.GetCommand().IndexOf("position") != -1)
            {
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
                serverSocket.Send(Encoding.ASCII.GetBytes("exit"));   
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();
                serverSocket = null;
            }
            catch
            { }
        }

        Process pProcess;
        DateTime EngineRunTime;
        private static StreamWriter PipeWriter { get; set; }

        public void StartPipeThread()
        {
            Thread pipeThread = new Thread(new ThreadStart(PipeThread));
            pipeThread.IsBackground = true;
            pipeThread.Start();
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
            OutputEngineQueueEnqueue("restart");
            deal(currentMsg);
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
                PipeWriter = null;
            }
            catch (System.Exception ex)
            {
               // MessageBox.Show(ex.Message);
            }
            Thread.Sleep(100);
        }

        public void PipeThread()
        {
            EngineRunTime = System.DateTime.Now;
            int intDepth = 0;
            string EnginePath = engine;

            string line = "";
            try
            {
                //管道参数初始化
                PipeInit(EnginePath, "");
                //截取输出流
                StreamReader reader = pProcess.StandardOutput;
                //截取输入流
                PipeWriter = pProcess.StandardInput;
                //每次读取一行
                line = reader.ReadLine();
                AddMsg(line);
                line = reader.ReadLine();
                AddMsg(line);

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
                            OutputEngineQueueEnqueue(line);
                        }

                        if (line.IndexOf("bestmove") != -1)
                        {
                            OutputEngineQueueEnqueue(line);
                            AddMsg(" depth " + intDepth.ToString() + " " + line);

                        }
                        Thread.Sleep(10);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OutputEngineQueueEnqueue(string line)
        {
            serverSocket.Send(Encoding.ASCII.GetBytes(line));        
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
            }
        }

    }
}
