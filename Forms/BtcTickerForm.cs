using System.Globalization;

namespace VortexTrade
{
    public sealed class BtcTickerForm : Form
    {
        private readonly Color _bg;
        private readonly Color _fg;
        private readonly Color _accent;

        private readonly ListView _listView;
        private readonly Label _lblStatus;
        private readonly Label _lblLastUpdate;
        private readonly Label _lblProvider;
        private readonly Button _btnRefresh;
        private readonly ComboBox _cmbFilter;
        private readonly TextBox _txtSearch;

        private IMarketDataStream? _stream;
        private IReadOnlyList<MarketTicker> _allTickers = [];

        public BtcTickerForm(Color bg, Color fg, Color accent)
        {
            _bg = bg;
            _fg = fg;
            _accent = accent;

            SuspendLayout();

            Text = "Anlık BTC Piyasaları";
            ClientSize = new Size(940, 620);
            BackColor = bg;
            ForeColor = fg;
            Font = new Font("Consolas", 9f);
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(800, 500);
            StartPosition = FormStartPosition.CenterParent;

            int y = 8;

            // ── Header ──
            var lblTitle = new Label
            {
                Text = "══════  CANLI BTC PİYASALARI  ══════",
                Location = new Point(15, y),
                ForeColor = accent,
                Font = new Font("Consolas", 11f, FontStyle.Bold),
                AutoSize = true
            };
            Controls.Add(lblTitle);
            y += 30;

            // ── Filter bar ──
            Controls.Add(CreateLabel("Filtre:", 15, y + 3));
            _cmbFilter = new ComboBox
            {
                Location = new Point(75, y),
                Size = new Size(130, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Lighten(bg, 20),
                ForeColor = fg,
                FlatStyle = FlatStyle.Flat
            };
            _cmbFilter.Items.AddRange(["Tümü", "USDT Çiftleri", "USD Çiftleri", "EUR Çiftleri"]);
            _cmbFilter.SelectedIndex = 0;
            _cmbFilter.SelectedIndexChanged += (_, _) => ApplyFilter();
            Controls.Add(_cmbFilter);

            Controls.Add(CreateLabel("Ara:", 220, y + 3));
            _txtSearch = new TextBox
            {
                Location = new Point(255, y),
                Size = new Size(150, 23),
                BackColor = Lighten(bg, 20),
                ForeColor = fg,
                BorderStyle = BorderStyle.FixedSingle,
                CharacterCasing = CharacterCasing.Upper
            };
            _txtSearch.TextChanged += (_, _) => ApplyFilter();
            Controls.Add(_txtSearch);

            _btnRefresh = CreateButton("⟳ Yenile", 420, y, 100);
            _btnRefresh.Click += async (_, _) => await RestartStreamAsync();
            Controls.Add(_btnRefresh);

            _lblStatus = new Label
            {
                Text = "",
                Location = new Point(535, y + 3),
                ForeColor = Color.FromArgb(150, fg.R, fg.G, fg.B),
                AutoSize = true
            };
            Controls.Add(_lblStatus);
            y += 34;

            // ── ListView ──
            _listView = new ListView
            {
                Location = new Point(15, y),
                Size = new Size(910, 510),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Lighten(bg, 8),
                ForeColor = fg,
                Font = new Font("Consolas", 9f),
                BorderStyle = BorderStyle.FixedSingle,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _listView.Columns.Add("Borsa", 130);
            _listView.Columns.Add("Çift", 100);
            _listView.Columns.Add("Fiyat", 130, HorizontalAlignment.Right);
            _listView.Columns.Add("USD Fiyat", 130, HorizontalAlignment.Right);
            _listView.Columns.Add("Hacim (BTC)", 120, HorizontalAlignment.Right);
            _listView.Columns.Add("Hacim (USD)", 140, HorizontalAlignment.Right);
            _listView.Columns.Add("Son İşlem", 130);

            Controls.Add(_listView);

            y = _listView.Bottom + 5;
            _lblLastUpdate = new Label
            {
                Text = "Son güncelleme: —",
                Location = new Point(15, y),
                ForeColor = Color.FromArgb(120, fg.R, fg.G, fg.B),
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            Controls.Add(_lblLastUpdate);

            _lblProvider = new Label
            {
                Text = "Kaynak: bekleniyor...",
                Location = new Point(400, y),
                ForeColor = Color.FromArgb(120, fg.R, fg.G, fg.B),
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            Controls.Add(_lblProvider);

            Load += async (_, _) => await StartStreamAsync();

            ResumeLayout(false);
        }

        private async Task StartStreamAsync()
        {
            _lblStatus.Text = "⏳ Bağlanıyor...";
            _lblStatus.ForeColor = _accent;

            _stream = new MarketDataStream(
            [
                new BinanceMarketDataProvider(),
                new CoinLoreMarketDataProvider()
            ]);

            _stream.DataReceived += OnDataReceived;
            _stream.ErrorOccurred += OnErrorOccurred;
            _stream.ProviderChanged += OnProviderChanged;

            await _stream.StartAsync("BTC", TimeSpan.FromSeconds(10));
        }

        private async Task RestartStreamAsync()
        {
            _btnRefresh.Enabled = false;
            _stream?.Stop();
            _stream?.Dispose();
            _stream = null;
            await StartStreamAsync();
            _btnRefresh.Enabled = true;
        }

        private void OnDataReceived(object? sender, MarketDataEventArgs e)
        {
            _allTickers = e.Tickers;
            ApplyFilter();

            _lblLastUpdate.Text =
                $"Son güncelleme: {DateTime.Now:HH:mm:ss} | {_allTickers.Count} çift";
            _lblStatus.Text = "✓ CANLI";
            _lblStatus.ForeColor = Color.FromArgb(0, 220, 60);
        }

        private void OnErrorOccurred(object? sender, MarketDataErrorEventArgs e)
        {
            var detail = e.Exception is HttpRequestException httpEx
                ? httpEx.InnerException?.Message ?? httpEx.Message
                : e.Exception.Message;

            _lblStatus.Text = $"✗ {e.ProviderName}: {Truncate(detail, 50)}";
            _lblStatus.ForeColor = Color.FromArgb(220, 50, 50);
            System.Diagnostics.Debug.WriteLine($"[BtcTickerForm] {e.ProviderName} hata: {e.Exception}");
        }

        private void OnProviderChanged(object? sender, string providerName)
        {
            _lblProvider.Text = $"Kaynak: {providerName}";
            _lblProvider.ForeColor = _accent;
            System.Diagnostics.Debug.WriteLine($"[BtcTickerForm] Provider değişti: {providerName}");
        }

        private static string Truncate(string text, int max) =>
            text.Length <= max ? text : string.Concat(text.AsSpan(0, max), "…");

        private void ApplyFilter()
        {
            var filtered = _allTickers.AsEnumerable();

            switch (_cmbFilter.SelectedIndex)
            {
                case 1:
                    filtered = filtered.Where(t =>
                        t.QuoteCurrency.Equals("USDT", StringComparison.OrdinalIgnoreCase));
                    break;
                case 2:
                    filtered = filtered.Where(t =>
                        t.QuoteCurrency.Equals("USD", StringComparison.OrdinalIgnoreCase));
                    break;
                case 3:
                    filtered = filtered.Where(t =>
                        t.QuoteCurrency.Equals("EUR", StringComparison.OrdinalIgnoreCase));
                    break;
            }

            var search = _txtSearch.Text.Trim();
            if (search.Length > 0)
            {
                filtered = filtered.Where(t =>
                    t.Exchange.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    t.QuoteCurrency.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            PopulateListView(filtered.ToList());
        }

        private void PopulateListView(IReadOnlyList<MarketTicker> tickers)
        {
            _listView.BeginUpdate();
            _listView.Items.Clear();

            var green = Color.FromArgb(0, 220, 60);

            foreach (var t in tickers)
            {
                var pair = $"{t.BaseCurrency}/{t.QuoteCurrency}";
                var item = new ListViewItem(t.Exchange)
                {
                    UseItemStyleForSubItems = false,
                    ForeColor = _fg,
                    BackColor = _listView.BackColor
                };

                item.SubItems.Add(pair);
                item.SubItems.Add(FormatPrice(t.Price));
                item.SubItems.Add($"${FormatPrice(t.PriceUsd)}");
                item.SubItems.Add(FormatVolume(t.Volume));
                item.SubItems.Add($"${FormatVolume(t.VolumeUsd)}");
                item.SubItems.Add(t.LastTraded.ToString("HH:mm:ss dd/MM"));

                item.SubItems[1].ForeColor = t.QuoteCurrency.Equals("USDT", StringComparison.OrdinalIgnoreCase)
                    ? green : _accent;

                item.SubItems[3].ForeColor = green;

                _listView.Items.Add(item);
            }

            _listView.EndUpdate();
        }

        private static string FormatPrice(decimal price)
        {
            return price switch
            {
                >= 1000m => price.ToString("N2", CultureInfo.InvariantCulture),
                >= 1m => price.ToString("N4", CultureInfo.InvariantCulture),
                >= 0.01m => price.ToString("N6", CultureInfo.InvariantCulture),
                _ => price.ToString("N8", CultureInfo.InvariantCulture)
            };
        }

        private static string FormatVolume(decimal volume)
        {
            return volume switch
            {
                >= 1_000_000_000m => $"{volume / 1_000_000_000m:N2}B",
                >= 1_000_000m => $"{volume / 1_000_000m:N2}M",
                >= 1_000m => $"{volume / 1_000m:N2}K",
                _ => volume.ToString("N2", CultureInfo.InvariantCulture)
            };
        }

        #region UI Helpers

        private Label CreateLabel(string text, int x, int y) => new()
        {
            Text = text,
            Location = new Point(x, y),
            ForeColor = _fg,
            AutoSize = true
        };

        private Button CreateButton(string text, int x, int y, int width)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 26),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, _accent.R, _accent.G, _accent.B),
                ForeColor = _accent,
                Font = new Font("Consolas", 9f),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = _accent;
            return btn;
        }

        private static Color Lighten(Color c, int amount) =>
            Color.FromArgb(
                Math.Min(c.R + amount, 255),
                Math.Min(c.G + amount, 255),
                Math.Min(c.B + amount, 255));

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream?.Stop();
                _stream?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
