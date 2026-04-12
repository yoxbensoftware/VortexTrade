# VortexTrade — Geliştirme Kuralları

Bu dosya, Copilot ve geliştirici tarafından her zaman uyulması gereken temel kuralları içerir.

---

## 1. Derleme (Build) Kuralları

- **Her değişiklikten sonra proje derlenmelidir.**
- Derleme sonucunda **hiçbir Error veya Warning olmamalıdır.**
- Derleme başarısız olursa, sonraki adıma geçilmeden hata çözülmelidir.

## 2. UI / Arayüz Kuralları

- Kontroller (buton, label vb.) **her zaman görünür alanda (client area) kalmalıdır.**
- Form boyutlandırmada `Size` yerine **`ClientSize`** tercih edilmelidir; böylece başlık çubuğu ve kenarlıklar hesaba katılmadan tutarlı iç alan elde edilir.
- Kontrollerin konumları belirlenirken **alt ve sağ kenardan en az 16px boşluk** bırakılmalıdır.
- Yeni bir pencere/dialog oluşturulduğunda, tüm kontrollerin taşmadığı görsel olarak doğrulanmalıdır.

## 3. Kod Kalitesi

- Varolan kod stili ve yapısına uyulmalıdır (Consolas font, retro tema, Türkçe menüler).
- Gereksiz yorum satırı eklenmemelidir; yalnızca karmaşık mantık açıklanmalıdır.
- Kullanılmayan `using` ifadeleri ve değişkenler temizlenmelidir.

## 4. Tema ve Stil Tutarlılığı

- Tüm yeni formlar ve kontroller, mevcut tema sistemine (`ThemeColors`) uyumlu olmalıdır.
- Sabit renkler yerine tema renkleri (`bg`, `fg`, `accent`) kullanılmalıdır.

## 5. Versiyonlama Sistemi

- Her geliştirme veya commit sonrasında versiyon **V.0.0.X** şeklinde 1 artırılır.
- Kullanıcı **"YENİ SÜRÜM OLUŞTUR"** dediğinde ana versiyon artırılır: **V.X.0.0**.
- Versiyon bilgisi `Constants/AppConstants.cs` dosyasında ve `.csproj` içinde güncellenir.
- Versiyon değişikliği `DEVLOG.md`'ye kayıt edilir.

## 6. Geliştirme Günlüğü (DEVLOG.md)

- **Her geliştirme** sonunda `DEVLOG.md` dosyasına kayıt eklenir.
- Kayıt formatı: **Versiyon, Tarih, Geliştiren AI/Kişi, Bilgisayar Adı, Yapılan Geliştirme**.
- `DEVLOG.md` dosya boyutu **10 MB**'ı aştığında `DEVLOG_archive_NNN.md` olarak arşivlenir ve yeni `DEVLOG.md` başlatılır.
- Programatik kayıt için `Helpers/DevLogHelper.cs` kullanılabilir.

## 7. Klasör Yapısı

```
VortexTrade/
├── Constants/       → Sabit değerler (AppConstants vb.)
├── Enums/           → Enum tanımları (ThemeType, TradingEnums vb.)
├── Forms/           → Windows Forms dosyaları (.cs, .Designer.cs, .resx)
├── Helpers/         → Yardımcı sınıflar (DevLogHelper vb.)
├── Models/          → Veri modelleri (DevLogEntry vb.)
├── Properties/      → Manifest ve uygulama özellikleri
├── Program.cs       → Uygulama giriş noktası
├── VortexTrade.csproj
├── DEVELOPMENT_GUIDELINES.md
└── DEVLOG.md
```

- Yeni dosyalar ilgili klasöre eklenmelidir.
- Forms klasörüne sadece Form dosyaları (.cs, .Designer.cs, .resx) konulmalıdır.

## 8. Admin ve Yetki

- Uygulama **Windows'ta Administrator** olarak çalışır (`Properties/app.manifest`).
- Manifest'te `requestedExecutionLevel level="requireAdministrator"` ayarlıdır.

## 9. Genel

- Her geliştirme isteği tamamlandığında sonuçlar kullanıcıya özet olarak sunulmalıdır.
- Bu dosya güncel tutulmalı ve yeni kurallar eklendiğinde güncellenmelidir.
