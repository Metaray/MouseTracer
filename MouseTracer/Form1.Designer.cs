namespace MouseTracer
{
    partial class MainWindow
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
            System.Console.WriteLine("Dispose");
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
                trayIcon.Dispose();
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
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tracingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorSchemeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hSVColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blackAndWhiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawClicksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iOGraphColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.tracingToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(428, 24);
            this.MainMenu.TabIndex = 0;
            this.MainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // tracingToolStripMenuItem
            // 
            this.tracingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.pauseToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.statisticsToolStripMenuItem});
            this.tracingToolStripMenuItem.Name = "tracingToolStripMenuItem";
            this.tracingToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.tracingToolStripMenuItem.Text = "Tracing";
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Enabled = false;
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // statisticsToolStripMenuItem
            // 
            this.statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
            this.statisticsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.statisticsToolStripMenuItem.Text = "Statistics";
            this.statisticsToolStripMenuItem.Click += new System.EventHandler(this.statisticsToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colorSchemeToolStripMenuItem,
            this.drawClicksToolStripMenuItem,
            this.drawPathToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // colorSchemeToolStripMenuItem
            // 
            this.colorSchemeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hSVColorToolStripMenuItem,
            this.blackAndWhiteToolStripMenuItem,
            this.iOGraphColorToolStripMenuItem});
            this.colorSchemeToolStripMenuItem.Name = "colorSchemeToolStripMenuItem";
            this.colorSchemeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.colorSchemeToolStripMenuItem.Text = "Color Scheme";
            // 
            // hSVColorToolStripMenuItem
            // 
            this.hSVColorToolStripMenuItem.Name = "hSVColorToolStripMenuItem";
            this.hSVColorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.hSVColorToolStripMenuItem.Text = "HSV Color";
            this.hSVColorToolStripMenuItem.Click += new System.EventHandler(this.colorToolStripMenuItem_Click);
            // 
            // blackAndWhiteToolStripMenuItem
            // 
            this.blackAndWhiteToolStripMenuItem.Name = "blackAndWhiteToolStripMenuItem";
            this.blackAndWhiteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.blackAndWhiteToolStripMenuItem.Text = "Black and White";
            this.blackAndWhiteToolStripMenuItem.Click += new System.EventHandler(this.colorToolStripMenuItem_Click);
            // 
            // drawClicksToolStripMenuItem
            // 
            this.drawClicksToolStripMenuItem.Checked = true;
            this.drawClicksToolStripMenuItem.CheckOnClick = true;
            this.drawClicksToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.drawClicksToolStripMenuItem.Name = "drawClicksToolStripMenuItem";
            this.drawClicksToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.drawClicksToolStripMenuItem.Text = "Draw Clicks";
            this.drawClicksToolStripMenuItem.CheckedChanged += new System.EventHandler(this.drawClicksToolStripMenuItem_CheckedChanged);
            // 
            // iOGraphColorToolStripMenuItem
            // 
            this.iOGraphColorToolStripMenuItem.Name = "iOGraphColorToolStripMenuItem";
            this.iOGraphColorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.iOGraphColorToolStripMenuItem.Text = "IOGraph Color";
            this.iOGraphColorToolStripMenuItem.Click += new System.EventHandler(this.colorToolStripMenuItem_Click);
            // 
            // drawPathToolStripMenuItem
            // 
            this.drawPathToolStripMenuItem.Checked = true;
            this.drawPathToolStripMenuItem.CheckOnClick = true;
            this.drawPathToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.drawPathToolStripMenuItem.Name = "drawPathToolStripMenuItem";
            this.drawPathToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.drawPathToolStripMenuItem.Text = "Draw Path";
            this.drawPathToolStripMenuItem.CheckedChanged += new System.EventHandler(this.drawPathToolStripMenuItem_CheckedChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 282);
            this.Controls.Add(this.MainMenu);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.MainMenu;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "MouseTracer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Resize += new System.EventHandler(this.MainWindow_Resize);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tracingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawClicksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statisticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorSchemeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hSVColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blackAndWhiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iOGraphColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawPathToolStripMenuItem;
    }
}

