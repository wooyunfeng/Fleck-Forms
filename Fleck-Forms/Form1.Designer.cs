﻿namespace Fleck_Forms
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listViewNF1 = new Fleck_Forms.ListViewNF();
            this.listView1 = new Fleck_Forms.ListViewNF();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btn_expend = new System.Windows.Forms.Button();
            this.btn_closeall = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listView2 = new Fleck_Forms.ListViewNF();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listViewNF2 = new Fleck_Forms.ListViewNF();
            this.listView3 = new Fleck_Forms.ListViewNF();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btn_reset = new System.Windows.Forms.Button();
            this.m_Mysql = new System.Windows.Forms.CheckBox();
            this.m_port = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_clear = new System.Windows.Forms.Button();
            this.m_Redis = new System.Windows.Forms.CheckBox();
            this.m_CloudApi = new System.Windows.Forms.CheckBox();
            this.listView4 = new Fleck_Forms.ListViewNF();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Controls.Add(this.listView4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(734, 422);
            this.panel1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(3, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(731, 392);
            this.tabControl1.TabIndex = 47;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listViewNF1);
            this.tabPage1.Controls.Add(this.listView1);
            this.tabPage1.Controls.Add(this.treeView1);
            this.tabPage1.Controls.Add(this.btn_expend);
            this.tabPage1.Controls.Add(this.btn_closeall);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(723, 366);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "在线用户";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listViewNF1
            // 
            this.listViewNF1.Location = new System.Drawing.Point(214, 3);
            this.listViewNF1.Name = "listViewNF1";
            this.listViewNF1.Size = new System.Drawing.Size(510, 363);
            this.listViewNF1.TabIndex = 36;
            this.listViewNF1.UseCompatibleStateImageBehavior = false;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(105, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(109, 363);
            this.listView1.TabIndex = 33;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(2, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(103, 331);
            this.treeView1.TabIndex = 26;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // btn_expend
            // 
            this.btn_expend.Location = new System.Drawing.Point(6, 338);
            this.btn_expend.Name = "btn_expend";
            this.btn_expend.Size = new System.Drawing.Size(41, 23);
            this.btn_expend.TabIndex = 28;
            this.btn_expend.Text = "展开";
            this.btn_expend.UseVisualStyleBackColor = true;
            this.btn_expend.Click += new System.EventHandler(this.btn_expend_Click);
            // 
            // btn_closeall
            // 
            this.btn_closeall.Location = new System.Drawing.Point(57, 338);
            this.btn_closeall.Name = "btn_closeall";
            this.btn_closeall.Size = new System.Drawing.Size(41, 23);
            this.btn_closeall.TabIndex = 27;
            this.btn_closeall.Text = "折叠";
            this.btn_closeall.UseVisualStyleBackColor = true;
            this.btn_closeall.Click += new System.EventHandler(this.btn_closeall_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listView2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(723, 366);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "输入";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listView2
            // 
            this.listView2.Location = new System.Drawing.Point(0, 0);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(727, 366);
            this.listView2.TabIndex = 34;
            this.listView2.UseCompatibleStateImageBehavior = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listViewNF2);
            this.tabPage3.Controls.Add(this.listView3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(723, 366);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "引擎";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // listViewNF2
            // 
            this.listViewNF2.Location = new System.Drawing.Point(2, 2);
            this.listViewNF2.Name = "listViewNF2";
            this.listViewNF2.Size = new System.Drawing.Size(727, 103);
            this.listViewNF2.TabIndex = 35;
            this.listViewNF2.UseCompatibleStateImageBehavior = false;
            // 
            // listView3
            // 
            this.listView3.Location = new System.Drawing.Point(3, 111);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(727, 249);
            this.listView3.TabIndex = 35;
            this.listView3.UseCompatibleStateImageBehavior = false;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btn_reset);
            this.tabPage4.Controls.Add(this.m_Mysql);
            this.tabPage4.Controls.Add(this.m_port);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.btn_clear);
            this.tabPage4.Controls.Add(this.m_Redis);
            this.tabPage4.Controls.Add(this.m_CloudApi);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(723, 366);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "设置";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btn_reset
            // 
            this.btn_reset.Location = new System.Drawing.Point(181, 170);
            this.btn_reset.Name = "btn_reset";
            this.btn_reset.Size = new System.Drawing.Size(75, 23);
            this.btn_reset.TabIndex = 59;
            this.btn_reset.Text = "重启引擎";
            this.btn_reset.UseVisualStyleBackColor = true;
            this.btn_reset.Click += new System.EventHandler(this.btn_reset_Click);
            // 
            // m_Mysql
            // 
            this.m_Mysql.AutoSize = true;
            this.m_Mysql.Checked = true;
            this.m_Mysql.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_Mysql.Location = new System.Drawing.Point(555, 177);
            this.m_Mysql.Name = "m_Mysql";
            this.m_Mysql.Size = new System.Drawing.Size(54, 16);
            this.m_Mysql.TabIndex = 58;
            this.m_Mysql.Text = "MySQL";
            this.m_Mysql.UseVisualStyleBackColor = true;
            this.m_Mysql.CheckedChanged += new System.EventHandler(this.m_Mysql_CheckedChanged);
            // 
            // m_port
            // 
            this.m_port.AutoSize = true;
            this.m_port.Location = new System.Drawing.Point(365, 177);
            this.m_port.Name = "m_port";
            this.m_port.Size = new System.Drawing.Size(29, 12);
            this.m_port.TabIndex = 57;
            this.m_port.Text = "port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(329, 177);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 56;
            this.label2.Text = "端口：";
            // 
            // btn_clear
            // 
            this.btn_clear.Location = new System.Drawing.Point(78, 170);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(75, 23);
            this.btn_clear.TabIndex = 53;
            this.btn_clear.Text = "清空消息";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // m_Redis
            // 
            this.m_Redis.AutoSize = true;
            this.m_Redis.Checked = true;
            this.m_Redis.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_Redis.Location = new System.Drawing.Point(486, 175);
            this.m_Redis.Name = "m_Redis";
            this.m_Redis.Size = new System.Drawing.Size(54, 16);
            this.m_Redis.TabIndex = 54;
            this.m_Redis.Text = "Redis";
            this.m_Redis.UseVisualStyleBackColor = true;
            this.m_Redis.CheckedChanged += new System.EventHandler(this.m_Redis_CheckedChanged);
            // 
            // m_CloudApi
            // 
            this.m_CloudApi.AutoSize = true;
            this.m_CloudApi.Checked = true;
            this.m_CloudApi.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_CloudApi.Location = new System.Drawing.Point(414, 175);
            this.m_CloudApi.Name = "m_CloudApi";
            this.m_CloudApi.Size = new System.Drawing.Size(48, 16);
            this.m_CloudApi.TabIndex = 55;
            this.m_CloudApi.Text = "云库";
            this.m_CloudApi.UseVisualStyleBackColor = true;
            this.m_CloudApi.CheckedChanged += new System.EventHandler(this.m_CloudApi_CheckedChanged);
            // 
            // listView4
            // 
            this.listView4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listView4.Location = new System.Drawing.Point(0, 389);
            this.listView4.Name = "listView4";
            this.listView4.Size = new System.Drawing.Size(734, 33);
            this.listView4.TabIndex = 43;
            this.listView4.UseCompatibleStateImageBehavior = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 422);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "象棋微学堂";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private ListViewNF listView4;
        private ListViewNF listView3;
        private ListViewNF listView2;
        private ListViewNF listView1;
        private System.Windows.Forms.Button btn_closeall;
        private System.Windows.Forms.Button btn_expend;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private ListViewNF listViewNF1;
        private ListViewNF listViewNF2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.CheckBox m_Redis;
        private System.Windows.Forms.CheckBox m_CloudApi;
        private System.Windows.Forms.Label m_port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox m_Mysql;
        private System.Windows.Forms.Button btn_reset;

    }
}

