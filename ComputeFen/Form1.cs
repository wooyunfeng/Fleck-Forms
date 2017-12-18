using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Data.SQLite;

namespace ComputeFen
{
    public partial class Form1 : Form
    {
        public delegate void AddMsgItem(string[] message);
        public AddMsgItem addMsgDelegate;
        Process pProcess;
        DateTime EngineRunTime;
        private static StreamWriter PipeWriter { get; set; }
        Thread pipeThread;
        StreamReader reader;
        bool bPipeRun;
        bool bRun;
        string strInfo;
        bool bdealing = false;
        int dealcount = 0;
        DateTime startdeal;
        DateTime starttime;
        RedisManage redis;
        string filepath = "";
        string engineer = "";
        SQLiteHelper positionSQLite;
        string board;
        public Form1()
        {
            addMsgDelegate = new AddMsgItem(AddMsgItemMethod);

            InitializeComponent();

            InitListView();

            filepath = textFilePath.Text;
            engineer = textEngineer.Text;
        }
  
        private void button1_Click(object sender, EventArgs e)
        {
            Stream mystream;
            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect = false;//允许同时选择多个文件
            op.Filter = "txt files(*.db)|*.db|All files(*.*)|*.*";
            op.FilterIndex = 2;
            op.RestoreDirectory = true;
            if (op.ShowDialog() == DialogResult.OK)
            {
                if ((mystream = op.OpenFile()) != null)
                {
                    for (int fi = 0; fi < op.FileNames.Length; fi++)
                    {
                        filepath = op.FileNames[fi].ToString();
                        textFilePath.Text = filepath;
                    }
                    mystream.Close();
                }
            }          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stream mystream;
            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect = false;//允许同时选择多个文件
            op.Filter = "txt files(*.exe)|*.exe|All files(*.*)|*.*";
            op.FilterIndex = 2;
            op.RestoreDirectory = true;
            if (op.ShowDialog() == DialogResult.OK)
            {
                if ((mystream = op.OpenFile()) != null)
                {
                    for (int fi = 0; fi < op.FileNames.Length; fi++)
                    {
                        engineer = op.FileNames[fi].ToString();
                        textEngineer.Text = engineer;
                    }
                    mystream.Close();
                }
            }          
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StartPipeThread();
            redis = new RedisManage(textAddr.Text);
            StartDealThread();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Deal()
        {
            positionSQLite = new SQLiteHelper(filepath);
            string sql = String.Format("select * from board order by visit desc ");
            SQLiteDataReader reader = positionSQLite.SQLite_ExecuteReader(sql);
            while (reader.Read())
            {
                board = reader["fen"].ToString();
                string vkey = reader["vkey"].ToString();
                bdealing = true;
                startdeal = DateTime.Now;
                PipeWriter.Write("position fen "+board + "\r\n");
                PipeWriter.Write("go depth " + textDepth.Text + "\r\n");
                while (bdealing)
                {
                    Thread.Sleep(100);
                }
            }                          
        }

        public void StartDealThread()
        {
            Thread thread = new Thread(new ThreadStart(Deal));
            thread.IsBackground = true;
            thread.Start();
        }
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
                strInfo = "KillPipeThread:" + ex.Message;
            }
        }

        public void PipeThread()
        {
            EngineRunTime = System.DateTime.Now;
            int intDepth = 0;
            string EnginePath = engineer;

            string line = "";
            string[] linearray = new string[3];

            int nLevel = Int32.Parse(textDepth.Text);
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
                            if (intDepth > 0 && intDepth < 32)
                            {
                                listinfo[intDepth - 1] = line;
                            }
                        }

                        if (line.IndexOf("bestmove") != -1)
                        {
                            bdealing = false;
                            dealcount++;
                            for (int i = 0; i < nLevel; i++)
                            {
                                redis.setItemToList(board, listinfo[i]);
                            }

                            Array.Clear(listinfo, 0, listinfo.Length);

                            linearray[0] = board;
                            linearray[1] = " depth " + intDepth.ToString() + " " + line;
                            AddMsg(linearray);
                        }
                        Thread.Sleep(10);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bdealing = false;
                resetEngine();
                strInfo = "PipeThread:" + ex.Message;
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
            listView1.Columns.Add("输入", 250);
            listView1.Columns.Add("输出", 250);
        }

        private void AddMsg(string[] param)
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
      

    }
}
