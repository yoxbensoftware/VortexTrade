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
                Font = new Font("Consolas", 24f, FontStyle.Bold),
                ForeColor = accent,
                AutoSize = true,
                Location = new Point(24, 24)
            };

            var lblVersion = new Label
            {
                Text = $"Versiyon: {AppConstants.AppFullVersion}",
                Font = new Font("Consolas", 10f),
                ForeColor = Color.FromArgb(210, fg.R, fg.G, fg.B),
                AutoSize = true,
                Location = new Point(24, lblTitle.Bottom + 10)
            };

            var lblDesc = new Label
            {
                Text = $"{AppConstants.Description}\n\n"
                     + $"{AppConstants.Copyright}\n"
                     + "Tüm hakları saklıdır.",
                Font = new Font("Consolas", 9f),
                ForeColor = Color.FromArgb(160, fg.R, fg.G, fg.B),
                AutoSize = true,
                MaximumSize = new Size(ClientSize.Width - 48, 0),
                Location = new Point(24, lblVersion.Bottom + 18)
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
                Location = new Point(ClientSize.Width - 124, ClientSize.Height - 48)
            };
            btnOk.FlatAppearance.BorderColor = accent;

            Controls.AddRange([lblTitle, lblVersion, lblDesc, btnOk]);
            AcceptButton = btnOk;
        }
    }
}
