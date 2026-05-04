# VortexTrade

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/Windows-Forms-0078D4?style=for-the-badge&logo=windows&logoColor=white" />
  <img src="https://img.shields.io/badge/Platform-Windows-0078D4?style=for-the-badge&logo=windows&logoColor=white" />
  <img src="https://img.shields.io/badge/Version-V.0.0.16-brightgreen?style=for-the-badge" />
  <img src="https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge" />
</p>

> **Retro temalı, MDI tabanlı kripto para trading terminali.**  
> Gerçek zamanlı piyasa verisi, SOLID mimari, Binance entegrasyonu.

---

## 📸 Ekran Görüntüleri

| Matrix Green | Amber Terminal |
|---|---|
| ![Matrix](.github/assets/matrix.png) | ![Amber](.github/assets/amber.png) |

| Ocean Blue | Violet Neon |
|---|---|
| ![Ocean](.github/assets/ocean.png) | ![Violet](.github/assets/violet.png) |

> *Ekran görüntüleri eklenecek.*

---

## ✨ Özellikler

### 🎨 Temalar
- **Matrix Green** — Klasik hacker terminali
- **Amber Terminal** — Retro sarı-turuncu
- **Ocean Blue** — Derin mavi
- **Violet Neon** — Mor neon

Seçilen tema `%LocalAppData%\VortexTrade\theme.txt` dosyasına kaydedilir, uygulama yeniden başlatıldığında hatırlanır.

### 📊 Anlık BTC Piyasaları
- Tüm borsalardaki BTC işlem çiftlerini listeler
- **Otomatik 10 saniyede bir yenileme**
- Filtre: Tümü / USDT / USD / EUR çiftleri
- Borsa veya parite ile arama
- Hacme göre sıralama (en yüksek önce)
- **Aktif veri kaynağı ekranda gösterilir**

### 🔄 Akıllı Veri Kaynağı (Provider Fallback)
| Öncelik | Sağlayıcı | Durum |
|---|---|---|
| 1 | **Binance** `api.binance.com/api/v3/ticker/24hr` | Türkiye'den engelli — VPN ile çalışır |
| 2 | **CoinLore** `api.coinlore.net/api/coin/markets` | Her koşulda çalışır, API key yok |

Binance erişilemezse sistem otomatik olarak CoinLore'a geçer. 3 ardışık hatada provider değişimi otomatik tetiklenir.

### 💹 Manuel Alım/Satım (Binance)
- HMAC-SHA256 imzalı Binance REST API entegrasyonu
- Bağlantı testi
- Anlık emir (Market / Limit)
- Zamanlanmış emir desteği

### 🖥️ Sistem İzleme
Status bar'da gerçek zamanlı:
- RAM kullanımı
- CPU kullanımı
- Thread sayısı
- Network I/O
- Uptime

---

## 🏗️ Mimari

```
VortexTrade/
├── Constants/              # AppConstants (versiyon, sabitler)
├── Enums/                  # TradingEnums, ThemeType
├── Forms/                  # MDIMainForm, BtcTickerForm, ManualTradeForm, AboutForm
├── Helpers/                # DevLogHelper
├── Models/                 # MarketTicker, TickerInfo, OrderRequest/Result vb.
├── Services/
│   ├── Interfaces/         # IMarketDataProvider, IMarketDataStream, IExchange vb.
│   ├── Exchanges/          # BinanceExchange (HMAC-SHA256)
│   └── MarketData/         # MarketDataStream, BinanceMarketDataProvider, CoinLoreMarketDataProvider
├── Installer/              # InnoSetup betiği + build scripti
└── Docs/
    ├── MDs/                # Proje dökümanları, manifest, guidelines
    └── DevLogs/            # Tarih bazlı geliştirme günlükleri
```

### SOLID Piyasa Verisi Altyapısı

```
IMarketDataProvider          ← Tek seferlik veri çekme
    ├── BinanceMarketDataProvider
    └── CoinLoreMarketDataProvider

IMarketDataStream            ← Sürekli veri akışı (event-based)
    └── MarketDataStream     ← Polling engine, fallback, SynchronizationContext

BtcTickerForm                ← IMarketDataStream'e abone (observer)
AnalysisEngine (gelecek)     ← IMarketDataStream'e abone
StrategyEngine (gelecek)     ← IMarketDataStream'e abone
```

Yeni bir borsa veya veri kaynağı eklemek için tek yapılacak: `IMarketDataProvider` implemente et, `MarketDataStream` listesine ekle.

---

## 🚀 Kurulum

### Gereksinimler
- Windows 10/11 (64-bit)
- .NET 10 Runtime veya daha yenisi

### Kaynak Koddan Derleme

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

> InnoSetup 6 kurulu olmalıdır. Script `publish/` altında self-contained tek dosya üretir ve `Output/VortexTradeSetup.exe` oluşturur.

---

## ⚙️ Binance API Kurulumu (Manuel Alım/Satım için)

1. [Binance](https://www.binance.com) hesabında **API Management** bölümüne git
2. Yeni API key oluştur, **Spot Trading** iznini ver
3. Güvenlik için IP kısıtlaması önerilir
4. VortexTrade → Manuel Alım/Satım ekranına API Key ve Secret'ı gir

> ⚠️ **Not:** Türkiye'den Binance API'ye bağlantı BDDK/SPK düzenlemeleri kapsamında engellenmektedir. Piyasa verisi ekranı bu durumda otomatik olarak CoinLore API'ye geçer.

---

## 🔧 Geliştirme

### Yeni Provider Ekleme

```csharp
public sealed class MyExchangeProvider : IMarketDataProvider
{
    public string ProviderName => "MyExchange";

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        // Ping veya basit endpoint kontrolü
    }

    public async Task<IReadOnlyList<MarketTicker>> GetTickersAsync(
        string coinId, CancellationToken ct = default)
    {
        // Veri çek, MarketTicker listesi döndür
    }

    public void Dispose() { /* HttpClient dispose */ }
}
```

Sonra `BtcTickerForm` veya herhangi bir consumer'da:

```csharp
var stream = new MarketDataStream([
    new BinanceMarketDataProvider(),
    new MyExchangeProvider(),       // Binance başarısız olursa buraya geçer
    new CoinLoreMarketDataProvider()
]);

stream.DataReceived += (_, e) => Console.WriteLine($"{e.Tickers.Count} ticker alındı ({e.ProviderName})");
stream.ProviderChanged += (_, name) => Console.WriteLine($"Aktif provider: {name}");

await stream.StartAsync("BTC", TimeSpan.FromSeconds(10));
```

### Versiyon Politikası
| Versiyon | Açıklama |
|---|---|
| `V.0.0.X` | Her commit bir patch |
| `V.0.X.0` | Yeni özellik / modül |
| `V.X.0.0` | Major sürüm |

---

## 📋 Geliştirme Günlüğü

Tüm değişiklikler `Docs/DevLogs/` altında tarih bazlı markdown dosyalarında tutulur:

```
Docs/DevLogs/
├── devlog.md               ← Ana index
├── dev_log_12042026.md     ← V.0.0.1 – V.0.0.13
└── dev_log_13042026.md     ← V.0.0.14 – V.0.0.16
```

---

## 📄 Lisans

MIT License — © 2026 [YoxbenSoftware](https://github.com/yoxbensoftware)
