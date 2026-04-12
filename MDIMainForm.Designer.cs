namespace VortexTrade
{
    partial class MDIMainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            menuStrip = new MenuStrip();
            temaMenu = new ToolStripMenuItem();
            matrixGreen = new ToolStripMenuItem();
            amberTerminal = new ToolStripMenuItem();
            oceanBlue = new ToolStripMenuItem();
            violetNeon = new ToolStripMenuItem();
            hakkindaMenu = new ToolStripMenuItem();

            statusStrip = new StatusStrip();
            lblRam = new ToolStripStatusLabel();
            lblCpu = new ToolStripStatusLabel();
            lblThreads = new ToolStripStatusLabel();
            lblNetIn = new ToolStripStatusLabel();
            lblNetOut = new ToolStripStatusLabel();
            lblSpacer = new ToolStripStatusLabel();
            lblUptime = new ToolStripStatusLabel();

            monitorTimer = new System.Windows.Forms.Timer(components);

            menuStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();

            // menuStrip
            menuStrip.Items.AddRange(new ToolStripItem[] {
                temaMenu, hakkindaMenu });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new Padding(4, 2, 0, 2);
            menuStrip.TabIndex = 0;

            // temaMenu
            temaMenu.DropDownItems.AddRange(new ToolStripItem[] {
                matrixGreen, amberTerminal, oceanBlue, violetNeon });
            temaMenu.Name = "temaMenu";
            temaMenu.Text = "Tema";

            // matrixGreen
            matrixGreen.Name = "matrixGreen";
            matrixGreen.Text = "Matrix Green";
            matrixGreen.Click += MatrixGreen_Click;

            // amberTerminal
            amberTerminal.Name = "amberTerminal";
            amberTerminal.Text = "Amber Terminal";
            amberTerminal.Click += AmberTerminal_Click;

            // oceanBlue
            oceanBlue.Name = "oceanBlue";
            oceanBlue.Text = "Ocean Blue";
            oceanBlue.Click += OceanBlue_Click;

            // violetNeon
            violetNeon.Name = "violetNeon";
            violetNeon.Text = "Violet Neon";
            violetNeon.Click += VioletNeon_Click;

            // hakkindaMenu
            hakkindaMenu.Name = "hakkindaMenu";
            hakkindaMenu.Text = "Hakkında";
            hakkindaMenu.Click += HakkindaMenu_Click;

            // statusStrip
            statusStrip.Items.AddRange(new ToolStripItem[] {
                lblRam, lblCpu, lblThreads, lblNetIn, lblNetOut, lblSpacer, lblUptime });
            statusStrip.Dock = DockStyle.Bottom;
            statusStrip.Name = "statusStrip";
            statusStrip.SizingGrip = false;

            // lblRam
            lblRam.Name = "lblRam";
            lblRam.Text = "RAM: —";
            lblRam.BorderSides = ToolStripStatusLabelBorderSides.Right;

            // lblCpu
            lblCpu.Name = "lblCpu";
            lblCpu.Text = "CPU: —";
            lblCpu.BorderSides = ToolStripStatusLabelBorderSides.Right;

            // lblThreads
            lblThreads.Name = "lblThreads";
            lblThreads.Text = "Threads: —";
            lblThreads.BorderSides = ToolStripStatusLabelBorderSides.Right;

            // lblNetIn
            lblNetIn.Name = "lblNetIn";
            lblNetIn.Text = "↓ In: —";
            lblNetIn.BorderSides = ToolStripStatusLabelBorderSides.Right;

            // lblNetOut
            lblNetOut.Name = "lblNetOut";
            lblNetOut.Text = "↑ Out: —";
            lblNetOut.BorderSides = ToolStripStatusLabelBorderSides.Right;

            // lblSpacer
            lblSpacer.Name = "lblSpacer";
            lblSpacer.Spring = true;
            lblSpacer.Text = "";

            // lblUptime
            lblUptime.Name = "lblUptime";
            lblUptime.Text = "Uptime: 00:00:00";

            // monitorTimer
            monitorTimer.Interval = 1000;
            monitorTimer.Tick += MonitorTimer_Tick;

            // MDIMainForm
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 700);
            Controls.Add(statusStrip);
            Controls.Add(menuStrip);
            Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            IsMdiContainer = true;
            MainMenuStrip = menuStrip;
            Name = "MDIMainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "VortexTrade";
            WindowState = FormWindowState.Maximized;

            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem temaMenu;
        private ToolStripMenuItem matrixGreen;
        private ToolStripMenuItem amberTerminal;
        private ToolStripMenuItem oceanBlue;
        private ToolStripMenuItem violetNeon;
        private ToolStripMenuItem hakkindaMenu;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblRam;
        private ToolStripStatusLabel lblCpu;
        private ToolStripStatusLabel lblThreads;
        private ToolStripStatusLabel lblNetIn;
        private ToolStripStatusLabel lblNetOut;
        private ToolStripStatusLabel lblSpacer;
        private ToolStripStatusLabel lblUptime;
        private System.Windows.Forms.Timer monitorTimer;
    }
}
