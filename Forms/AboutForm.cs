namespace VortexTrade
{
    public class AboutForm : Form
    {
        public AboutForm(Color bg, Color fg, Color accent)
        {
            Text = $"Hakkında — {AppConstants.AppName}";
            ClientSize = new Size(420, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = bg;
            ForeColor = fg;
            Font = new Font("Consolas", 9f);

            var lblTitle = new Label
            {
                Text = AppConstants.AppName,
                Font = new Font("Consolas", 26f, FontStyle.Bold),
                ForeColor = accent,
                AutoSize = true,
                Location = new Point(24, 24)
            };

            var lblVersion = new Label
            {
                Text = $"Versiyon: {AppConstants.AppFullVersion}",
                Font = new Font("Consolas", 10f),
                ForeColor = fg,
                AutoSize = true,
                Location = new Point(26, 68)
            };

            var lblDesc = new Label
            {
                Text = $"{AppConstants.Description}\n\n"
                     + $"{AppConstants.Copyright}\n"
                     + "Tüm hakları saklıdır.",
                Font = new Font("Consolas", 9f),
                ForeColor = Color.FromArgb(160, fg.R, fg.G, fg.B),
                AutoSize = true,
                Location = new Point(26, 100)
            };

            var btnOk = new Button
            {
                Text = "Tamam",
                DialogResult = DialogResult.OK,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, accent.R, accent.G, accent.B),
                ForeColor = accent,
                Font = new Font("Consolas", 9f),
                Size = new Size(100, 32),
                Location = new Point(300, 252)
            };
            btnOk.FlatAppearance.BorderColor = accent;

            Controls.AddRange([lblTitle, lblVersion, lblDesc, btnOk]);
            AcceptButton = btnOk;
        }
    }
}
