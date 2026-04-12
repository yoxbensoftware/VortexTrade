# Trading Workbench — Manifest

## MDI Tabanlı Modüler Trading Sistemi

### Amaç

Bu uygulama, algoritmik trading stratejilerini yapılandırmak, simüle etmek ve çalıştırmak için tasarlanmış masaüstü tabanlı modüler bir çalışma platformudur.

Sistem; strateji seçimi, coin seçimi, risk yönetimi, AI destekli kararlar ve execution ayarları gibi bağımsız modüllerin bir araya getirilmesiyle çalışır. Her modül kendi konfigürasyonunu üretir ve saklar.

Merkezi bir ana ekran (orchestration), tüm modüllerden gelen ayarları toplar, doğrular ve tek bir "trading session" konfigürasyonu oluşturur. Bu final yapı üzerinden sistem simülasyon (paper trading) veya gerçek alım-satım işlemlerini gerçekleştirir.

---

## Temel Özellikler (Core Features)

### 1. Strateji Motoru (Strategy Engine)

Al/sat kurallarının, indikatörlerin ve sinyal üretim mantığının tanımlandığı çekirdek katman.

### 2. Grafik Motoru (Charting / Visualization)

Fiyat, indikatör, işlem geçmişi ve performans verilerinin görselleştirildiği yapı.

### 3. Emir Motoru (Execution / Order Engine)

Emir oluşturma, gönderme, takip etme, iptal ve fill yönetimini gerçekleştiren katman.

### 4. Bot Yönetimi (Bot Manager)

Birden fazla botu oluşturma, konfigüre etme, başlatma/durdurma ve yönetme yeteneği.

### 5. Varlık Seçimi (Coin / Asset Selection)

Birden fazla coin seçimi, filtreleme ve işlem yapılacak varlık evreninin belirlenmesi.

### 6. Geçmiş Veri Analizi ve Öneri (Historical Insight / Recommendation)

Geçmiş piyasa verilerine göre sinyal üretimi, öneri ve performans analizi.

### 7. Simülasyon (Backtest / Paper Trading)

Stratejilerin gerçek para riski olmadan test edilmesini sağlayan simülasyon ortamı.

### 8. AI Destekli Karar Sistemi (AI Decision Support / AI Tactics)

Açıklanabilir AI tabanlı öneriler, sinyal güçlendirme ve taktik üretimi.

### 9. Merkezi Karar ve Konfigürasyon Birleştirme (Orchestration Layer)

Tüm modüllerden gelen ayarları birleştirerek final trading session oluşturan ana yapı.

### 10. Loglama ve İzlenebilirlik (Audit & Logging)

Tüm işlemlerin, kararların ve sistem aksiyonlarının kayıt altına alınması.

### 11. Risk Yönetimi (Risk Management)

Stop-loss, take-profit, maksimum zarar limiti gibi risk kurallarının uygulanması.

### 12. Portföy Yönetimi (Portfolio Management)

Varlık dağılımı, bakiye yönetimi ve pozisyon takibi.

### 13. Bildirim ve Uyarı Sistemi (Notification System)

İşlem, hata ve kritik durumlar için kullanıcıya anlık bildirim gönderimi.

---

## Vizyon

Karmaşık trading senaryolarını modüler bir yapı ile görsel olarak oluşturulabilen, merkezi olarak doğrulanabilen ve güvenli şekilde çalıştırılabilen esnek bir masaüstü platform sunmak.
