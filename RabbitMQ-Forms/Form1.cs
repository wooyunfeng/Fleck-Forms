using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RabbitMQ.Client;
using System.Threading;
using RabbitMQ.Client.Events;
using System.Collections;
using Newtonsoft.Json;
using System.IO;

namespace RabbitMQ_Forms
{
    public partial class Form1 : Form
    {

        ConnectionFactory factory;
        IConnection connection;
        Comm comm;
        DateTime RunTime;
        int MsgInCount = 0;
        int MsgOutCount = 0;

        public delegate void AddMsgInItem(string[] message);
        public AddMsgInItem addMsgInDelegate;

        public delegate void AddMsgOutItem(string[] message);
        public AddMsgOutItem addMsgOutDelegate;
        public Form1()
        {
            InitializeComponent();
            RunTime = System.DateTime.Now;
            addMsgInDelegate = new AddMsgInItem(AddMsgItemInMethod);
            addMsgOutDelegate = new AddMsgOutItem(AddMsgItemOutMethod);
        }
        public void AddMsgItemInMethod(string[] message)
        {
            AddListViewItem(listView1, message);
            System.Threading.Thread.Sleep(1);
        }

        public void AddMsgItemOutMethod(string[] message)
        {
            AddListViewItem(listView2, message);
            System.Threading.Thread.Sleep(1);
        }

        private void AddMsgIn(string[] param)
        {
            try
            {
                MsgInCount++;
                this.Invoke(this.addMsgInDelegate, new Object[] { param });
            }
            catch (System.Exception ex)
            {
                //ShowError(ex.Message);
            }
        }

        private void AddMsgOut(string[] param)
        {
            try
            {
                MsgOutCount++;
                this.Invoke(this.addMsgOutDelegate, new Object[] { param });
            }
            catch (System.Exception ex)
            {
                //ShowError(ex.Message);
            }
        }


        private void btn_clear_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView3.Items.Clear();
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
            listView1.View = View.Details;
            listView1.Columns.Add("时间", 60);
            listView1.Columns.Add("uuid", 230, HorizontalAlignment.Center);
            listView1.Columns.Add("mode", 40, HorizontalAlignment.Center);
            listView1.Columns.Add("index", 45, HorizontalAlignment.Center);            
            listView1.Columns.Add("level", 45, HorizontalAlignment.Center);
            listView1.Columns.Add("board", 260); 

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
            listView2.Columns.Add("UUID", 250, HorizontalAlignment.Center);
            listView2.Columns.Add("index", 50, HorizontalAlignment.Center);
            listView2.Columns.Add("result", 200, HorizontalAlignment.Center);

            listView3.GridLines = true;
            //单选时,选择整行
            listView3.FullRowSelect = true;
            //显示方式
            listView3.View = View.Details;
            //没有足够的空间显示时,是否添加滚动条
            listView3.Scrollable = true;
            //是否可以选择多行
            listView3.MultiSelect = false;

            listView3.View = View.Details;
            listView3.Columns.Add("  时间", 60, HorizontalAlignment.Center);
            listView3.Columns.Add("输入数量", 100, HorizontalAlignment.Center);
            listView3.Columns.Add("输出数量", 100, HorizontalAlignment.Center);
            listView3.Columns.Add("剩余数量", 60, HorizontalAlignment.Center);
            listView3.Columns.Add("CPU", 40, HorizontalAlignment.Center);
            listView3.Columns.Add("剩余内存", 80, HorizontalAlignment.Center);
            listView3.Columns.Add("启动时间", 130, HorizontalAlignment.Center);
            listView3.Columns.Add("运行时间", 120, HorizontalAlignment.Center);

        }

        private void AddListViewItem(ListView listView, string[] array, int showLines = 35)
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

        private void Form1_Load(object sender, EventArgs e)
        {
            InitListView();
            comm = new Comm();
            comm.Init();
            this.Text = Setting.title;
            initRabbit();
            recvThread();
        }
        private void initRabbit()
        {
            factory = new ConnectionFactory();
            factory.HostName = "47.96.28.91";
            factory.UserName = "chd1219";
            factory.Password = "jiao19890228";
            connection = factory.CreateConnection();
            

        }

        private void recvThread()
        {
            Thread thread1 = new Thread(new ThreadStart(recv_fanout));  
            thread1.Start();

            Thread thread2 = new Thread(new ThreadStart(send_fanout));  
            thread2.Start();
        }

        private void send_fanout()
        {
            string EXCHANGE_NAME = "send-fanout";
            string ROUTING_KEY = "";
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(EXCHANGE_NAME, "fanout");//广播
                QueueDeclareOk queueOk = channel.QueueDeclare();//每当Consumer连接时，我们需要一个新的，空的queue。因为我们不对老的log感兴趣。幸运的是，如果在声明queue时不指定名字，那么RabbitMQ会随机为我们选择这个名字。
                ////现在我们已经创建了fanout类型的exchange和没有名字的queue（实际上是RabbitMQ帮我们取了名字）。
                ////那exchange怎么样知道它的Message发送到哪个queue呢？答案就是通过bindings：绑定。
                string queueName = queueOk.QueueName;//得到RabbitMQ帮我们取了名字
                channel.QueueBind(queueName, EXCHANGE_NAME, ROUTING_KEY);//不需要指定routing key，设置了fanout,指了也没有用.
                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queueName, true, consumer);
                while (true)
                {
                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();//挂起的操作
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    ShowIn(message);
                }
            } 
        }

        public void recv_fanout()
        {
            const string EXCHANGE_NAME = "recv-fanout";
            const string ROUTING_KEY = "";
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(EXCHANGE_NAME, "fanout");//广播
                QueueDeclareOk queueOk = channel.QueueDeclare();//每当Consumer连接时，我们需要一个新的，空的queue。因为我们不对老的log感兴趣。幸运的是，如果在声明queue时不指定名字，那么RabbitMQ会随机为我们选择这个名字。
                ////现在我们已经创建了fanout类型的exchange和没有名字的queue（实际上是RabbitMQ帮我们取了名字）。
                ////那exchange怎么样知道它的Message发送到哪个queue呢？答案就是通过bindings：绑定。
                string queueName = queueOk.QueueName;//得到RabbitMQ帮我们取了名字
                channel.QueueBind(queueName, EXCHANGE_NAME, ROUTING_KEY);//不需要指定routing key，设置了fanout,指了也没有用.
                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queueName, true, consumer);
                while (true)
                {
                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();//挂起的操作
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    ShowOut(message);
                }
            }
        }

        private void ShowIn(string message)
        {
            NewMsg msg = new NewMsg(message);
            string level = "17";
            if (msg.GetType() == "0")
            {
                level = msg.GetDepth();
            }
            string[] showmsg = { DateTime.Now.ToLongTimeString(),  msg.uuid, msg.GetType(),msg.index, level, msg.GetBoard() };
            if (msg.GetCommandType() == "position")
            {
                AddMsgIn(showmsg);
            }
            comm.WriteInfo(message);
            System.Threading.Thread.Sleep(1);
        }

        private void ShowOut(string message)
        {
            if (JsonSplit.IsJson(message))//传入的json串
            {
                JavaScriptObject jsonObj = JavaScriptConvert.DeserializeObject<JavaScriptObject>(message);
                string index = jsonObj["index"].ToString();
                string uuid = jsonObj["uuid"].ToString();
                string result = jsonObj["result"].ToString();
                string[] names = { DateTime.Now.ToLongTimeString(), uuid, index, result };
                if (message.IndexOf("bestmove") != -1)
                {
                    AddMsgOut(names); 
                }
                comm.WriteInfo(message);
                System.Threading.Thread.Sleep(1);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            string m_time = RunTime.ToString();

            DateTime currentTime = System.DateTime.Now;
            TimeSpan span = currentTime.Subtract(RunTime);
            string m_span = span.Days + "天" + span.Hours + "时" + span.Minutes + "分" + span.Seconds + "秒";

            string m_CPU = comm.getCurrentCpuUsage();
            string m_Memory = comm.getAvailableRAM();

            string[] args = { DateTime.Now.ToLongTimeString(), MsgInCount.ToString(), MsgOutCount.ToString(),(MsgInCount - MsgOutCount).ToString(),m_CPU, m_Memory, m_time, m_span };
            AddListViewItem(listView3, args, 0);
        }

    }
}
