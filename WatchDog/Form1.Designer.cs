namespace WatchDog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuShowItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuExitItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuAddItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuDelItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuShowItem,
            this.ToolStripMenuExitItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(131, 48);
            // 
            // ToolStripMenuShowItem
            // 
            this.ToolStripMenuShowItem.Name = "ToolStripMenuShowItem";
            this.ToolStripMenuShowItem.Size = new System.Drawing.Size(130, 22);
            this.ToolStripMenuShowItem.Text = "显示主窗口";
            this.ToolStripMenuShowItem.Click += new System.EventHandler(this.ToolStripMenuShowItem_Click);
            // 
            // ToolStripMenuExitItem
            // 
            this.ToolStripMenuExitItem.Name = "ToolStripMenuExitItem";
            this.ToolStripMenuExitItem.Size = new System.Drawing.Size(130, 22);
            this.ToolStripMenuExitItem.Text = "退出";
            this.ToolStripMenuExitItem.Click += new System.EventHandler(this.ToolStripMenuExitItem_Click);
            // 
            // listView1
            // 
            this.listView1.ContextMenuStrip = this.contextMenuStrip2;
            this.listView1.Location = new System.Drawing.Point(1, 8);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(594, 260);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            this.listView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseUp);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuAddItem,
            this.ToolStripMenuDelItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(95, 48);
            // 
            // ToolStripMenuAddItem
            // 
            this.ToolStripMenuAddItem.Name = "ToolStripMenuAddItem";
            this.ToolStripMenuAddItem.Size = new System.Drawing.Size(94, 22);
            this.ToolStripMenuAddItem.Text = "添加";
            this.ToolStripMenuAddItem.Click += new System.EventHandler(this.ToolStripMenuAddItem_Click);
            // 
            // ToolStripMenuDelItem
            // 
            this.ToolStripMenuDelItem.Name = "ToolStripMenuDelItem";
            this.ToolStripMenuDelItem.Size = new System.Drawing.Size(94, 22);
            this.ToolStripMenuDelItem.Text = "删除";
            this.ToolStripMenuDelItem.Click += new System.EventHandler(this.ToolStripMenuDelItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 273);
            this.Controls.Add(this.listView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuExitItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuShowItem;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuAddItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuDelItem;
    }
}

