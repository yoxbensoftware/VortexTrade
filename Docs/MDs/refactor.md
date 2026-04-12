# VortexTrade — REFACTOR MASTER GUIDE

Bu doküman, AI tarafından gerçekleştirilecek tüm refactor, code review ve iyileştirme işlemleri için referans alınmalıdır.

Kullanıcı `REFACTOR YAP` dediğinde, AI bu dosyaya göre hareket eder.

---

## 1. Amaç

- Mevcut kodu bozmadan iyileştirmek
- Error ve Warning'leri sıfırlamak
- Kod kalitesini artırmak
- Performans ve güvenlik risklerini azaltmak
- Mimariyi koruyarak düzenleme yapmak

## 2. Temel Kurallar

- Mevcut çalışan akış **KESİNLİKLE** bozulmaz
- UI davranışı değiştirilmez
- İş mantığı değiştirilmez (sadece iyileştirilir)
- Yeni özellik eklenmez (refactor scope dışı)
- Gereksiz karmaşıklık oluşturulmaz

## 3. Derleme ve Hata Yönetimi

Refactor sonrası:

- `0 Error`
- `0 Warning`

Warning'ler:

- `nullable`
- `unused variable`
- `async misuse`
- `dead code`

mutlaka temizlenmelidir.

## 4. Kod Temizliği (Code Cleanup)

AI aşağıdakileri uygular:

- Kullanılmayan `using` kaldırılır
- Kullanılmayan değişkenler silinir
- Duplicate kodlar sadeleştirilir
- Uzun metotlar küçük parçalara bölünür
- Magic number/string → constant yapılır
- Gereksiz yorum satırları temizlenir

## 5. Mimari Koruma

- Mevcut proje yapısı korunur
- Katmanlar arası bağımlılık artırılmaz
- Form içindeki iş mantığı mümkünse servis katmanına taşınır (kırmadan)
- Modüller arası coupling artırılmaz

## 6. Performans İyileştirmeleri

AI aşağıdaki riskleri kontrol eder:

- Gereksiz loop / tekrar hesaplama
- UI thread bloklayan işlemler
- Senkron I/O işlemleri (async'e uygun olanlar düzeltilir)
- Büyük koleksiyonlarda gereksiz LINQ kullanımı
- Gereksiz object creation

## 7. UI Performansı (WinForms / MDI)

- UI thread bloklanmamalı
- Uzun işlemler background'da çalışmalı
- `Invoke` / `BeginInvoke` doğru kullanılmalı
- Gereksiz redraw / refresh azaltılmalı

## 8. Güvenlik Kontrolleri

- Hardcoded API key / secret kontrol edilir
- Hassas veriler loglanmaz
- Exception mesajları güvenli hale getirilir
- Input validation eksikleri belirlenir

## 9. Logging ve Hata Yönetimi

- Try-catch blokları mantıklı hale getirilir
- Silent catch blokları kaldırılır
- Kritik hatalar loglanır
- Debug yerine structured logging tercih edilir

## 10. Refactor Seviyeleri

AI işlemi 3 seviyede yapar:

### Seviye 1 — Safe Cleanup

- Kod temizliği
- Warning fix
- Naming düzeltmeleri

### Seviye 2 — Structural Improvement

- Metot bölme
- Readability artırma
- Basit performans iyileştirme

### Seviye 3 — Advanced Optimization

- Async dönüşüm
- Daha iyi pattern kullanımı
- Gereksiz bağımlılık azaltma

## 11. Riskli İşlemler (YAPILMAZ)

AI aşağıdakileri yapmaz:

- Büyük mimari değişiklik
- Yeni dependency ekleme
- DB schema değiştirme
- API contract değiştirme
- UI davranışını değiştirme

## 12. Çıktı Formatı

Refactor sonrası AI:

- Yapılan değişiklikleri listeler
- Neden yapıldığını açıklar
- Risk durumu belirtir (`LOW` / `MEDIUM`)
- Etkilenen dosyaları belirtir

## 13. Kural

AI her refactor işleminde:

- Bu dokümana uyar
- Scope dışına çıkmaz
- Güvenli refactor yapar

## 14. Trigger

Kullanıcı aşağıdaki ifadeleri kullandığında bu doküman aktif olur:

- `REFACTOR YAP`
- `CODE REVIEW YAP`
- `KODU TEMİZLE`
- `PERFORMANCE FIX YAP`
