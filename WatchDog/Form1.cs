using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace WatchDog
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        //查找进程、结束进程
        public void CheckProc(string fullPath)
        {
            int index  = fullPath.LastIndexOf("\\");
            string fileName = fullPath.Substring(index + 1, fullPath.Length-index-5);//文件名
          
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
                    Process.Start(fullPath);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckProc("D:\\Fleck-Forms\\NetRemotingClient\\bin\\Debug\\NetRemotingClient.exe");
        }

    }
}
