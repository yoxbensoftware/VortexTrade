using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection;

namespace VortexTrade
{
    public partial class MDIMainForm : Form
    {
        private record ThemeColors(
            Color Background, Color Foreground, Color Accent,
            Color MenuBg, Color StatusBg);

        private static readonly Dictionary<string, ThemeColors> Themes = new()
        {
            ["Matrix Green"] = new(
                Color.FromArgb(10, 10, 10),
                Color.FromArgb(0, 255, 65),
                Color.FromArgb(0, 200, 50),
                Color.FromArgb(18, 18, 18),
                Color.FromArgb(14, 14, 14)),
            ["Amber Terminal"] = new(
                Color.FromArgb(16, 12, 0),
                Color.FromArgb(255, 176, 0),
                Color.FromArgb(255, 140, 0),
                Color.FromArgb(24, 18, 0),
                Color.FromArgb(20, 15, 0)),
            ["Ocean Blue"] = new(
                Color.FromArgb(8, 8, 26),
                Color.FromArgb(0, 191, 255),
                Color.FromArgb(0, 119, 182),
                Color.FromArgb(14, 14, 34),
                Color.FromArgb(10, 10, 28)),
            ["Violet Neon"] = new(
                Color.FromArgb(16, 8, 26),
                Color.FromArgb(191, 64, 191),
                Color.FromArgb(155, 48, 255),
                Color.FromArgb(22, 14, 34),
                Color.FromArgb(18, 10, 28)),
        };

        private static string ThemeSettingsPath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "VortexTrade", "theme.txt");

        private ThemeColors _theme = Themes["Matrix Green"];
        private MdiClient? _mdiClient;
        private readonly Stopwatch _uptimeWatch = Stopwatch.StartNew();
        private readonly Process _currentProcess = Process.GetCurrentProcess();
        private DateTime _lastCpuCheck;
        private TimeSpan _lastCpuUsage;
        private long _lastBytesIn;
        private long _lastBytesOut;

        public MDIMainForm()
        {
            InitializeComponent();
            SetupThemeItemColors();
            Icon = CreateStockChartIcon();
            Load += MDIMainForm_Load;
        }

        private void SetupThemeItemColors()
        {
            matrixGreen.Tag = Themes["Matrix Green"].Foreground;
            amberTerminal.Tag = Themes["Amber Terminal"].Foreground;
            oceanBlue.Tag = Themes["Ocean Blue"].Foreground;
            violetNeon.Tag = Themes["Violet Neon"].Foreground;
        }

        private void MDIMainForm_Load(object? sender, EventArgs e)
        {
            SetupMdiClient();
            InitMonitoring();
            ApplyTheme(LoadThemeSetting());
            monitorTimer.Start();
        }

        #region Theme Persistence

        private static void SaveThemeSetting(string themeName)
        {
            try
            {
                var dir = Path.GetDirectoryName(ThemeSettingsPath)!;
                Directory.CreateDirectory(dir);
                File.WriteAllText(ThemeSettingsPath, themeName);
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"Theme setting could not be saved: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine($"Theme setting access denied: {ex.Message}");
            }
        }

        private static string LoadThemeSetting()
        {
            try
            {
                if (File.Exists(ThemeSettingsPath))
                {
                    var name = File.ReadAllText(ThemeSettingsPath).Trim();
                    if (Themes.ContainsKey(name)) return name;
                }
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"Theme setting could not be loaded: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine($"Theme setting access denied: {ex.Message}");
            }
            return "Matrix Green";
        }

        #endregion

        #region Stock Chart Icon

        private static Icon CreateStockChartIcon()
        {
            var bmp = new Bitmap(32, 32);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(10, 10, 10));

            using var gridPen = new Pen(Color.FromArgb(35, 0, 200, 50));
            for (int y = 6; y <= 26; y += 5)
                g.DrawLine(gridPen, 3, y, 29, y);

            var green = Color.FromArgb(0, 220, 60);
            var red = Color.FromArgb(220, 50, 50);
            int[] barX = [5, 10, 15, 20, 25];
            int[] barTop = [18, 14, 20, 10, 6];
            int[] barH = [6, 8, 5, 9, 10];
            bool[] bullish = [true, true, false, true, true];

            for (int i = 0; i < barX.Length; i++)
            {
                var c = bullish[i] ? green : red;
                using var brush = new SolidBrush(c);
                using var pen = new Pen(c);
                g.FillRectangle(brush, barX[i], barTop[i], 3, barH[i]);
                g.DrawLine(pen, barX[i] + 1, barTop[i] - 2, barX[i] + 1, barTop[i] + barH[i] + 2);
            }

            using var trendPen = new Pen(Color.FromArgb(180, 0, 255, 65), 1.5f);
            g.DrawLines(trendPen, new PointF[]
            {
                new(4, 22), new(10, 18), new(16, 23), new(22, 13), new(28, 8)
            });

            using var framePen = new Pen(Color.FromArgb(60, 0, 200, 50));
            g.DrawRectangle(framePen, 0, 0, 31, 31);

            return Icon.FromHandle(bmp.GetHicon());
        }

        #endregion

        private void SetupMdiClient()
        {
            foreach (Control c in Controls)
            {
                if (c is MdiClient mdi)
                {
                    _mdiClient = mdi;
                    EnableOptimizedPainting(_mdiClient);
                    _mdiClient.Paint += MdiClient_Paint;
                    _mdiClient.Resize += (_, _) => _mdiClient.Invalidate();
                    break;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields",
            Justification = "MdiClient does not expose a public API for enabling the required paint styles. This is limited to the framework Control.SetStyle method on the current MDI client instance.")]
        private static void EnableOptimizedPainting(MdiClient mdiClient)
        {
            var setStyle = typeof(Control).GetMethod(
                "SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);

            if (setStyle is null)
            {
                return;
            }

            setStyle.Invoke(mdiClient, new object[]
            {
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true
            });
        }

        private void MdiClient_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var r = _mdiClient!.ClientRectangle;

            using var bg = new SolidBrush(_theme.Background);
            g.FillRectangle(bg, r);

            using var scanPen = new Pen(Color.FromArgb(12, _theme.Foreground));
            for (int y = 0; y < r.Height; y += 3)
                g.DrawLine(scanPen, 0, y, r.Width, y);

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            using var titleFont = new Font("Consolas", 52f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(50, _theme.Foreground));
            var sz = g.MeasureString("VortexTrade", titleFont);
            float tx = (r.Width - sz.Width) / 2;
            float ty = (r.Height - sz.Height) / 2 - 20;

            using var glow = new SolidBrush(Color.FromArgb(15, _theme.Accent));
            for (int i = 4; i >= 1; i--)
            {
                g.DrawString("VortexTrade", titleFont, glow, tx - i, ty);
                g.DrawString("VortexTrade", titleFont, glow, tx + i, ty);
                g.DrawString("VortexTrade", titleFont, glow, tx, ty - i);
                g.DrawString("VortexTrade", titleFont, glow, tx, ty + i);
            }
            g.DrawString("VortexTrade", titleFont, titleBrush, tx, ty);

            using var subFont = new Font("Consolas", 11f);
            using var subBrush = new SolidBrush(Color.FromArgb(35, _theme.Foreground));
            const string sub = "[ AI B A S E D  T R A D I N G   T E R M I N A L ]";
            var ssz = g.MeasureString(sub, subFont);
            g.DrawString(sub, subFont, subBrush,
                (r.Width - ssz.Width) / 2, ty + sz.Height + 8);

            using var border = new Pen(Color.FromArgb(25, _theme.Accent));
            g.DrawRectangle(border, 8, 8, r.Width - 17, r.Height - 17);
        }

        private void InitMonitoring()
        {
            _lastCpuCheck = DateTime.UtcNow;
            _lastCpuUsage = _currentProcess.TotalProcessorTime;
            ReadNetworkCounters(out _lastBytesIn, out _lastBytesOut);
        }

        private static void ReadNetworkCounters(out long bytesIn, out long bytesOut)
        {
            bytesIn = 0;
            bytesOut = 0;
            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up &&
                        ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        var s = ni.GetIPStatistics();
                        bytesIn += s.BytesReceived;
                        bytesOut += s.BytesSent;
                    }
                }
            }
            catch (NetworkInformationException ex)
            {
                Debug.WriteLine($"Network counters could not be read: {ex.Message}");
            }
            catch (PlatformNotSupportedException ex)
            {
                Debug.WriteLine($"Network counters are not supported: {ex.Message}");
            }
        }

        private void MonitorTimer_Tick(object? sender, EventArgs e)
        {
            _currentProcess.Refresh();

            lblRam.Text = $"RAM: {_currentProcess.WorkingSet64 / (1024.0 * 1024.0):F1} MB";

            var now = DateTime.UtcNow;
            var cpuDelta = _currentProcess.TotalProcessorTime - _lastCpuUsage;
            var timeDelta = (now - _lastCpuCheck).TotalMilliseconds;
            var cpuPct = timeDelta > 0
                ? cpuDelta.TotalMilliseconds / (timeDelta * Environment.ProcessorCount) * 100.0
                : 0;
            _lastCpuCheck = now;
            _lastCpuUsage = _currentProcess.TotalProcessorTime;
            lblCpu.Text = $"CPU: {cpuPct:F1}%";

            lblThreads.Text = $"Threads: {_currentProcess.Threads.Count}";

            ReadNetworkCounters(out long curIn, out long curOut);
            lblNetIn.Text = $"↓ {FormatBytes(curIn - _lastBytesIn)}/s";
            lblNetOut.Text = $"↑ {FormatBytes(curOut - _lastBytesOut)}/s";
            _lastBytesIn = curIn;
            _lastBytesOut = curOut;

            var t = _uptimeWatch.Elapsed;
            lblUptime.Text = $"Uptime: {(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
        }

        private static string FormatBytes(long b)
        {
            if (b < 1024)
            {
                return $"{b} B";
            }

            if (b < 1048576)
            {
                return $"{b / 1024.0:F1} KB";
            }

            return $"{b / 1048576.0:F1} MB";
        }

        private void ApplyTheme(string name)
        {
            if (!Themes.TryGetValue(name, out var t)) return;
            _theme = t;

            foreach (ToolStripItem item in temaMenu.DropDownItems)
            {
                if (item is ToolStripMenuItem { Text: { Length: > 0 } menuText } mi &&
                    Themes.ContainsKey(menuText))
                {
                    mi.Checked = menuText == name;
                }
            }

            var renderer = new RetroRenderer(t);
            menuStrip.Renderer = renderer;
            menuStrip.BackColor = t.MenuBg;
            menuStrip.ForeColor = t.Foreground;

            statusStrip.Renderer = renderer;
            statusStrip.BackColor = t.StatusBg;
            statusStrip.ForeColor = t.Foreground;
            foreach (ToolStripItem lbl in statusStrip.Items)
                lbl.ForeColor = t.Foreground;

            BackColor = t.Background;
            ForeColor = t.Foreground;

            _mdiClient?.Invalidate();
            Text = $"VortexTrade — [{name}]";

            SaveThemeSetting(name);
        }

        private void MatrixGreen_Click(object? sender, EventArgs e) => ApplyTheme("Matrix Green");
        private void AmberTerminal_Click(object? sender, EventArgs e) => ApplyTheme("Amber Terminal");
        private void OceanBlue_Click(object? sender, EventArgs e) => ApplyTheme("Ocean Blue");
        private void VioletNeon_Click(object? sender, EventArgs e) => ApplyTheme("Violet Neon");

        private void HakkindaMenu_Click(object? sender, EventArgs e)
        {
            using var about = new AboutForm(_theme.Background, _theme.Foreground, _theme.Accent);
            about.ShowDialog(this);
        }

        #region Retro Renderer

        private sealed class RetroRenderer(ThemeColors theme)
            : ToolStripProfessionalRenderer(new RetroColorTable(theme))
        {
            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                if (e.Item.Tag is Color c)
                    e.TextColor = e.Item.Selected ? theme.Background : c;
                else
                    e.TextColor = e.Item.Selected ? theme.Background : theme.Foreground;
                base.OnRenderItemText(e);
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                var rect = e.Item.ContentRectangle;
                if (e.Item.Selected || e.Item.Pressed)
                {
                    using var b = new SolidBrush(theme.Accent);
                    e.Graphics.FillRectangle(b, rect);
                }
                else
                {
                    using var b = new SolidBrush(theme.MenuBg);
                    e.Graphics.FillRectangle(b, rect);
                }
            }
        }

        private sealed class RetroColorTable(ThemeColors t) : ProfessionalColorTable
        {
            public override Color MenuStripGradientBegin => t.MenuBg;
            public override Color MenuStripGradientEnd => t.MenuBg;
            public override Color MenuItemBorder => t.Accent;
            public override Color MenuBorder => t.Accent;
            public override Color MenuItemSelected => t.Accent;
            public override Color MenuItemSelectedGradientBegin =>
                Color.FromArgb(40, t.Accent.R, t.Accent.G, t.Accent.B);
            public override Color MenuItemSelectedGradientEnd =>
                Color.FromArgb(40, t.Accent.R, t.Accent.G, t.Accent.B);
            public override Color MenuItemPressedGradientBegin => t.Accent;
            public override Color MenuItemPressedGradientEnd => t.Accent;
            public override Color ImageMarginGradientBegin => t.MenuBg;
            public override Color ImageMarginGradientMiddle => t.MenuBg;
            public override Color ImageMarginGradientEnd => t.MenuBg;
            public override Color ToolStripDropDownBackground => t.MenuBg;
            public override Color SeparatorDark =>
                Color.FromArgb(50, t.Foreground.R, t.Foreground.G, t.Foreground.B);
            public override Color SeparatorLight => Color.Transparent;
            public override Color StatusStripGradientBegin => t.StatusBg;
            public override Color StatusStripGradientEnd => t.StatusBg;
        }

        #endregion
    }
}
