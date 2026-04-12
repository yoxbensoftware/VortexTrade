using System.Globalization;

namespace VortexTrade
{
    public sealed class ManualTradeForm : Form
    {
        private readonly Color _bg;
        private readonly Color _fg;
        private readonly Color _accent;

        private readonly TextBox _txtApiKey;
        private readonly TextBox _txtApiSecret;
        private readonly Button _btnTestConnection;

        private readonly TextBox _txtSymbol;
        private readonly RadioButton _rbBuy;
        private readonly RadioButton _rbSell;
        private readonly ComboBox _cmbOrderType;
        private readonly TextBox _txtQuantity;
        private readonly TextBox _txtPrice;
        private readonly Label _lblPriceHint;
        private readonly TextBox _txtStopPrice;
        private readonly Label _lblStopPriceHint;

        private readonly CheckBox _chkScheduled;
        private readonly DateTimePicker _dtpScheduledTime;

        private readonly Button _btnExecute;
        private readonly TextBox _txtLog;

        private IExchange? _exchange;
        private IOrderService? _orderService;
        private ITradeScheduler? _scheduler;

        public ManualTradeForm(Color bg, Color fg, Color accent)
        {
            _bg = bg;
            _fg = fg;
            _accent = accent;

            SuspendLayout();

            Text = "Manuel Alım/Satım";
            ClientSize = new Size(520, 730);
            BackColor = bg;
            ForeColor = fg;
            Font = new Font("Consolas", 9f);
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(540, 750);
            StartPosition = FormStartPosition.CenterParent;

            const int lx = 15;
            const int cx = 140;
            const int cw = 355;
            int y = 10;

            // ── Bağlantı ──
            Controls.Add(CreateSectionLabel("── Bağlantı ──", lx, y));
            y += 26;

            Controls.Add(CreateLabel("API Key:", lx, y + 3));
            _txtApiKey = CreateTextBox(cx, y, cw);
            _txtApiKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(_txtApiKey);
            y += 30;

            Controls.Add(CreateLabel("API Secret:", lx, y + 3));
            _txtApiSecret = CreateTextBox(cx, y, cw);
            _txtApiSecret.UseSystemPasswordChar = true;
            _txtApiSecret.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(_txtApiSecret);
            y += 30;

            _btnTestConnection = CreateButton("⚡ Bağlantıyı Test Et", cx, y, 200);
            _btnTestConnection.Click += BtnTestConnection_Click;
            Controls.Add(_btnTestConnection);
            y += 42;

            // ── Emir Detayları ──
            Controls.Add(CreateSectionLabel("── Emir Detayları ──", lx, y));
            y += 26;

            Controls.Add(CreateLabel("Sembol:", lx, y + 3));
            _txtSymbol = CreateTextBox(cx, y, 150);
            _txtSymbol.Text = "BTCUSDT";
            _txtSymbol.CharacterCasing = CharacterCasing.Upper;
            Controls.Add(_txtSymbol);
            y += 30;

            Controls.Add(CreateLabel("Yön:", lx, y + 3));
            _rbBuy = new RadioButton
            {
                Text = "ALIM",
                Location = new Point(cx, y),
                ForeColor = Color.FromArgb(0, 220, 60),
                Font = new Font("Consolas", 9f, FontStyle.Bold),
                AutoSize = true,
                Checked = true,
                FlatStyle = FlatStyle.Flat
            };
            _rbSell = new RadioButton
            {
                Text = "SATIM",
                Location = new Point(cx + 100, y),
                ForeColor = Color.FromArgb(220, 50, 50),
                Font = new Font("Consolas", 9f, FontStyle.Bold),
                AutoSize = true,
                FlatStyle = FlatStyle.Flat
            };
            Controls.Add(_rbBuy);
            Controls.Add(_rbSell);
            y += 28;

            Controls.Add(CreateLabel("Emir Tipi:", lx, y + 3));
            _cmbOrderType = new ComboBox
            {
                Location = new Point(cx, y),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(
                    Math.Min(bg.R + 20, 255),
                    Math.Min(bg.G + 20, 255),
                    Math.Min(bg.B + 20, 255)),
                ForeColor = fg,
                FlatStyle = FlatStyle.Flat
            };
            _cmbOrderType.Items.AddRange(new object[] { "Market", "Limit", "Stop", "Stop Limit" });
            _cmbOrderType.SelectedIndex = 0;
            _cmbOrderType.SelectedIndexChanged += CmbOrderType_SelectedIndexChanged;
            Controls.Add(_cmbOrderType);
            y += 30;

            Controls.Add(CreateLabel("Miktar:", lx, y + 3));
            _txtQuantity = CreateTextBox(cx, y, 150);
            Controls.Add(_txtQuantity);
            y += 30;

            Controls.Add(CreateLabel("Fiyat:", lx, y + 3));
            _txtPrice = CreateTextBox(cx, y, 150);
            _txtPrice.Enabled = false;
            _lblPriceHint = CreateLabel("(Limit emirler için)", cx + 160, y + 3);
            _lblPriceHint.ForeColor = Color.FromArgb(100, fg.R, fg.G, fg.B);
            Controls.Add(_txtPrice);
            Controls.Add(_lblPriceHint);
            y += 30;

            Controls.Add(CreateLabel("Stop Fiyatı:", lx, y + 3));
            _txtStopPrice = CreateTextBox(cx, y, 150);
            _txtStopPrice.Enabled = false;
            _lblStopPriceHint = CreateLabel("(Stop emirleri için)", cx + 160, y + 3);
            _lblStopPriceHint.ForeColor = Color.FromArgb(100, fg.R, fg.G, fg.B);
            Controls.Add(_txtStopPrice);
            Controls.Add(_lblStopPriceHint);
            y += 38;

            // ── Zamanlama ──
            Controls.Add(CreateSectionLabel("── Zamanlama ──", lx, y));
            y += 26;

            _chkScheduled = new CheckBox
            {
                Text = "Zamanlanmış Emir",
                Location = new Point(lx, y),
                ForeColor = fg,
                AutoSize = true,
                FlatStyle = FlatStyle.Flat
            };
            _chkScheduled.CheckedChanged += ChkScheduled_CheckedChanged;
            Controls.Add(_chkScheduled);
            y += 26;

            Controls.Add(CreateLabel("Tarih/Saat:", lx, y + 3));
            _dtpScheduledTime = new DateTimePicker
            {
                Location = new Point(cx, y),
                Size = new Size(220, 23),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm:ss",
                Value = DateTime.Now.AddMinutes(5),
                Enabled = false
            };
            Controls.Add(_dtpScheduledTime);
            y += 40;

            // ── Execute ──
            _btnExecute = new Button
            {
                Text = "═══  EMRİ GÖNDER  ═══",
                Location = new Point(lx, y),
                Size = new Size(cw + cx - lx, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, accent.R, accent.G, accent.B),
                ForeColor = accent,
                Font = new Font("Consolas", 11f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            _btnExecute.FlatAppearance.BorderColor = accent;
            _btnExecute.Click += BtnExecute_Click;
            Controls.Add(_btnExecute);
            y += 50;

            // ── İşlem Günlüğü ──
            Controls.Add(CreateSectionLabel("── İşlem Günlüğü ──", lx, y));
            y += 26;

            _txtLog = new TextBox
            {
                Location = new Point(lx, y),
                Size = new Size(cw + cx - lx, ClientSize.Height - y - 15),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(
                    Math.Min(bg.R + 8, 255),
                    Math.Min(bg.G + 8, 255),
                    Math.Min(bg.B + 8, 255)),
                ForeColor = fg,
                Font = new Font("Consolas", 8.5f),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                       | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(_txtLog);

            ResumeLayout(false);

            LogMessage("Manuel Alım/Satım modülü hazır.");
            LogMessage("Binance API bağlantısı için API Key ve Secret giriniz.");
        }

        #region Event Handlers

        private async void BtnTestConnection_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtApiKey.Text) ||
                string.IsNullOrWhiteSpace(_txtApiSecret.Text))
            {
                LogMessage("✗ API Key ve Secret girilmelidir.");
                return;
            }

            _btnTestConnection.Enabled = false;
            LogMessage("Bağlantı test ediliyor...");

            try
            {
                InitializeServices();
                var connected = await _exchange!.TestConnectionAsync();
                LogMessage(connected
                    ? "✓ Binance bağlantısı başarılı."
                    : "✗ Binance bağlantısı başarısız.");
            }
            catch (HttpRequestException ex)
            {
                LogMessage($"✗ Ağ hatası: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                LogMessage("✗ Bağlantı zaman aşımına uğradı.");
            }
            catch (Exception ex)
            {
                LogMessage($"✗ Bağlantı hatası: {ex.Message}");
            }
            finally
            {
                _btnTestConnection.Enabled = true;
            }
        }

        private async void BtnExecute_Click(object? sender, EventArgs e)
        {
            if (_exchange is null)
            {
                LogMessage("✗ Önce bağlantıyı test edin.");
                return;
            }

            var request = BuildOrderRequest();
            if (request is null) return;

            if (_chkScheduled.Checked)
            {
                ExecuteScheduledOrder(request);
                return;
            }

            await ExecuteImmediateOrderAsync(request);
        }

        private void CmbOrderType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var index = _cmbOrderType.SelectedIndex;
            var needsPrice = index is 1 or 3;
            var needsStopPrice = index is 2 or 3;

            _txtPrice.Enabled = needsPrice;
            _lblPriceHint.Text = needsPrice ? "(Fiyat giriniz)" : "(Limit emirler için)";
            _lblPriceHint.ForeColor = Color.FromArgb(100, _fg.R, _fg.G, _fg.B);

            _txtStopPrice.Enabled = needsStopPrice;
            _lblStopPriceHint.Text = needsStopPrice
                ? "(Stop fiyatı giriniz)"
                : "(Stop emirleri için)";
            _lblStopPriceHint.ForeColor = Color.FromArgb(100, _fg.R, _fg.G, _fg.B);
        }

        private void ChkScheduled_CheckedChanged(object? sender, EventArgs e)
        {
            _dtpScheduledTime.Enabled = _chkScheduled.Checked;
            _btnExecute.Text = _chkScheduled.Checked
                ? "═══  EMRİ ZAMANLA  ═══"
                : "═══  EMRİ GÖNDER  ═══";
        }

        #endregion

        #region Service Management

        private void InitializeServices()
        {
            _scheduler?.Dispose();
            _exchange?.Dispose();

            var credentials = new ExchangeCredentials(
                _txtApiKey.Text.Trim(),
                _txtApiSecret.Text.Trim(),
                ExchangeType.Binance);

            _exchange = new BinanceExchange(credentials);
            var validator = new OrderValidator();
            _orderService = new OrderService(_exchange, validator);
            _scheduler = new TradeScheduler(_orderService);
            _scheduler.OrderExecuted += OnScheduledOrderExecuted;
            _scheduler.OrderFailed += OnScheduledOrderFailed;
        }

        #endregion

        #region Order Execution

        private OrderRequest? BuildOrderRequest()
        {
            if (string.IsNullOrWhiteSpace(_txtSymbol.Text))
            {
                LogMessage("✗ Sembol girilmelidir.");
                return null;
            }

            if (!decimal.TryParse(_txtQuantity.Text, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var quantity) || quantity <= 0)
            {
                LogMessage("✗ Geçerli bir miktar girilmelidir.");
                return null;
            }

            var side = _rbBuy.Checked ? OrderSide.Buy : OrderSide.Sell;
            var orderType = _cmbOrderType.SelectedIndex switch
            {
                1 => OrderType.Limit,
                2 => OrderType.Stop,
                3 => OrderType.StopLimit,
                _ => OrderType.Market
            };

            decimal? price = null;
            if (orderType is OrderType.Limit or OrderType.StopLimit)
            {
                if (!decimal.TryParse(_txtPrice.Text, NumberStyles.Any,
                        CultureInfo.InvariantCulture, out var p) || p <= 0)
                {
                    LogMessage("✗ Limit emirleri için geçerli bir fiyat girilmelidir.");
                    return null;
                }
                price = p;
            }

            decimal? stopPrice = null;
            if (orderType is OrderType.Stop or OrderType.StopLimit)
            {
                if (!decimal.TryParse(_txtStopPrice.Text, NumberStyles.Any,
                        CultureInfo.InvariantCulture, out var sp) || sp <= 0)
                {
                    LogMessage("✗ Stop emirleri için geçerli bir stop fiyatı girilmelidir.");
                    return null;
                }
                stopPrice = sp;
            }

            return new OrderRequest(
                Symbol: _txtSymbol.Text.Trim().ToUpperInvariant(),
                Side: side,
                Type: orderType,
                Quantity: quantity,
                Price: price,
                StopPrice: stopPrice);
        }

        private async Task ExecuteImmediateOrderAsync(OrderRequest request)
        {
            _btnExecute.Enabled = false;
            var sideText = request.Side == OrderSide.Buy ? "ALIM" : "SATIM";
            var priceText = request.Type == OrderType.Market
                ? "Market"
                : request.Price?.ToString(CultureInfo.InvariantCulture) ?? "?";

            LogMessage($"▸ {request.Symbol} {sideText} {request.Quantity} @ {priceText}");

            try
            {
                var result = await _orderService!.ExecuteOrderAsync(request);
                LogOrderResult(result);
            }
            catch (Exception ex)
            {
                LogMessage($"✗ Hata: {ex.Message}");
            }
            finally
            {
                _btnExecute.Enabled = true;
            }
        }

        private void ExecuteScheduledOrder(OrderRequest request)
        {
            var scheduleTimeUtc = _dtpScheduledTime.Value.ToUniversalTime();
            if (scheduleTimeUtc <= DateTime.UtcNow)
            {
                LogMessage("✗ Zamanlama tarihi gelecekte olmalıdır.");
                return;
            }

            var id = _scheduler!.ScheduleOrder(request, scheduleTimeUtc);
            var sideText = request.Side == OrderSide.Buy ? "ALIM" : "SATIM";
            LogMessage($"⏰ Emir zamanlandı: {request.Symbol} {sideText} {request.Quantity}");
            LogMessage($"   Tarih: {_dtpScheduledTime.Value:dd/MM/yyyy HH:mm:ss} | ID: {id:N}");
        }

        private void LogOrderResult(OrderResult result)
        {
            if (result.Success)
            {
                LogMessage($"✓ Emir başarılı | ID: {result.OrderId}");
                LogMessage($"  Miktar: {result.ExecutedQuantity} | Fiyat: {result.ExecutedPrice}");
            }
            else
            {
                LogMessage($"✗ Emir başarısız: {result.ErrorMessage}");
            }
        }

        #endregion

        #region Scheduled Order Events

        private void OnScheduledOrderExecuted(object? sender, OrderResult result)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnScheduledOrderExecuted(sender, result));
                return;
            }

            var sideText = result.Side == OrderSide.Buy ? "ALIM" : "SATIM";
            LogMessage($"⏰ Zamanlanmış emir çalıştırıldı: {result.Symbol} {sideText}");
            LogOrderResult(result);
        }

        private void OnScheduledOrderFailed(object? sender, ScheduledOrderErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnScheduledOrderFailed(sender, e));
                return;
            }

            LogMessage($"⏰ ✗ Zamanlanmış emir hatası: {e.ErrorMessage}");
        }

        #endregion

        #region UI Helpers

        private void LogMessage(string message)
        {
            var time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            _txtLog.AppendText($"[{time}] {message}{Environment.NewLine}");
        }

        private Label CreateLabel(string text, int x, int y) => new()
        {
            Text = text,
            Location = new Point(x, y),
            ForeColor = _fg,
            AutoSize = true
        };

        private Label CreateSectionLabel(string text, int x, int y) => new()
        {
            Text = text,
            Location = new Point(x, y),
            ForeColor = _accent,
            Font = new Font("Consolas", 9.5f, FontStyle.Bold),
            AutoSize = true
        };

        private TextBox CreateTextBox(int x, int y, int width) => new()
        {
            Location = new Point(x, y),
            Size = new Size(width, 23),
            BackColor = Color.FromArgb(
                Math.Min(_bg.R + 20, 255),
                Math.Min(_bg.G + 20, 255),
                Math.Min(_bg.B + 20, 255)),
            ForeColor = _fg,
            BorderStyle = BorderStyle.FixedSingle
        };

        private Button CreateButton(string text, int x, int y, int width)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, _accent.R, _accent.G, _accent.B),
                ForeColor = _accent,
                Font = new Font("Consolas", 9f),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = _accent;
            return btn;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _scheduler?.Dispose();
                _exchange?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
