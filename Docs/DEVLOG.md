# VortexTrade — Geliştirme Günlüğü

> Bu dosya 10 MB'ı aştığında `DEVLOG_archive_NNN.md` olarak arşivlenir ve yeni `DEVLOG.md` oluşturulur.

---

### V.0.0.1 — 2025-07-15

| Alan | Değer |
|---|---|
| **Tarih** | 2025-07-15 |
| **Geliştiren** | GitHub Copilot (AI) |
| **Bilgisayar** | PCDELL |

**Yapılan Geliştirme:**
- İlk proje oluşturma: MDI retro UI, 4 tema sistemi (Matrix Green, Amber Terminal, Ocean Blue, Violet Neon)
- Sistem izleme status bar (RAM, CPU, Threads, Network I/O, Uptime)
- Tema kalıcılığı (LocalAppData'da kayıt)
- Hakkında dialog penceresi
- Programatik stock chart ikonu (GDI+)

---

### V.0.0.2 — 2025-07-15

| Alan | Değer |
|---|---|
| **Tarih** | 2025-07-15 |
| **Geliştiren** | GitHub Copilot (AI) |
| **Bilgisayar** | PCDELL |

**Yapılan Geliştirme:**
- Klasör yapısı düzenlendi: Forms/, Constants/, Enums/, Models/, Helpers/, Properties/
- Versiyonlama sistemi eklendi (V.0.0.X commit başına, V.X.0.0 yeni sürüm)
- Admin manifest eklendi (requireAdministrator)
- AppConstants.cs oluşturuldu (merkezi sabit değerler)
- ThemeType, TradingEnums (ConnectionState, OrderSide, OrderType, OrderStatus, TimeFrame) eklendi
- DevLogEntry model ve DevLogHelper (10MB rotasyonlu) oluşturuldu
- AboutForm AppConstants kullanacak şekilde güncellendi
- DEVLOG.md geliştirme günlüğü sistemi kuruldu
- DEVELOPMENT_GUIDELINES.md genişletildi

---

### V.0.0.3 — 2025-07-15

| Alan | Değer |
|---|---|
| **Tarih** | 2025-07-15 |
| **Geliştiren** | GitHub Copilot (AI) |
| **Bilgisayar** | PCDELL |

**Yapılan Geliştirme:**
- `Docs/` klasörü oluşturuldu, tüm MD dosyaları bu klasöre taşındı
- `manifest.md` oluşturuldu — Trading Workbench sistem manifestosu (13 temel modül tanımı)
- `DEVELOPMENT_GUIDELINES.md` → `Docs/ai_development_guidelines.md` olarak yeniden adlandırıldı
- `DEVLOG.md` → `Docs/DEVLOG.md` olarak taşındı
- Versiyon V.0.0.3'e yükseltildi (AppConstants.cs + .csproj)
