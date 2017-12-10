namespace WatchDog
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_brower = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_OK = new System.Windows.Forms.Button();
            this.button_Cancle = new System.Windows.Forms.Button();
            this.textPath = new System.Windows.Forms.TextBox();
            this.textSpan = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_brower
            // 
            this.button_brower.Location = new System.Drawing.Point(366, 12);
            this.button_brower.Name = "button_brower";
            this.button_brower.Size = new System.Drawing.Size(75, 23);
            this.button_brower.TabIndex = 0;
            this.button_brower.Text = "浏览";
            this.button_brower.UseVisualStyleBackColor = true;
            this.button_brower.Click += new System.EventHandler(this.button_brower_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "路径：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "时间间隔：";
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(109, 88);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 23);
            this.button_OK.TabIndex = 3;
            this.button_OK.Text = "确定";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_Cancle
            // 
            this.button_Cancle.Location = new System.Drawing.Point(277, 88);
            this.button_Cancle.Name = "button_Cancle";
            this.button_Cancle.Size = new System.Drawing.Size(75, 23);
            this.button_Cancle.TabIndex = 4;
            this.button_Cancle.Text = "取消";
            this.button_Cancle.UseVisualStyleBackColor = true;
            this.button_Cancle.Click += new System.EventHandler(this.button_Cancle_Click);
            // 
            // textPath
            // 
            this.textPath.Location = new System.Drawing.Point(81, 13);
            this.textPath.Name = "textPath";
            this.textPath.Size = new System.Drawing.Size(267, 21);
            this.textPath.TabIndex = 5;
            // 
            // textSpan
            // 
            this.textSpan.Location = new System.Drawing.Point(81, 48);
            this.textSpan.Name = "textSpan";
            this.textSpan.Size = new System.Drawing.Size(87, 21);
            this.textSpan.TabIndex = 6;
            this.textSpan.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textSpan_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(175, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "秒";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 123);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textSpan);
            this.Controls.Add(this.textPath);
            this.Controls.Add(this.button_Cancle);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_brower);
            this.Name = "Form2";
            this.Text = "设置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_brower;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Button button_Cancle;
        private System.Windows.Forms.TextBox textPath;
        private System.Windows.Forms.TextBox textSpan;
        private System.Windows.Forms.Label label3;
    }
}