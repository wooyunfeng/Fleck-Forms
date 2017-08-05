using Fleck_Forms;
namespace NetRemotingClient
{
    partial class Client
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
            this.label1 = new System.Windows.Forms.Label();
            this.textDepth = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.labelstarttime = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelruntime = new System.Windows.Forms.Label();
            this.labelinfo = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.labelCPU = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelMemory = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.listView1 = new Fleck_Forms.ListViewNF();
            this.label5 = new System.Windows.Forms.Label();
            this.labelcount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(376, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "深度：";
            // 
            // textDepth
            // 
            this.textDepth.Location = new System.Drawing.Point(420, 4);
            this.textDepth.Name = "textDepth";
            this.textDepth.Size = new System.Drawing.Size(29, 21);
            this.textDepth.TabIndex = 10;
            this.textDepth.Text = "17";
            this.textDepth.TextChanged += new System.EventHandler(this.textDepth_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 409);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 12;
            this.label2.Text = "启动时间：";
            // 
            // labelstarttime
            // 
            this.labelstarttime.AutoSize = true;
            this.labelstarttime.Location = new System.Drawing.Point(71, 409);
            this.labelstarttime.Name = "labelstarttime";
            this.labelstarttime.Size = new System.Drawing.Size(41, 12);
            this.labelstarttime.TabIndex = 12;
            this.labelstarttime.Text = "label2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(176, 409);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "运行时间：";
            // 
            // labelruntime
            // 
            this.labelruntime.AutoSize = true;
            this.labelruntime.Location = new System.Drawing.Point(246, 409);
            this.labelruntime.Name = "labelruntime";
            this.labelruntime.Size = new System.Drawing.Size(41, 12);
            this.labelruntime.TabIndex = 12;
            this.labelruntime.Text = "label2";
            // 
            // labelinfo
            // 
            this.labelinfo.AutoSize = true;
            this.labelinfo.Location = new System.Drawing.Point(480, 9);
            this.labelinfo.Name = "labelinfo";
            this.labelinfo.Size = new System.Drawing.Size(65, 12);
            this.labelinfo.TabIndex = 13;
            this.labelinfo.Text = "象棋微学堂";
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "重连";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(340, 409);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "CPU：";
            // 
            // labelCPU
            // 
            this.labelCPU.AutoSize = true;
            this.labelCPU.Location = new System.Drawing.Point(368, 409);
            this.labelCPU.Name = "labelCPU";
            this.labelCPU.Size = new System.Drawing.Size(41, 12);
            this.labelCPU.TabIndex = 12;
            this.labelCPU.Text = "label2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(406, 409);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "内存：";
            // 
            // labelMemory
            // 
            this.labelMemory.AutoSize = true;
            this.labelMemory.Location = new System.Drawing.Point(442, 409);
            this.labelMemory.Name = "labelMemory";
            this.labelMemory.Size = new System.Drawing.Size(41, 12);
            this.labelMemory.TabIndex = 12;
            this.labelMemory.Text = "label2";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(107, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 15;
            this.button2.Text = "断开";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(202, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 15;
            this.button3.Text = "重启引擎";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(297, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 16;
            this.button4.Text = "清空消息";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(12, 32);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(866, 368);
            this.listView1.TabIndex = 11;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(532, 409);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "处理数量：";
            // 
            // labelcount
            // 
            this.labelcount.AutoSize = true;
            this.labelcount.Location = new System.Drawing.Point(596, 409);
            this.labelcount.Name = "labelcount";
            this.labelcount.Size = new System.Drawing.Size(41, 12);
            this.labelcount.TabIndex = 12;
            this.labelcount.Text = "label2";
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 428);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelinfo);
            this.Controls.Add(this.labelcount);
            this.Controls.Add(this.labelMemory);
            this.Controls.Add(this.labelCPU);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelruntime);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelstarttime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.textDepth);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Client";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Client_FormClosing);
            this.Load += new System.EventHandler(this.Client_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textDepth;
        private ListViewNF listView1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelstarttime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelruntime;
        private System.Windows.Forms.Label labelinfo;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelCPU;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelMemory;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelcount;
    }
}

