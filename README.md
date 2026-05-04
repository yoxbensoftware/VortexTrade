# VortexTrade

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/Windows-Forms-0078D4?style=for-the-badge&logo=windows&logoColor=white" />
  <img src="https://img.shields.io/badge/Platform-Windows-0078D4?style=for-the-badge&logo=windows&logoColor=white" />
  <img src="https://img.shields.io/badge/Version-V.0.0.16-brightgreen?style=for-the-badge" />
  <img src="https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge" />
</p>

> **A retro-themed, MDI-based cryptocurrency trading terminal.**  
> Real-time market data, SOLID architecture, Binance integration.

---

## 📸 Screenshots

| Matrix Green | Amber Terminal |
|---|---|
| ![Matrix](.github/assets/matrix.png) | ![Amber](.github/assets/amber.png) |

| Ocean Blue | Violet Neon |
|---|---|
| ![Ocean](.github/assets/ocean.png) | ![Violet](.github/assets/violet.png) |

> *Screenshots will be added.*

---

## ✨ Features

### 🎨 Themes
- **Matrix Green** — Classic hacker terminal
- **Amber Terminal** — Retro yellow-orange
- **Ocean Blue** — Deep blue
- **Violet Neon** — Purple neon

The selected theme is saved to `%LocalAppData%\VortexTrade\theme.txt` and restored on next launch.

### 📊 Live BTC Markets
- Lists BTC trading pairs across all exchanges
- **Auto-refresh every 10 seconds**
- Filter: All / USDT / USD / EUR pairs
- Search by exchange name or quote currency
- Sorted by volume (highest first)
- **Active data source is displayed on screen**

### 🔄 Smart Provider Fallback
| Priority | Provider | Status |
|---|---|---|
| 1 | **Binance** `api.binance.com/api/v3/ticker/24hr` | Blocked in Turkey — works via VPN |
| 2 | **CoinLore** `api.coinlore.net/api/coin/markets` | Always available, no API key required |

If Binance is unreachable, the system automatically switches to CoinLore. After 3 consecutive errors, provider failover is triggered automatically.

### 💹 Manual Trading (Binance)
- HMAC-SHA256 signed Binance REST API integration
- Connection test
- Instant orders (Market / Limit)
- Scheduled order support

### 🖥️ System Monitoring
Real-time status bar showing:
- RAM usage
- CPU usage
- Thread count
- Network I/O
- Uptime

---

## 🏗️ Architecture

```
VortexTrade/
├── Constants/              # AppConstants (version, app-wide constants)
├── Enums/                  # TradingEnums, ThemeType
├── Forms/                  # MDIMainForm, BtcTickerForm, ManualTradeForm, AboutForm
├── Helpers/                # DevLogHelper
├── Models/                 # MarketTicker, TickerInfo, OrderRequest/Result, etc.
├── Services/
│   ├── Interfaces/         # IMarketDataProvider, IMarketDataStream, IExchange, etc.
│   ├── Exchanges/          # BinanceExchange (HMAC-SHA256)
│   └── MarketData/         # MarketDataStream, BinanceMarketDataProvider, CoinLoreMarketDataProvider
├── Installer/              # InnoSetup script + build script
└── Docs/
    ├── MDs/                # Project docs, manifest, guidelines
    └── DevLogs/            # Date-based development logs
```

### SOLID Market Data Infrastructure

```
IMarketDataProvider          ← Single fetch responsibility
    ├── BinanceMarketDataProvider
    └── CoinLoreMarketDataProvider

IMarketDataStream            ← Continuous data stream (event-based)
    └── MarketDataStream     ← Polling engine, fallback, SynchronizationContext

BtcTickerForm                ← Subscribes to IMarketDataStream (observer)
AnalysisEngine (planned)     ← Subscribes to IMarketDataStream
StrategyEngine (planned)     ← Subscribes to IMarketDataStream
```

Adding a new exchange or data source requires only implementing `IMarketDataProvider` and adding it to the `MarketDataStream` provider list.

---

## 🚀 Getting Started

### Requirements
- Windows 10/11 (64-bit)
- .NET 10 Runtime or later

### Build from Source

```powershell
git clone https://github.com/yoxbensoftware/VortexTrade.git
cd VortexTrade
dotnet build
dotnet run
```

### Windows Installer (InnoSetup)

```powershell
cd Installer
.\build-installer.ps1
```

> Requires InnoSetup 6. The script produces a self-contained single-file executable under `publish/` and generates `Output/VortexTradeSetup.exe`.

---

## ⚙️ Binance API Setup (for Manual Trading)

1. Go to **API Management** in your [Binance](https://www.binance.com) account
2. Create a new API key and enable **Spot Trading** permission
3. Restrict access to trusted IPs for security
4. Enter your API Key and Secret in VortexTrade → Manual Trading screen

> ⚠️ **Note:** Direct access to the Binance API from Turkey is geo-blocked due to local financial regulations. The market data screen automatically falls back to CoinLore API in this case.

---

## 🔧 Development

### Adding a New Provider

```csharp
public sealed class MyExchangeProvider : IMarketDataProvider
{
    public string ProviderName => "MyExchange";

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        // Ping or lightweight endpoint check
    }

    public async Task<IReadOnlyList<MarketTicker>> GetTickersAsync(
        string coinId, CancellationToken ct = default)
    {
        // Fetch data and return a list of MarketTicker
    }

    public void Dispose() { /* dispose HttpClient */ }
}
```

Then in any consumer (form, analysis engine, strategy):

```csharp
var stream = new MarketDataStream([
    new BinanceMarketDataProvider(),
    new MyExchangeProvider(),        // Fallback if Binance fails
    new CoinLoreMarketDataProvider()
]);

stream.DataReceived  += (_, e) => Console.WriteLine($"{e.Tickers.Count} tickers received ({e.ProviderName})");
stream.ProviderChanged += (_, name) => Console.WriteLine($"Active provider: {name}");

await stream.StartAsync("BTC", TimeSpan.FromSeconds(10));
```

### Versioning Policy
| Version | Description |
|---|---|
| `V.0.0.X` | Each commit is a patch |
| `V.0.X.0` | New feature / module |
| `V.X.0.0` | Major release |

---

## 📋 Development Log

All changes are tracked in date-based markdown files under `Docs/DevLogs/`:

```
Docs/DevLogs/
├── devlog.md               ← Main index
├── dev_log_12042026.md     ← V.0.0.1 – V.0.0.13
└── dev_log_13042026.md     ← V.0.0.14 – V.0.0.16
```

---

## 📄 License

MIT License — © 2026 [YoxbenSoftware](https://github.com/yoxbensoftware)
