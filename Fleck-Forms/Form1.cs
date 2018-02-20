using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Fleck;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;
using System.Collections.Concurrent;
using System.Collections;
using System.Data.SQLite;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace Fleck_Forms
{
    public partial class Form1 : Form
    {
        string Port = "9001";        
        IConnection connection;        
        Dictionary<String, Object> pUUIDList = new Dictionary<String, Object>();
        Dictionary<String, Object> pConnect = new Dictionary<String, Object>();
        Engine engine;
        DateTime StartRunTime;

        //安全调用控件
        public delegate void AddConnectionItem(IWebSocketConnection socket);
        public AddConnectionItem addListDelegate;

        public delegate void AddMsgItem(string[] message);
        public AddMsgItem addMsgDelegate;

        public delegate void AddLogItem(string[] message);
        public AddLogItem addLogDelegate;

        public delegate void RemoveConnectionListItem(IWebSocketConnection socket);
        public RemoveConnectionListItem removeListDelegate;

        public void AddMsgItemMethod(string[] message)
        {
            AddListViewItem(listMsgIn, message);
            System.Threading.Thread.Sleep(1);
        }

        public void AddLogMethod(string[] message)
        {
            AddListViewItem(listLog, message);
            System.Threading.Thread.Sleep(1);
        }

        public void AddListItemMethod(IWebSocketConnection socket)
        {
            string address = socket.ConnectionInfo.ClientIpAddress;
            string port = socket.ConnectionInfo.ClientPort.ToString();
            string str = address + ":" + port;

            string[] names = { DateTime.Now.ToLongTimeString(), str, "connected!" };
            AddListViewItem(listLogin,names);

            add2Tree(address, port);
        }

        public Form1()
        {
            addListDelegate = new AddConnectionItem(AddListItemMethod);
            removeListDelegate = new RemoveConnectionListItem(RemoveListItemMethod);
            addMsgDelegate = new AddMsgItem(AddMsgItemMethod);
            addLogDelegate = new AddLogItem(AddLogMethod);

            InitializeComponent();            
        }

        public Form1(string port)
        {
            Port = port;
            addListDelegate = new AddConnectionItem(AddListItemMethod);
            removeListDelegate = new RemoveConnectionListItem(RemoveListItemMethod);
            addMsgDelegate = new AddMsgItem(AddMsgItemMethod);
            addLogDelegate = new AddLogItem(AddLogMethod);

            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            InitListView();
            engine = new Engine();
            //启动引擎服务
            engine.Start();
            StartRunTime = System.DateTime.Now;
            m_port.Text = Setting.websocketPort;
            this.Text = Setting.title;
            FleckLog.Level = LogLevel.Info;
            OnWebSocketServer(Setting.websocketPort);
        }

        public void OnWebSocketServer(string port)
        {
            var server = new WebSocketServer("ws://0.0.0.0:" + port);

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    //list增加条目
                    AddConnection(socket);
                    engine.OnOpen(socket);
                    string strAddr = getSocketAddr(socket);
                    pConnect.Add(strAddr, socket);
                };
                socket.OnClose = () =>
                {
                    //list删除条目
                    DelConnection(socket);
                    engine.OnClose(socket);
                    string strAddr = getSocketAddr(socket);
                    pConnect.Remove(strAddr);
                    pUUIDList.Remove(strAddr);
                };
                socket.OnMessage = message =>
                {
                    NewMsg msg = new NewMsg(socket,message);
                    string strAddr = getSocketAddr(socket);
                    if (!pUUIDList.ContainsKey(strAddr))
                    {
                        pUUIDList.Add(strAddr,msg.uuid);
                    }
                                               
                    engine.OnMessage(msg);

                    string level = "17";
                    //0为微学堂，2为世界象棋
                    if (msg.GetType() == "0" || msg.GetType() == "2")
                    {
                        level = msg.GetDepth();
                    }
                    string[] showmsg = { DateTime.Now.ToLongTimeString(), msg.GetAddr(), msg.GetType(), msg.index, level, msg.GetBoard() };
                    if (msg.GetCommandType() == "position")
                    {
                        AddMsg(showmsg);
                    }                    
                };
            });
        }

        private string getSocketAddr(IWebSocketConnection socket)
        {
            return socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort.ToString();                    
        }
        #region rabbitmq
        ConnectionFactory factory;
        IModel send_channel;
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
            send_channel.QueueDeclare("send-queue", true, false, false, null);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = send_channel.CreateBasicProperties();
            send_channel.BasicPublish("", "send-queue", properties, body);
        }

        private void revfromRabbitThread()
        {
            Thread thread = new Thread(new ThreadStart(revfromRabbit));
            thread.Start();
        }

        private void revfromRabbit()
        {
            using (var recv_channel = connection.CreateModel())
            {
                recv_channel.QueueDeclare("recv-queue", true, false, false, null);
                recv_channel.BasicQos(0, 1, false);

                var consumer = new QueueingBasicConsumer(recv_channel);
                recv_channel.BasicConsume("recv-queue", false, consumer);

                while (true)
                {
                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    SendtoUser(message);
                    recv_channel.BasicAck(ea.DeliveryTag, false);
                }
            }
        }
        
        private void SendtoUser(string message)
        {
            if (JsonSplit.IsJson(message))//传入的json串
            {
                JavaScriptObject jsonObj = JavaScriptConvert.DeserializeObject<JavaScriptObject>(message);
                string uuid = jsonObj["uuid"].ToString();
                if (pUUIDList.ContainsKey(uuid))
                {
                    NewMsg sendmsg = (NewMsg)pUUIDList[uuid];
                    sendmsg.Send(message);
                }                
            }
        }
        #endregion
        private void AddMsg(string [] param)
        {
            try
            {
                this.Invoke(this.addMsgDelegate, new Object[] { param });
            }
            catch (System.Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (engine != null)
            {
                string m_online = engine.getUserCount().ToString();
                string m_undo = engine.getMsgQueueCount().ToString() + " 个";
                string m_time = StartRunTime.ToString();
                DateTime currentTime = System.DateTime.Now;
                TimeSpan span = currentTime.Subtract(StartRunTime);
                string m_span = span.Days + "天" + span.Hours + "时" + span.Minutes + "分" + span.Seconds + "秒";

                string m_CPU = engine.comm.getCurrentCpuUsage();
                string m_Memory = engine.comm.getAvailableRAM();

                string[] names = { DateTime.Now.ToLongTimeString(),m_online, m_undo, m_CPU, m_Memory, m_time, m_span };
                AddListViewItem(listMonitor, names, 0);

                //显示引擎信息
                int num = engine.getOutputContainerCount();
                for (int i = 0; i < num; i++)
                {
                    string[] msg = (string[])engine.OutputEngineQueueDequeue();
                    string[] info = new string[msg.Length + 1];
                    info[0] = DateTime.Now.ToLongTimeString();
                    msg.CopyTo(info, 1);
                    AddListViewItem(listEngineOut, info);
                }
           
                //显示引擎信息
                showEngineInfo();                     
            }
        }

        private void showEngineInfo()
        {
            listEngine.Items.Clear();
            if (engine.customerlist != null)
            {
                foreach (var customer in engine.customerlist.ToList())
                {
                    if (customer.check())
                    {
                        string[] str = { customer.getName(), customer.logintime.ToString(), customer.getlastdealtime(), customer.getDealCount().ToString(), customer.getCount().ToString() };
                        AddListViewItem(listEngine, str);
                    }
                    else
                    {
                        engine.customerlist.Remove(customer);
                    }

                }
            }           
        }

        private void InitListView()
        {
            listLogin.GridLines = true;
            //单选时,选择整行
            listLogin.FullRowSelect = true;
            //显示方式
            listLogin.View = View.Details;
            //没有足够的空间显示时,是否添加滚动条
            listLogin.Scrollable = true;
            //是否可以选择多行
            listLogin.MultiSelect = false;

            listLogin.View = View.Details;
            listLogin.Columns.Add("  时间", 60);
            listLogin.Columns.Add("用户", 140, HorizontalAlignment.Center);
            listLogin.Columns.Add("状态", 80, HorizontalAlignment.Center);

            listMsgIn.GridLines = true;
            //单选时,选择整行
            listMsgIn.FullRowSelect = true;
            //显示方式
            listMsgIn.View = View.Details;
            //没有足够的空间显示时,是否添加滚动条
            listMsgIn.Scrollable = true;
            //是否可以选择多行
            listMsgIn.MultiSelect = false;

            listMsgIn.View = View.Details;
            listMsgIn.Columns.Add("  时间", 60, HorizontalAlignment.Center);
            listMsgIn.Columns.Add("用户", 132, HorizontalAlignment.Center);
            listMsgIn.Columns.Add("mode", 38, HorizontalAlignment.Center);
            listMsgIn.Columns.Add("index", 45, HorizontalAlignment.Center);
            listMsgIn.Columns.Add("level", 45, HorizontalAlignment.Center);
            listMsgIn.Columns.Add("board", 360, HorizontalAlignment.Center);

            listEngineOut.GridLines = true;
            //单选时,选择整行
            listEngineOut.FullRowSelect = true;
            //显示方式
            listEngineOut.View = View.Details;
            //没有足够的空间显示时,是否添加滚动条
            listEngineOut.Scrollable = true;
            //是否可以选择多行
            listEngineOut.MultiSelect = false;

            listEngineOut.View = View.Details;
            listEngineOut.Columns.Add("  时间", 60);
            listEngineOut.Columns.Add("用户", 150, HorizontalAlignment.Center);
            listEngineOut.Columns.Add("引擎", 150, HorizontalAlignment.Center);
            listEngineOut.Columns.Add("index", 80, HorizontalAlignment.Center);
            listEngineOut.Columns.Add("result", 200, HorizontalAlignment.Center);


            listMonitor.GridLines = true;
            //单选时,选择整行
            listMonitor.FullRowSelect = true;
            //显示方式
            listMonitor.View = View.Details;
            //没有足够的空间显示时,是否添加滚动条
            listMonitor.Scrollable = true;
            //是否可以选择多行
            listMonitor.MultiSelect = false;

            listMonitor.View = View.Details;
            listMonitor.Columns.Add("  时间", 60, HorizontalAlignment.Center);
            listMonitor.Columns.Add("在线用户", 100, HorizontalAlignment.Center);
            listMonitor.Columns.Add("等待处理", 100, HorizontalAlignment.Center);
            listMonitor.Columns.Add("CPU", 80, HorizontalAlignment.Center);
            listMonitor.Columns.Add("剩余内存", 100, HorizontalAlignment.Center);
            listMonitor.Columns.Add("启动时间", 140, HorizontalAlignment.Center);
            listMonitor.Columns.Add("运行时间", 130, HorizontalAlignment.Center);


            listLog.GridLines = true;
            //单选时,选择整行
            listLog.FullRowSelect = true;
            //显示方式
            listLog.View = View.Details;
            //没有足够的空间显示时,是否添加滚动条
            listLog.Scrollable = true;
            //是否可以选择多行
            listLog.MultiSelect = false;

            listLog.View = View.Details;
            listLog.Columns.Add("  时间", 60, HorizontalAlignment.Center);
            listLog.Columns.Add("消息", 600, HorizontalAlignment.Center);


            listEngine.GridLines = true;
            //单选时,选择整行
            listEngine.FullRowSelect = true;
            //显示方式
            listEngine.View = View.Details;
            //没有足够的空间显示时,是否添加滚动条
            listEngine.Scrollable = true;
            //是否可以选择多行
            listEngine.MultiSelect = false;

            listEngine.View = View.Details;
            listEngine.Columns.Add("引擎地址", 200, HorizontalAlignment.Center);
            listEngine.Columns.Add("登录时间", 150, HorizontalAlignment.Center);
            listEngine.Columns.Add("上一次处理时间", 150, HorizontalAlignment.Center);
            listEngine.Columns.Add("处理数量", 100, HorizontalAlignment.Center);
            listEngine.Columns.Add("等待处理", 100, HorizontalAlignment.Center);
        }

        private void AddListViewItem(ListView listView, string[] array,int showLines = 35)
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

        private void add2Tree(string address, string port)
        {
            TreeNode tn;
            string str;
            bool isfind = false;
            int index = 0;
            for (index = 0; index < treeView1.Nodes.Count; index++)
            {
                str = treeView1.Nodes[index].Text;
                if (str.IndexOf(address) != -1)
                {
                    isfind = true;
                    break;
                }
            }

            if (isfind)
            {
                tn = treeView1.Nodes[index];
                tn.Name = tn.Text = address+"("+(tn.Nodes.Count+1)+")";
            }
            else
            {
                tn = new TreeNode();
                tn.Name = tn.Text = address;
                treeView1.Nodes.Add(tn);
            }

            tn.Nodes.Add(port);
            System.Threading.Thread.Sleep(1);
        }

        private void RemoveListItemMethod(IWebSocketConnection socket)
        {
            string address = socket.ConnectionInfo.ClientIpAddress;
            string port = socket.ConnectionInfo.ClientPort.ToString();
            string str = address + ":" + port;

            string[] names = { DateTime.Now.ToLongTimeString(), str, "closed!" };
            AddListViewItem(listLogin,names);

            remove4Tree(address, port);            
        }

        private void remove4Tree(string address, string port)
        {
            TreeNode tn;
            string str;
            try
            {
                for (int i = 0; i < treeView1.Nodes.Count; i++)
                {
                    str = treeView1.Nodes[i].Text;
                    if (str.IndexOf(address) != -1)
                    {
                        tn = treeView1.Nodes[i];

                        if (tn.Nodes.Count > 1)
                        {
                            for (int j = 0; j < tn.Nodes.Count; j++)
                            {
                                str = tn.Nodes[j].Text;
                                if (str.IndexOf(port) != -1)
                                {
                                    tn.Nodes.Remove(tn.Nodes[j]);
                                    break;
                                }
                            }

                            if (tn.Nodes.Count > 1)
                            {
                                tn.Name = tn.Text = address + "(" + tn.Nodes.Count + ")";
                            }
                            else
                            {
                                tn.Name = tn.Text = address;
                            }

                        }
                        else
                        {
                            treeView1.Nodes.Remove(tn);
                        }

                        break;
                    }
                }

                System.Threading.Thread.Sleep(1);
            }
            catch (System.Exception ex)
            {
                ShowError(ex.Message);
            } 
            
        }

        private void ShowError(string p)
        {
            if (engine != null)
            {
                string[] str = {"", "", p};
                engine.OutputEngineQueueEnqueue(str);
            }
        }

        private void DelConnection(IWebSocketConnection socket)
        {
            try
            {
                this.Invoke(this.removeListDelegate, new Object[] { socket });
            }
            catch (System.Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void AddConnection(IWebSocketConnection socket)
        {
            try
            {
                this.Invoke(this.addListDelegate, new Object[] { socket });
            }
            catch (System.Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void btn_expend_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void btn_closeall_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            listLogin.Items.Clear();
            listMsgIn.Items.Clear();
            listEngineOut.Items.Clear();
            listMonitor.Items.Clear();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            engine.Close();
            System.Environment.Exit(0);
        }

        private void btn_reset_Click(object sender, EventArgs e)
        {
            if (engine.customerlist != null)
            {
                foreach (var customer in engine.customerlist.ToList())
                {
                    customer.reset();
                    engine.customerlist.Remove(customer);
                }
            }
        }
    }

}
