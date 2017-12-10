using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WatchDog
{
    public partial class Form2 : Form
    {
        public string path;
        public string span;
        public Form2()
        {
            InitializeComponent();
            textSpan.Text = "10";
        }

        private void button_brower_Click(object sender, EventArgs e)
        {
            Stream mystream;
            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect = false;//允许同时选择多个文件
            op.Filter = "txt files(*.txt)|*.txt|All files(*.*)|*.*";
            op.FilterIndex = 2;
            op.RestoreDirectory = true;
            if (op.ShowDialog() == DialogResult.OK)
            {
                if ((mystream = op.OpenFile()) != null)
                {
                    for (int fi = 0; fi < op.FileNames.Length; fi++)
                    {
                        textPath.Text = op.FileNames[fi].ToString();
                        Update();
                    }
                    mystream.Close();
                }
            }          
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            path = textPath.Text;
            span = textSpan.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void button_Cancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void textSpan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }  
        }
    }
}
