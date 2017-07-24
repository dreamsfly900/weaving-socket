namespace 统一登录服务
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.开始ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.启动ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重写加载ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.启动WEB网关ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重写加载WEB节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.开放端口ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.内部端口ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.端口比例ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox3 = new System.Windows.Forms.ToolStripComboBox();
            this.最大承载人数ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox4 = new System.Windows.Forms.ToolStripTextBox();
            this.节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.增加节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.从网关节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.增加ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.服务节点节点状态 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.启动byte网关ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.服务节点节点状态.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 380);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(616, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(56, 17);
            this.toolStripStatusLabel1.Text = "连接状态";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.开始ToolStripMenuItem,
            this.节点ToolStripMenuItem,
            this.从网关节点ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(616, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 开始ToolStripMenuItem
            // 
            this.开始ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.启动byte网关ToolStripMenuItem,
            this.启动ToolStripMenuItem,
            this.重写加载ToolStripMenuItem,
            this.启动WEB网关ToolStripMenuItem,
            this.重写加载WEB节点ToolStripMenuItem,
            this.开放端口ToolStripMenuItem,
            this.内部端口ToolStripMenuItem,
            this.端口比例ToolStripMenuItem,
            this.最大承载人数ToolStripMenuItem});
            this.开始ToolStripMenuItem.Name = "开始ToolStripMenuItem";
            this.开始ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.开始ToolStripMenuItem.Text = "开始";
            // 
            // 启动ToolStripMenuItem
            // 
            this.启动ToolStripMenuItem.Name = "启动ToolStripMenuItem";
            this.启动ToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.启动ToolStripMenuItem.Text = "启动普通网关";
            this.启动ToolStripMenuItem.Click += new System.EventHandler(this.启动ToolStripMenuItem_Click);
            // 
            // 重写加载ToolStripMenuItem
            // 
            this.重写加载ToolStripMenuItem.Name = "重写加载ToolStripMenuItem";
            this.重写加载ToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.重写加载ToolStripMenuItem.Text = "重写加载节点";
            this.重写加载ToolStripMenuItem.Click += new System.EventHandler(this.重写加载ToolStripMenuItem_Click);
            // 
            // 启动WEB网关ToolStripMenuItem
            // 
            this.启动WEB网关ToolStripMenuItem.Name = "启动WEB网关ToolStripMenuItem";
            this.启动WEB网关ToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.启动WEB网关ToolStripMenuItem.Text = "启动WEB网关";
            this.启动WEB网关ToolStripMenuItem.Click += new System.EventHandler(this.启动WEB网关ToolStripMenuItem_Click);
            // 
            // 重写加载WEB节点ToolStripMenuItem
            // 
            this.重写加载WEB节点ToolStripMenuItem.Name = "重写加载WEB节点ToolStripMenuItem";
            this.重写加载WEB节点ToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.重写加载WEB节点ToolStripMenuItem.Text = "重写加载WEB节点";
            this.重写加载WEB节点ToolStripMenuItem.Click += new System.EventHandler(this.重写加载WEB节点ToolStripMenuItem_Click);
            // 
            // 开放端口ToolStripMenuItem
            // 
            this.开放端口ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1});
            this.开放端口ToolStripMenuItem.Name = "开放端口ToolStripMenuItem";
            this.开放端口ToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.开放端口ToolStripMenuItem.Text = "开放端口";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            this.toolStripTextBox1.Text = "11001";
            // 
            // 内部端口ToolStripMenuItem
            // 
            this.内部端口ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox2});
            this.内部端口ToolStripMenuItem.Name = "内部端口ToolStripMenuItem";
            this.内部端口ToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.内部端口ToolStripMenuItem.Text = "内部端口";
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Size = new System.Drawing.Size(100, 23);
            this.toolStripTextBox2.Text = "9998";
            // 
            // 端口比例ToolStripMenuItem
            // 
            this.端口比例ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox3});
            this.端口比例ToolStripMenuItem.Name = "端口比例ToolStripMenuItem";
            this.端口比例ToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.端口比例ToolStripMenuItem.Text = "通道数量";
            // 
            // toolStripTextBox3
            // 
            this.toolStripTextBox3.Items.AddRange(new object[] {
            "10",
            "100",
            "1000",
            "10000"});
            this.toolStripTextBox3.Name = "toolStripTextBox3";
            this.toolStripTextBox3.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBox3.Text = "10";
            this.toolStripTextBox3.Click += new System.EventHandler(this.toolStripTextBox3_Click);
            // 
            // 最大承载人数ToolStripMenuItem
            // 
            this.最大承载人数ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox4});
            this.最大承载人数ToolStripMenuItem.Name = "最大承载人数ToolStripMenuItem";
            this.最大承载人数ToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.最大承载人数ToolStripMenuItem.Text = "最大承载人数";
            // 
            // toolStripTextBox4
            // 
            this.toolStripTextBox4.Name = "toolStripTextBox4";
            this.toolStripTextBox4.Size = new System.Drawing.Size(100, 23);
            this.toolStripTextBox4.Text = "30000";
            // 
            // 节点ToolStripMenuItem
            // 
            this.节点ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.增加节点ToolStripMenuItem,
            this.删除节点ToolStripMenuItem});
            this.节点ToolStripMenuItem.Name = "节点ToolStripMenuItem";
            this.节点ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.节点ToolStripMenuItem.Text = "服务节点";
            // 
            // 增加节点ToolStripMenuItem
            // 
            this.增加节点ToolStripMenuItem.Name = "增加节点ToolStripMenuItem";
            this.增加节点ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.增加节点ToolStripMenuItem.Text = "增加节点";
            // 
            // 删除节点ToolStripMenuItem
            // 
            this.删除节点ToolStripMenuItem.Name = "删除节点ToolStripMenuItem";
            this.删除节点ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.删除节点ToolStripMenuItem.Text = "删除节点";
            // 
            // 从网关节点ToolStripMenuItem
            // 
            this.从网关节点ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.增加ToolStripMenuItem,
            this.删除ToolStripMenuItem});
            this.从网关节点ToolStripMenuItem.Name = "从网关节点ToolStripMenuItem";
            this.从网关节点ToolStripMenuItem.Size = new System.Drawing.Size(80, 21);
            this.从网关节点ToolStripMenuItem.Text = "从网关节点";
            // 
            // 增加ToolStripMenuItem
            // 
            this.增加ToolStripMenuItem.Name = "增加ToolStripMenuItem";
            this.增加ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.增加ToolStripMenuItem.Text = "增加";
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.删除ToolStripMenuItem.Text = "删除";
            // 
            // 服务节点节点状态
            // 
            this.服务节点节点状态.Controls.Add(this.tabPage1);
            this.服务节点节点状态.Controls.Add(this.tabPage2);
            this.服务节点节点状态.Controls.Add(this.tabPage3);
            this.服务节点节点状态.Dock = System.Windows.Forms.DockStyle.Fill;
            this.服务节点节点状态.HotTrack = true;
            this.服务节点节点状态.Location = new System.Drawing.Point(0, 25);
            this.服务节点节点状态.Name = "服务节点节点状态";
            this.服务节点节点状态.SelectedIndex = 0;
            this.服务节点节点状态.Size = new System.Drawing.Size(616, 355);
            this.服务节点节点状态.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.txtLog);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(608, 329);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "基本信息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "日志：";
            // 
            // txtLog
            // 
            this.txtLog.AcceptsReturn = true;
            this.txtLog.AcceptsTab = true;
            this.txtLog.Location = new System.Drawing.Point(6, 27);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(594, 296);
            this.txtLog.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage2.Size = new System.Drawing.Size(608, 335);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "服务节点节点状态";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(6, 50);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(596, 268);
            this.listBox1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listBox2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage3.Size = new System.Drawing.Size(608, 335);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "从网关节点状态";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 12;
            this.listBox2.Location = new System.Drawing.Point(3, 45);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(596, 268);
            this.listBox2.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // 启动byte网关ToolStripMenuItem
            // 
            this.启动byte网关ToolStripMenuItem.Name = "启动byte网关ToolStripMenuItem";
            this.启动byte网关ToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.启动byte网关ToolStripMenuItem.Text = "启动byte网关";
            this.启动byte网关ToolStripMenuItem.Click += new System.EventHandler(this.启动byte网关ToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(616, 402);
            this.Controls.Add(this.服务节点节点状态);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "统一登录网关";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.服务节点节点状态.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 开始ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 启动ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重写加载ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 节点ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 增加节点ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除节点ToolStripMenuItem;
        private System.Windows.Forms.TabControl 服务节点节点状态;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.ToolStripMenuItem 启动WEB网关ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重写加载WEB节点ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 从网关节点ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 增加ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 开放端口ToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripMenuItem 内部端口ToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripMenuItem 端口比例ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 最大承载人数ToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox4;
        private System.Windows.Forms.ToolStripComboBox toolStripTextBox3;
        private System.Windows.Forms.ToolStripMenuItem 启动byte网关ToolStripMenuItem;
    }
}

