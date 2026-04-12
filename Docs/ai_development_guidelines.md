# VortexTrade — Geliştirme Kuralları

Bu dosya, Copilot ve geliştirici tarafından her zaman uyulması gereken temel kuralları içerir.

---

## 1. Derleme (Build) Kuralları

- Her değişiklikten sonra proje derlenmelidir.
- Derleme sonucunda **hiçbir Error veya Warning** olmamalıdır.
- Derleme başarısız ise, hata çözülmeden sonraki adıma geçilmemelidir.

## 2. UI / Arayüz Kuralları

- Tüm kontroller (buton, label vb.) her zaman **görünür alanda (client area)** kalmalıdır.
- Form boyutlandırmada `Size` yerine **`ClientSize`** kullanılmalıdır.
- Kontroller konumlandırılırken **alt ve sağ kenardan en az 16px boşluk** bırakılmalıdır.
- Yeni açılan pencere/dialog'larda tüm kontrollerin taşmadığı doğrulanmalıdır.

## 3. Kod Kalitesi

- Mevcut kod stili ve yapısı korunmalıdır (Consolas font, retro tema, Türkçe menüler).
- Gereksiz yorum satırlarından kaçınılmalı, yalnızca karmaşık mantık açıklanmalıdır.
- Kullanılmayan `using` ifadeleri ve değişkenler temizlenmelidir.

## 4. Tema ve Stil Tutarlılığı

- Yeni formlar ve kontroller mevcut tema sistemi (`ThemeColors`) ile uyumlu olmalıdır.
- Sabit renkler yerine tema renkleri (`bg`, `fg`, `accent`) kullanılmalıdır.

## 5. Versiyonlama Sistemi

- Her geliştirme/commit sonrası versiyon **V.0.0.X** formatında artırılır.
- Kullanıcı **"YENİ SÜRÜM OLUŞTUR"** dediğinde ana versiyon **V.X.0.0** olarak artırılır.
- Versiyon bilgisi aşağıdaki dosyalarda güncellenmelidir:
  - `Constants/AppConstants.cs`
  - `VortexTrade.csproj`
- Tüm versiyon değişiklikleri geliştirme günlüğüne kaydedilir.

## 6. Geliştirme Günlüğü (DevLogs)

- **Her geliştirme** sonunda `Docs/DevLogs/` klasöründeki aktif günlük dosyasına kayıt eklenmelidir.
- Dosya isimlendirme formatı: **`dev_log_DDMMYYYY.md`** (örn: `dev_log_12042026.md`).
- Kayıt formatı:
  - Versiyon
  - Tarih
  - Geliştiren (AI / Kişi)
  - Bilgisayar adı
  - Yapılan geliştirme
- Dosya boyutu **10 MB**'ı aştığında yeni bir tarihli dosya oluşturulur.
- Programatik kayıt için `Helpers/DevLogHelper.cs` kullanılabilir.

## 7. Klasör Yapısı

```
VortexTrade/
├── Constants/       → Sabit değerler
├── Docs/            → Dokümantasyon
│   ├── DevLogs/     → Geliştirme günlükleri (dev_log_DDMMYYYY.md)
│   ├── ai_development_guidelines.md
│   └── manifest.md
├── Enums/           → Enum tanımları
├── Forms/           → Windows Forms dosyaları
├── Helpers/         → Yardımcı sınıflar
├── Models/          → Veri modelleri
├── Properties/      → Uygulama özellikleri ve manifest
├── Program.cs       → Giriş noktası
└── VortexTrade.csproj
```

- Yeni dosyalar uygun klasöre eklenmelidir.
- `Forms/` klasöründe yalnızca form dosyaları bulunmalıdır (`.cs`, `.Designer.cs`, `.resx`).

## 8. Admin ve Yetki

- Uygulama **Windows'ta Administrator** olarak çalışır.
- `Properties/app.manifest` içinde `requestedExecutionLevel level="requireAdministrator"` ayarı bulunmalıdır.

## 9. Genel

- Her geliştirme tamamlandığında sonuç kullanıcıya özetlenmelidir.
- Bu doküman güncel tutulmalı ve yeni kurallar eklendikçe güncellenmelidir.
