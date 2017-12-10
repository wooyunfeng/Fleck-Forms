using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;
using Newtonsoft.Json;
using Fleck_Forms;

namespace AnalyseLogs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitListView();
            redis = new RedisManage();
            sql = new SQLiteManage();
            timer1.Enabled = true;
        }

        ArrayList filepath = new ArrayList();

        ArrayList fileline = new ArrayList();
        
        private void button_brower_Click(object sender, EventArgs e)
        {
            filepath.Clear();
            Stream mystream;
            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect=true;//允许同时选择多个文件
            op.Filter = "txt files(*.txt)|*.txt|All files(*.*)|*.*";
            op.FilterIndex = 2;
            op.RestoreDirectory = true;
            if (op.ShowDialog() == DialogResult.OK)
            {
                if ((mystream = op.OpenFile()) != null)
                {
                    for (int fi = 0; fi < op.FileNames.Length; fi++)
                    {
                        filepath.Add(op.FileNames[fi].ToString());
                        string[] info = { (fi+1).ToString(), op.FileNames[fi].ToString(), "" };
                        AddListViewItem(listView1, info);
                    }
                    mystream.Close();
                }
            }          
        }

        public static void ListFiles(FileSystemInfo info, string Ext, TextBox obj)
        {
            if (!info.Exists) return;

            DirectoryInfo dir = info as DirectoryInfo;
            //不是目录 
            if (dir == null) return;
            try
            {

                FileSystemInfo[] files = dir.GetFileSystemInfos();
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo file = files[i] as FileInfo;
                    //是文件
                    if (file != null && file.Extension.ToUpper() == "." + Ext.ToUpper())
                    {
                        obj.Text = obj.Text + file.FullName + "\r\n";
                        obj.Refresh();
                    }
                    //对于子目录，进行递归调用 
                    else
                        ListFiles(files[i], Ext, obj);

                }
            }
            catch (UnauthorizedAccessException ex)
            {
                obj.Text = obj.Text + ex.Message;
            }

        }

        private void button_deal_Click(object sender, EventArgs e)
        {
            StartThread();  
        }
        int value = 0;
        int max = 0;
        bool bset = false;
        private void ParseFile()
        {
            foreach (var file in filepath)
            {
                bset = false;
                max = 0;
                value = 0;
                timer1.Enabled = true;
                Thread.Sleep(100);
                loadfile((string)file);
                max = fileline.Count;                
                foreach (var line in fileline)
                {
                    ParseLine(line);
                    value++;
                }
                MessageBox.Show(file+" Done");
            }
           
        }

        public void StartThread()
        {
            Thread pthread;
            pthread = new Thread(new ThreadStart(ParseFile));
            pthread.IsBackground = true;
            pthread.Start();
        }
        public string uuid { get; set; }
        public string id { get; set; }
        private string result { get; set; }
        public string index { get; set; }
        public string commandtype { get; set; }
        public string command { get; set; }
        RedisManage redis;
        SQLiteManage sql;
        private void ParseLine(object line)
        {
            try
            {
                string message = (string)line;
                if (message.Length > 50)
                {
                    int seek = message.IndexOf("{");
                    string str = message.Substring(seek, message.Length - seek);
                    if (JsonSplit.IsJson(str))//传入的json串
                    {
                        JavaScriptObject jsonObj = JavaScriptConvert.DeserializeObject<JavaScriptObject>(str);
                        index = jsonObj["index"].ToString();
                        if (str.IndexOf("uuid") != -1)
                        {
                            uuid = jsonObj["uuid"].ToString();
                            result = jsonObj["result"].ToString();
                            if (result.IndexOf("bestmove") != -1)
                            {
                                sql.Update(uuid, index, result);
                            }
                            command = sql.getCommand(uuid, index);
                            redis.setItemToList(command, result);
                        }
                        else
                        {
                            id = jsonObj["id"].ToString();
                            command = jsonObj["command"].ToString();
                            string[] args = { index, id, command };
                            sql.Insert(args);
                        }
                    }
                    else
                    {
                        Console.WriteLine(message);
                    }
                }
               
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }         
                   
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
            listView1.Columns.Add("序号", 50);
            listView1.Columns.Add("路径", 500);
            listView1.Columns.Add("处理结果", 80);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bset)
            {
                if (progressBar1.Value <= progressBar1.Maximum)
                {
                    progressBar1.Value = value;
                }
                else
                {
                    timer1.Enabled = false;
                }
            }
            else
            {
                if (max > 0)
                {
                    bset = true;
                    progressBar1.Maximum = max;
                }
            }
            
            
        }


        private void loadfile(string file)
        {
            fileline.Clear();
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            StreamReader read = new StreamReader(fs, Encoding.Default);
            string strReadline;
            while ((strReadline = read.ReadLine()) != null)
            {
                fileline.Add(strReadline);
                // strReadline即为按照行读取的字符串
            }
            fs.Close();
            read.Close();
        }
    }
}
