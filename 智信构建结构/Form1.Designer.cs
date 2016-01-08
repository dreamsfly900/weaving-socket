namespace 智信构建结构
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
            this.button1 = new System.Windows.Forms.Button();
            this.lab_IP = new System.Windows.Forms.Label();
            this.lab_port = new System.Windows.Forms.Label();
            this.txt_IP = new System.Windows.Forms.TextBox();
            this.txt_port = new System.Windows.Forms.TextBox();
            this.lab_info = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(125, 166);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(135, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "加载服务器端插件";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lab_IP
            // 
            this.lab_IP.AutoSize = true;
            this.lab_IP.Location = new System.Drawing.Point(35, 28);
            this.lab_IP.Name = "lab_IP";
            this.lab_IP.Size = new System.Drawing.Size(95, 12);
            this.lab_IP.TabIndex = 1;
            this.lab_IP.Text = "服务器端IP地址:";
            // 
            // lab_port
            // 
            this.lab_port.AutoSize = true;
            this.lab_port.Location = new System.Drawing.Point(35, 72);
            this.lab_port.Name = "lab_port";
            this.lab_port.Size = new System.Drawing.Size(95, 12);
            this.lab_port.TabIndex = 2;
            this.lab_port.Text = "服务器端端口号:";
            // 
            // txt_IP
            // 
            this.txt_IP.Location = new System.Drawing.Point(136, 25);
            this.txt_IP.Name = "txt_IP";
            this.txt_IP.Size = new System.Drawing.Size(168, 21);
            this.txt_IP.TabIndex = 3;
            this.txt_IP.Text = "127.0.0.1";
            // 
            // txt_port
            // 
            this.txt_port.Location = new System.Drawing.Point(136, 69);
            this.txt_port.Name = "txt_port";
            this.txt_port.Size = new System.Drawing.Size(168, 21);
            this.txt_port.TabIndex = 4;
            this.txt_port.Text = "8989";
            // 
            // lab_info
            // 
            this.lab_info.AutoSize = true;
            this.lab_info.ForeColor = System.Drawing.Color.Red;
            this.lab_info.Location = new System.Drawing.Point(133, 103);
            this.lab_info.Name = "lab_info";
            this.lab_info.Size = new System.Drawing.Size(0, 12);
            this.lab_info.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 235);
            this.Controls.Add(this.lab_info);
            this.Controls.Add(this.txt_port);
            this.Controls.Add(this.txt_IP);
            this.Controls.Add(this.lab_port);
            this.Controls.Add(this.lab_IP);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "郑州大河智信服务器端";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lab_IP;
        private System.Windows.Forms.Label lab_port;
        private System.Windows.Forms.TextBox txt_IP;
        private System.Windows.Forms.TextBox txt_port;
        private System.Windows.Forms.Label lab_info;
    }
}

