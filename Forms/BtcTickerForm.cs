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
        private readonly Button _btnRefresh;
        private readonly ComboBox _cmbFilter;
        private readonly TextBox _txtSearch;
        private readonly System.Windows.Forms.Timer _autoRefreshTimer;

        private IMarketDataService? _marketService;
        private IReadOnlyList<TickerInfo> _allTickers = [];
        private bool _isLoading;

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
            _btnRefresh.Click += async (_, _) => await LoadTickersAsync();
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
                Size = new Size(910, 520),
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

            _listView.Columns.Add("Borsa", 140);
            _listView.Columns.Add("Çift", 110);
            _listView.Columns.Add("Fiyat", 130, HorizontalAlignment.Right);
            _listView.Columns.Add("USD Fiyat", 130, HorizontalAlignment.Right);
            _listView.Columns.Add("Hacim (BTC)", 120, HorizontalAlignment.Right);
            _listView.Columns.Add("Hacim (USD)", 140, HorizontalAlignment.Right);
            _listView.Columns.Add("Son İşlem", 140);

            Controls.Add(_listView);

            y = _listView.Bottom + 5;
            _lblLastUpdate = new Label
            {
                Text = "Son güncelleme: —  |  Veri: CoinLore API",
                Location = new Point(15, y),
                ForeColor = Color.FromArgb(120, fg.R, fg.G, fg.B),
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            Controls.Add(_lblLastUpdate);

            // ── Auto-refresh timer (10 sec) ──
            _autoRefreshTimer = new System.Windows.Forms.Timer { Interval = 10_000 };
            _autoRefreshTimer.Tick += async (_, _) => await LoadTickersAsync();

            Load += async (_, _) =>
            {
                _marketService = new CoinLoreMarketDataService();
                await LoadTickersAsync();
                _autoRefreshTimer.Start();
            };

            ResumeLayout(false);
        }

        private async Task LoadTickersAsync()
        {
            if (_isLoading || _marketService is null) return;
            _isLoading = true;

            _lblStatus.Text = "⏳ Yükleniyor...";
            _lblStatus.ForeColor = _accent;
            _btnRefresh.Enabled = false;

            try
            {
                _allTickers = await _marketService.GetBtcTickersAsync();
                ApplyFilter();

                _lblLastUpdate.Text =
                    $"Son güncelleme: {DateTime.Now:HH:mm:ss} | {_allTickers.Count} BTC çifti | CoinLore";
                _lblStatus.Text = "✓ OK";
                _lblStatus.ForeColor = Color.FromArgb(0, 220, 60);
            }
            catch (HttpRequestException ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                _lblStatus.Text = $"✗ Ağ: {Truncate(detail, 60)}";
                _lblStatus.ForeColor = Color.FromArgb(220, 50, 50);
                System.Diagnostics.Debug.WriteLine($"Ticker HTTP error: {ex}");
            }
            catch (TaskCanceledException)
            {
                _lblStatus.Text = "✗ Zaman aşımı (30sn)";
                _lblStatus.ForeColor = Color.FromArgb(220, 50, 50);
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"✗ {Truncate(ex.Message, 60)}";
                _lblStatus.ForeColor = Color.FromArgb(220, 50, 50);
                System.Diagnostics.Debug.WriteLine($"Ticker error: {ex}");
            }
            finally
            {
                _isLoading = false;
                _btnRefresh.Enabled = true;
            }
        }

        private static string Truncate(string text, int max) =>
            text.Length <= max ? text : string.Concat(text.AsSpan(0, max), "…");

        private void ApplyFilter()
        {
            var filtered = _allTickers.AsEnumerable();

            switch (_cmbFilter.SelectedIndex)
            {
                case 1: // USDT pairs
                    filtered = filtered.Where(t =>
                        t.Target.Equals("USDT", StringComparison.OrdinalIgnoreCase));
                    break;
                case 2: // USD pairs
                    filtered = filtered.Where(t =>
                        t.Target.Equals("USD", StringComparison.OrdinalIgnoreCase));
                    break;
                case 3: // EUR pairs
                    filtered = filtered.Where(t =>
                        t.Target.Equals("EUR", StringComparison.OrdinalIgnoreCase));
                    break;
            }

            var search = _txtSearch.Text.Trim();
            if (search.Length > 0)
            {
                filtered = filtered.Where(t =>
                    t.Exchange.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    t.Target.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            PopulateListView(filtered.ToList());
        }

        private void PopulateListView(IReadOnlyList<TickerInfo> tickers)
        {
            _listView.BeginUpdate();
            _listView.Items.Clear();

            var green = Color.FromArgb(0, 220, 60);

            foreach (var t in tickers)
            {
                var pair = $"{t.Base}/{t.Target}";
                var item = new ListViewItem(t.Exchange)
                {
                    UseItemStyleForSubItems = false,
                    ForeColor = _fg,
                    BackColor = _listView.BackColor
                };

                item.SubItems.Add(pair);
                item.SubItems.Add(FormatPrice(t.LastPrice));
                item.SubItems.Add($"${FormatPrice(t.UsdPrice)}");
                item.SubItems.Add(FormatVolume(t.Volume));
                item.SubItems.Add($"${FormatVolume(t.UsdVolume)}");
                item.SubItems.Add(t.LastTraded.ToString("HH:mm:ss dd/MM"));

                // USDT pairs green, others accent
                item.SubItems[1].ForeColor = t.Target.Equals("USDT", StringComparison.OrdinalIgnoreCase)
                    ? green : _accent;

                // USD price green
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
                _autoRefreshTimer.Stop();
                _autoRefreshTimer.Dispose();
                _marketService?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
