namespace AnalyseLogs
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button_brower = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.button_deal = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(2, 220);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(599, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // button_brower
            // 
            this.button_brower.Location = new System.Drawing.Point(193, 12);
            this.button_brower.Name = "button_brower";
            this.button_brower.Size = new System.Drawing.Size(75, 23);
            this.button_brower.TabIndex = 1;
            this.button_brower.Text = "选择文件";
            this.button_brower.UseVisualStyleBackColor = true;
            this.button_brower.Click += new System.EventHandler(this.button_brower_Click);
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(2, 41);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(599, 163);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // button_deal
            // 
            this.button_deal.Location = new System.Drawing.Point(336, 12);
            this.button_deal.Name = "button_deal";
            this.button_deal.Size = new System.Drawing.Size(75, 23);
            this.button_deal.TabIndex = 4;
            this.button_deal.Text = "开始处理";
            this.button_deal.UseVisualStyleBackColor = true;
            this.button_deal.Click += new System.EventHandler(this.button_deal_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 256);
            this.Controls.Add(this.button_deal);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.button_brower);
            this.Controls.Add(this.progressBar1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button_brower;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button button_deal;
        private System.Windows.Forms.Timer timer1;

    }
}

