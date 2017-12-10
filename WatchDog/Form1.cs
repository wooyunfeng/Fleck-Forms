using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace WatchDog
{
  
    public partial class Form1 : Form
    {
        public Form1()
        {
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        public List<Info> myList { get; set; }
        public int count = 0;
        XmlDocument xmlDoc;
        string xmlPath;
        string slectline;
        //查找进程、结束进程
        public void CheckProc(Info info)
        {
            string fullPath = info.path;
            if (fullPath.Length > 0)
            {
                int index = fullPath.LastIndexOf("\\");
                string filePath = fullPath.Substring(0, index);
                string fileName = fullPath.Substring(index + 1, fullPath.Length - index - 5);//文件名
                string procName = fullPath.Substring(0, fileName.Length - 4);//进程名
                Process[] pro = Process.GetProcesses();//获取已开启的所有进程
                bool bfind = false;
                //遍历所有查找到的进程
                for (int i = 0; i < pro.Length; i++)
                {
                    //判断此进程是否是要查找的进程
                    if (pro[i].ProcessName.ToString().ToLower() == fileName.ToLower())
                    {
                        bfind = true;
                        break;
                    }
                }

                if (!bfind)
                {
                    try
                    {
                        if (File.Exists(fullPath))
                        {
                             //启动外部程序
                            Process pr = new Process();
                            pr.StartInfo.WorkingDirectory = filePath;
                            pr.StartInfo.FileName = fileName;
                            pr.Start();
                            info.stat = "启动中";
                            info.count++;
                            info.time = System.DateTime.Now.ToString();
                        }
                        else
                        {
                            info.stat = "文件不存在";
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    info.stat = "运行正常";
                }
            }            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            count++;
            ShowList();
            foreach (Info info in myList)
            {
                int nSpan = int.Parse(info.span);
                if ((count % nSpan == 1) ||(nSpan == 1))
                {
                    CheckProc(info);
                }
            }  
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "进程监控";

            myList = new List<Info>();
            InitListView();
            xmlPath = ".\\config.xml";
            LoadXml(xmlPath);
            ShowList();
            timer1.Enabled = true;
            //隐藏窗体
            //this.WindowState = FormWindowState.Minimized;
        }

        private void ShowList()
        {
            listView1.Items.Clear();
            foreach (Info info in myList)
            {
                string[] args = { (listView1.Items.Count + 1).ToString(), info.path, info.span, info.stat, info.count.ToString(), info.time };
                AddListViewItem(listView1, args);
            }            
        }

        public void LoadXml(string xmlpath)
        {
            xmlDoc = new XmlDocument();
            if (File.Exists(xmlpath))
            {               
                xmlDoc.Load(xmlpath);
                XmlNodeList xnlist = xmlDoc.SelectNodes("Settings/setting");
                foreach (XmlNode xn in xnlist)
                {
                    XmlNodeList xnl = xn.ChildNodes;
                    Info info = new Info();
                    foreach (XmlNode xn1 in xnl)
                    {
                        XmlElement xe = (XmlElement)xn1;
                        if (xe.Name == "Path")
                        {
                            info.path = xe.InnerText;
                        }
                        if (xe.Name == "Span")
                        {
                            info.span = xe.InnerText;
                            if (IsNumberic(info.span))
                            {
                                myList.Add(info);
                            }
                        }
                    }
                }            
            }
            else
            {
                CreateXmlFile(xmlpath);
            }           
        }

        public void SaveXml()
        {
            xmlDoc.Save(xmlPath);
        }

        public void CreateXmlFile(string xmlpath)
        {
            //创建类型声明节点    
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);
            //创建根节点    
            XmlNode root = xmlDoc.CreateElement("Settings");
            xmlDoc.AppendChild(root);
            try
            {
                xmlDoc.Save(xmlpath);
            }
            catch (Exception e)
            {
                //显示错误信息    
                Console.WriteLine(e.Message);
            }
        }

        public void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

        public void AddNode(Info info)
        {
            XmlNode root = xmlDoc.SelectSingleNode("Settings");
            if (root == null)
            {
                root = xmlDoc.CreateElement("Settings");
            }
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, "setting", null);
            CreateNode(xmlDoc, node, "Path", info.path);
            CreateNode(xmlDoc, node, "Span", info.span);
            root.AppendChild(node);
            SaveXml();
        }

        public void DelNode(Info info)
        {
            XmlElement xe = xmlDoc.DocumentElement; // DocumentElement 获取xml文档对象的根XmlElement.
            string strPath = string.Format("/Settings/setting[Path=\"{0}\"]", info.path);
            XmlElement selectXe = (XmlElement)xe.SelectSingleNode(strPath);  //selectSingleNode 根据XPath表达式,获得符合条件的第一个节点.
            selectXe.ParentNode.RemoveChild(selectXe);
            SaveXml();
        }

        private bool IsNumberic(string oText)
        {
            try
            {
                int var1 = Convert.ToInt32(oText);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.Show();
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
            listView1.Columns.Add("序号", 40);
            listView1.Columns.Add("路径", 200, HorizontalAlignment.Center);
            listView1.Columns.Add("间隔(s)", 60, HorizontalAlignment.Center);
            listView1.Columns.Add("运行状态", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("启动次数", 60, HorizontalAlignment.Center);
            listView1.Columns.Add("启动时间", 140, HorizontalAlignment.Center);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {            
            e.Cancel = true; // 取消关闭窗体
            this.Hide();
            this.ShowInTaskbar = false;//取消窗体在任务栏的显示
            this.notifyIcon1.Visible = true;//显示托盘图标            
        }

        private void ToolStripMenuShowItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.notifyIcon1.Visible = false;
        }

        private void ToolStripMenuExitItem_Click(object sender, EventArgs e)
        {
            SaveXml();
            this.Dispose(true);
            Application.ExitThread();
        }

        private void ToolStripMenuAddItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.StartPosition = FormStartPosition.CenterScreen;
            if (form2.ShowDialog() == DialogResult.OK)
            {
                Info info = new Info();
                info.path = form2.path;
                info.span = form2.span;
                string[] args = { (listView1.Items.Count+1).ToString(), info.path, info.span, "" };
                foreach (Info i in myList)
                {
                    if (i.path == info.path)
                    {
                        MessageBox.Show("文件已存在，请删除重试！");
                        return;
                    }
                }
                AddNode(info);
                myList.Add(info);
            }
        }

        private void ToolStripMenuDelItem_Click(object sender, EventArgs e)
        {
            foreach (Info info in myList)
            {
                if (info.path == slectline)
                {
                    myList.Remove(info);
                    DelNode(info);
                    break;
                }
            }              
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = listView1.GetItemAt(e.X, e.Y);
            ListViewItem.ListViewSubItem lvsi = lvi.GetSubItemAt(e.X, e.Y);

            int colIndex = lvi.SubItems.IndexOf(lvsi);

            MessageBox.Show(lvi.SubItems[1].Text);
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = listView1.GetItemAt(e.X, e.Y);
            if (lvi != null)
            {
                slectline = lvi.SubItems[1].Text;
            } 
        }

    }
}
