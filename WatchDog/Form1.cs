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

namespace WatchDog
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string fullPath;

        //查找进程、结束进程
        public void CheckProc(string fullPath)
        {
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
                        //启动外部程序
                        Process pr = new Process();
                        pr.StartInfo.WorkingDirectory = filePath;
                        pr.StartInfo.FileName = fileName;
                        pr.Start();
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckProc(fullPath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadXml();
            //隐藏窗体
            this.WindowState = FormWindowState.Minimized;
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
                if (xe.GetAttribute("key").ToString() == "fullPath")
                {
                    fullPath = xe.GetAttribute("value").ToString();
                }
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //MessageBox.Show("程序将最小化到系统托盘区");
            e.Cancel = true; // 取消关闭窗体
            this.Hide();
            this.ShowInTaskbar = false;//取消窗体在任务栏的显示
            this.notifyIcon1.Visible = true;//显示托盘图标
        }

        private void 显示主窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.notifyIcon1.Visible = false;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose(true);
            Application.ExitThread();
        }

    }
}
