namespace Fleck_RabbitMQ
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
            this.listView4 = new Fleck_Forms.ListViewNF();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listView1 = new Fleck_Forms.ListViewNF();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btn_expend = new System.Windows.Forms.Button();
            this.btn_closeall = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listView2 = new Fleck_Forms.ListViewNF();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listViewNF1 = new Fleck_Forms.ListViewNF();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.m_port = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_clear = new System.Windows.Forms.Button();
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
            this.panel1.Controls.Add(this.listView4);
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(737, 649);
            this.panel1.TabIndex = 1;
            // 
            // listView4
            // 
            this.listView4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listView4.Location = new System.Drawing.Point(0, 613);
            this.listView4.Name = "listView4";
            this.listView4.Size = new System.Drawing.Size(737, 36);
            this.listView4.TabIndex = 48;
            this.listView4.UseCompatibleStateImageBehavior = false;
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
            this.tabControl1.Size = new System.Drawing.Size(731, 617);
            this.tabControl1.TabIndex = 47;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listView1);
            this.tabPage1.Controls.Add(this.treeView1);
            this.tabPage1.Controls.Add(this.btn_expend);
            this.tabPage1.Controls.Add(this.btn_closeall);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(723, 591);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "在线用户";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(169, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(554, 591);
            this.listView1.TabIndex = 34;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(2, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(161, 553);
            this.treeView1.TabIndex = 26;
            // 
            // btn_expend
            // 
            this.btn_expend.Location = new System.Drawing.Point(28, 562);
            this.btn_expend.Name = "btn_expend";
            this.btn_expend.Size = new System.Drawing.Size(41, 23);
            this.btn_expend.TabIndex = 28;
            this.btn_expend.Text = "展开";
            this.btn_expend.UseVisualStyleBackColor = true;
            this.btn_expend.Click += new System.EventHandler(this.btn_expend_Click);
            // 
            // btn_closeall
            // 
            this.btn_closeall.Location = new System.Drawing.Point(79, 562);
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
            this.tabPage2.Size = new System.Drawing.Size(723, 591);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "输入";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listView2
            // 
            this.listView2.Location = new System.Drawing.Point(-2, 0);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(727, 591);
            this.listView2.TabIndex = 35;
            this.listView2.UseCompatibleStateImageBehavior = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listViewNF1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(723, 591);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "引擎";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // listViewNF1
            // 
            this.listViewNF1.Location = new System.Drawing.Point(-4, 2);
            this.listViewNF1.Name = "listViewNF1";
            this.listViewNF1.Size = new System.Drawing.Size(731, 586);
            this.listViewNF1.TabIndex = 38;
            this.listViewNF1.UseCompatibleStateImageBehavior = false;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.m_port);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.btn_clear);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(723, 591);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "设置";
            this.tabPage4.UseVisualStyleBackColor = true;
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
            this.ClientSize = new System.Drawing.Size(737, 649);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "象棋微学堂";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
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
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button btn_expend;
        private System.Windows.Forms.Button btn_closeall;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label m_port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Timer timer1;
        private Fleck_Forms.ListViewNF listView1;
        private Fleck_Forms.ListViewNF listView2;
        private Fleck_Forms.ListViewNF listView4;
        private System.Windows.Forms.TabPage tabPage3;
        private Fleck_Forms.ListViewNF listViewNF1;
    }
}

