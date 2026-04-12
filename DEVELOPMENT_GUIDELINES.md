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

## 5. Genel

- Her geliştirme isteği tamamlandığında sonuçlar kullanıcıya özet olarak sunulmalıdır.
- Bu dosya güncel tutulmalı ve yeni kurallar eklendiğinde güncellenmelidir.
