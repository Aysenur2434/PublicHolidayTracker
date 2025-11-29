PublicHolidayTracker
📌 Proje Hakkında

PublicHolidayTracker C# ile geliştirilmiş asenkron konsol uygulaması. System.Net.Http.HttpClient aracılığıyla harici bir API'ye (nager.at) bağlanarak 3 yıllık (2023-2025) Türkiye resmi tatil verilerini JSON formatında çekip RAM'de (bellekte) saklar. Uygulama, kullanıcıya LINQ tabanlı filtreleme (yıla, isme ve tarihe göre arama) işlevleri sunar.

🌐 API Kaynağı

Uygulama aşağıdaki Nager.Date Public Holidays API
 kullanılarak verileri çeker:

2023: https://date.nager.at/api/v3/PublicHolidays/2023/TR

2024: https://date.nager.at/api/v3/PublicHolidays/2024/TR

2025: https://date.nager.at/api/v3/PublicHolidays/2025/TR

💻 Kullanılan Teknolojiler

C# .NET 6 / 7 Console App

HttpClient (API’den veri çekmek için)

System.Text.Json (JSON verilerini deserialize etmek için)

LINQ (Filtreleme ve sıralama işlemleri için)

⚙️ Özellikler

Yıl Seçerek Tatil Listesi Görüntüleme
Kullanıcı istediği yılın resmi tatillerini görüntüleyebilir.

Tarihe Göre Tatil Arama
Kullanıcı gg-aa formatında bir tarih girerek tatil olup olmadığını kontrol edebilir.

İsme Göre Tatil Arama
Tatil ismi veya kısmi isme göre arama yapılabilir.

Tüm Tatilleri 3 Yıl Boyunca Gösterme
2023, 2024 ve 2025 yıllarındaki tüm tatiller sıralı şekilde listelenir.

Çıkış
Kullanıcı menüden çıkış yapabilir.

📝 Kurulum ve Çalıştırma

Visual Studio veya .NET 6/7 destekleyen bir IDE açın.

Yeni bir Console App projesi oluşturun.

Program.cs dosyasına proje kodlarını yapıştırın.

Projeyi derleyin ve çalıştırın (F5 veya Ctrl+F5).

Menü üzerinden işlemleri yapabilirsiniz.

📂 Kod Yapısı

Holiday sınıfı: API’den gelen JSON verilerini temsil eder.

Program sınıfı: API çağrıları, menü ve kullanıcı etkileşimi metotlarını içerir.

Async/await ile API çağrıları ve verilerin belleğe yüklenmesi yapılır.

🎯 Kullanım Örnekleri
===== PublicHolidayTracker =====
1. Tatil listesini göster (yıl seçmeli)
2. Tarihe göre tatil ara (gg-aa formatı)
3. İsme göre tatil ara
4. Tüm tatilleri 3 yıl boyunca göster (2023–2025)
5. Çıkış
Seçiminiz: 1
Hangi yılın tatillerini görmek istersiniz? 2023
01-01 - Yılbaşı (New Year's Day)
23-04 - Ulusal Egemenlik ve Çocuk Bayramı (National Sovereignty and Children's Day)
...

✅ Notlar

Konsol çıktısı Türkçe karakterleri destekler.

API’ye bağlanamadığında hata mesajı gösterir, uygulama kapanmaz.

Kullanıcı hatalı giriş yaptığında açıklayıcı mesaj verilir.

Kod async olarak çalıştığı için API çağrıları sırasında program donmaz.
