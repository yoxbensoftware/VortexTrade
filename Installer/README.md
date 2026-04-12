# VortexTrade Installer

Windows için kurulum paketi oluşturma araçları.

## Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [InnoSetup 6](https://jrsoftware.org/isdl.php)

## Kullanım

```powershell
cd Installer
.\build-installer.ps1
```

## Çıktı

- `publish/` — Self-contained uygulama dosyaları
- `Output/VortexTrade_Setup_v{version}.exe` — Kurulum dosyası

## Özellikler

- Windows Program Ekle/Kaldır desteği
- 64-bit kurulum
- Masaüstü kısayolu (opsiyonel)
- Başlat menüsü grubu
- Türkçe ve İngilizce dil desteği
- Self-contained (hedef makinede .NET kurulumu gerekmez)
- LZMA2 sıkıştırma
