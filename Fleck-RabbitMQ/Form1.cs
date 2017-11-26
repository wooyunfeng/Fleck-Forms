using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RabbitMQ.Client;
using Fleck;
using Fleck.online;
using Fleck_Forms;
using System.Threading;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Collections;

namespace Fleck_RabbitMQ
{
    public partial class Form1 : Form
    {
        ConnectionFactory factory;
        IConnection connection;
        IModel send_channel;
        Dictionary<String, Object> pList = new Dictionary<String, Object>();
        Comm comm;
        public Form1()
        {
            addListDelegate = new AddConnectionItem(AddListItemMethod);
            removeListDelegate = new RemoveConnectionListItem(RemoveListItemMethod);
            addMsgInDelegate = new AddMsgInItem(AddMsgItemInMethod);
            addMsgOutDelegate = new AddMsgOutItem(AddMsgItemOutMethod);
            InitializeComponent();            
        }

        public Form1(string port)
        {
            addListDelegate = new AddConnectionItem(AddListItemMethod);
            removeListDelegate = new RemoveConnectionListItem(RemoveListItemMethod);
            addMsgInDelegate = new AddMsgInItem(AddMsgItemInMethod);

            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            InitListView();
            comm = new Comm();
            comm.Init();
            countQueue = new Queue();
            MsgCount = 0;
            RunTime = System.DateTime.Now;
            m_CloudApi.Checked = Setting.isSupportCloudApi;
            m_port.Text = Setting.websocketPort;
            this.Text = Setting.title;
            FleckLog.Level = LogLevel.Info;
            OnWebSocketServer(Setting.websocketPort);           
            initRabbit();
            recvThread();
        }

        public void OnWebSocketServer(string port)
        {
            var server = new WebSocketServer("ws://0.0.0.0:" + port);
            RoomSet roomSet = new RoomSet();

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    AddConnection(socket);
                    var role = new Role(socket);
                    comm.user.Add(role);
                };
                socket.OnClose = () =>
                {
                    DelConnection(socket);
                    var role = comm.user.GetAt(socket);
                    if (role != null)
                    {
                        comm.user.Remove(socket);
                    }       
                    roomSet.Remove(socket);
                };
                socket.OnMessage = message =>
                {                    
                    if (message.IndexOf("roomid") != -1)
                    {
                        roomSet.Add(message, socket);
                    }
                    else if (message.IndexOf("move") != -1)
                    {
                        roomSet.Send(socket, message);
                    }
                    else if (message.IndexOf("resign") != -1)
                    {
                        roomSet.RemoveAll(socket);
                    }
                    OnMessage(socket, message);
                };
            });
        }

        public void OnMessage(IWebSocketConnection socket, string message)
        {
            sendtoRabbit(message);
            NewMsg msg = new NewMsg(socket, message);
            if (!pList.ContainsKey(msg.uuid))
            {
                pList.Add(msg.uuid, msg);
            }
           
            string strAddr = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort.ToString();
            string[] showmsg = { DateTime.Now.ToLongTimeString(), strAddr, message };
            AddMsgIn(showmsg);

            //过滤命令
            try
            {
                if (message.IndexOf("queryall") != -1)
                {
                    string strQueryall = comm.DealQueryallMessage(msg.GetBoard());
                    if (strQueryall != "" && strQueryall != "unknown")
                    {
                        string sendmsg = msg.Send(strQueryall);
                        string[] msgs = { strAddr, "redis", sendmsg };
                    }
                }
                else if (message.IndexOf("position") != -1)
                {
                    DealPositionMessage(socket, message);
                }
                else if (message.IndexOf("move") != -1)
                {
                    DealMoveMessage(socket, message);
                }
                else if (message.IndexOf("openbook") != -1)
                {
                    DealOpenBookMessage(socket, message);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DealMoveMessage(IWebSocketConnection socket, string message)
        {
//             var role_from = new Role(socket);
//             foreach (var role in comm.user.allRoles.ToList())
//             {
//                 if (socket != role.connection)
//                 {
//                     role.Send(message);
//                 }
//             }
        }

        private void DealOpenBookMessage(IWebSocketConnection socket, string message)
        {
//             NewMsg msg = new NewMsg(socket, message);
//             object obj = comm.sqlOperate.getOpenBook(msg.GetBoard());
//             string str = JavaScriptConvert.SerializeObject(obj);
//             msg.Send(str);
        }

        private void DealPositionMessage(IWebSocketConnection socket, string message)
        {
            NewMsg msg = new NewMsg(socket, message);

            if (msg.GetCommand() == null)
            {
                comm.WriteInfo(msg.GetAddr() + "  " + message);
                return;
            }

            //查库
            if (comm.bRedis)
            {
                if (comm.getItemFromList(msg))
                {
                    string sendmsg = comm.getbestmoveFromList(msg);
                    string[] msgs = { msg.GetAddr(), "reids", sendmsg };
                    return;
                }
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
            send_channel.QueueDeclare("send-queue", true, false, false, null);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = send_channel.CreateBasicProperties();
            send_channel.BasicPublish("", "send-queue", properties, body);
        }

        private void recvThread()
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
                if (pList.ContainsKey(uuid))
                {
                    NewMsg sendmsg = (NewMsg)pList[uuid];
                    sendmsg.Send(message);
                }                
            }
            string[] names = { DateTime.Now.ToLongTimeString(), message,""};
            AddMsgOut(names);
            System.Threading.Thread.Sleep(1);
        }

        private void AddMsgIn(string [] param)
        {
            try
            {
                MsgCount++;
                this.Invoke(this.addMsgInDelegate, new Object[] { param });
            }
            catch (System.Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void AddMsgOut(string[] param)
        {
            try
            {
                MsgCount++;
                this.Invoke(this.addMsgOutDelegate, new Object[] { param });
            }
            catch (System.Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            string m_speed = null;
            string m_online = comm.user.allRoles.Count.ToString();

            string m_msg = MsgCount.ToString() + " 个";

            if (countQueue != null)
            {
                countQueue.Enqueue(MsgCount);
                if (countQueue.Count > 60)
                {
                    countQueue.Dequeue();
                }
                m_speed = (MsgCount - (int)countQueue.Peek()).ToString() + " 个/分钟";
            }       

            string m_undo = " 个";

            string m_time = RunTime.ToString();

            DateTime currentTime = System.DateTime.Now;
            TimeSpan span = currentTime.Subtract(RunTime);
            string m_span = span.Days + "天" + span.Hours + "时" + span.Minutes + "分" + span.Seconds + "秒";

            string m_CPU = comm.getCurrentCpuUsage();
            string m_Memory = comm.getAvailableRAM();

            string[] names = { DateTime.Now.ToLongTimeString(),m_online, m_msg, m_speed, m_undo, m_CPU, m_Memory, m_time, m_span };
            AddListViewItem(listView4, names, 0);
 
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
            listView1.Columns.Add("用户", 132);
            listView1.Columns.Add("状态", 73);

            listView2.GridLines = true;
            //单选时,选择整行
            listView2.FullRowSelect = true;
            //显示方式
            listView2.View = View.Details;
            //没有足够的空间显示时,是否添加滚动条
            listView2.Scrollable = true;
            //是否可以选择多行
            listView2.MultiSelect = false;

            listView2.View = View.Details;
            listView2.Columns.Add("时间", 60);
            listView2.Columns.Add("用户", 132);
            listView2.Columns.Add("命令", 1100);

            listView4.GridLines = true;
            //单选时,选择整行
            listView4.FullRowSelect = true;
            //显示方式
            listView4.View = View.Details;
            //没有足够的空间显示时,是否添加滚动条
            listView4.Scrollable = true;
            //是否可以选择多行
            listView4.MultiSelect = false;

            listView4.View = View.Details;
            listView4.Columns.Add("  时间", 60, HorizontalAlignment.Center);
            listView4.Columns.Add("在线用户", 60, HorizontalAlignment.Center);
            listView4.Columns.Add("接受请求", 60, HorizontalAlignment.Center);
            listView4.Columns.Add("处理速度", 80, HorizontalAlignment.Center);
            listView4.Columns.Add("等待处理", 60, HorizontalAlignment.Center);
            listView4.Columns.Add("CPU使用", 60, HorizontalAlignment.Center);
            listView4.Columns.Add("剩余内存", 100, HorizontalAlignment.Center);
            listView4.Columns.Add("启动时间", 130, HorizontalAlignment.Center);
            listView4.Columns.Add("运行时间", 100, HorizontalAlignment.Center);

            listViewNF1.GridLines = true;
            //单选时,选择整行
            listViewNF1.FullRowSelect = true;
            //显示方式
            listViewNF1.View = View.Details;
            //没有足够的空间显示时,是否添加滚动条
            listViewNF1.Scrollable = true;
            //是否可以选择多行
            listViewNF1.MultiSelect = false;

            listViewNF1.View = View.Details;
            listViewNF1.Columns.Add("时间", 60);
            listViewNF1.Columns.Add("结果",1000);
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

        DateTime RunTime;
        int MsgCount;
        Queue countQueue;

        //安全调用控件
        public delegate void AddConnectionItem(IWebSocketConnection socket);
        public AddConnectionItem addListDelegate;

        public delegate void AddMsgInItem(string [] message);
        public AddMsgInItem addMsgInDelegate;

        public delegate void AddMsgOutItem(string[] message);
        public AddMsgOutItem addMsgOutDelegate;

        public delegate void RemoveConnectionListItem(IWebSocketConnection socket);
        public RemoveConnectionListItem removeListDelegate;

        public void AddMsgItemInMethod(string [] message)
        {
            AddListViewItem(listView2, message);
            System.Threading.Thread.Sleep(1);
        }

        public void AddMsgItemOutMethod(string[] message)
        {
            AddListViewItem(listViewNF1, message);
            System.Threading.Thread.Sleep(1);
        }

        public void AddListItemMethod(IWebSocketConnection socket)
        {
            string address = socket.ConnectionInfo.ClientIpAddress;
            string port = socket.ConnectionInfo.ClientPort.ToString();
            string str = address + ":" + port;

            string[] names = { DateTime.Now.ToLongTimeString(), str, "connected!" };
            AddListViewItem(listView1,names);

            add2Tree(address, port);

        }

        public void add2Tree(string address, string port)
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

        public void RemoveListItemMethod(IWebSocketConnection socket)
        {
            string address = socket.ConnectionInfo.ClientIpAddress;
            string port = socket.ConnectionInfo.ClientPort.ToString();
            string str = address + ":" + port;
            
            string[] names = { DateTime.Now.ToLongTimeString(), str, "closed!" };
            AddListViewItem(listView1,names);

            remove4Tree(address, port);            
        }

        public void remove4Tree(string address, string port)
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

        }

        public void DelConnection(IWebSocketConnection socket)
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
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView4.Items.Clear();
        }

        private void m_Redis_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void m_CloudApi_CheckedChanged(object sender, EventArgs e)
        {
            Setting.isSupportCloudApi = m_CloudApi.Checked;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
       
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void m_Mysql_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btn_reset_Click(object sender, EventArgs e)
        {
            
        }
    }    
}
