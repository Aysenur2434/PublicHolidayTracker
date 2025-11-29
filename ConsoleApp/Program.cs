using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

/// <summary>
/// API'den çekilen Türkiye resmi tatil bilgilerini temsil eden veri modeli (POCO).
/// Alan isimleri, JSON yanıtındaki anahtarlarla birebir eşleşmektedir.
/// </summary>
public class Holiday
{
    // YYYY-MM-DD formatında tatil tarihi.
    public string date { get; set; }
    // Tatilin yerel dildeki adı (Örn: Cumhuriyet Bayramı).
    public string localName { get; set; }
    // Tatilin İngilizce adı (Örn: Republic Day).
    public string name { get; set; }
    // Ülke kodu (TR).
    public string countryCode { get; set; }
    // Tatilin her yıl aynı tarihte olup olmadığını belirtir. '@' C# anahtar kelimesini ayırmak için kullanıldı.
    public bool @fixed { get; set; }
    // Tatilin tüm dünyada geçerli olup olmadığını belirtir.
    public bool global { get; set; }

    /// <summary>
    /// Nesnenin konsol çıktısını düzenler.
    /// </summary>
    /// <returns>Tarih ve yerel adı içeren formatlı dize.</returns>
    public override string ToString()
    {
        return $"{date} - {localName} ({name})";
    }
}

/// <summary>
/// Resmi Tatil Takipçisi konsol uygulamasının ana sınıfı.
/// API ile iletişim kurar ve kullanıcı menüsünü yönetir.
/// </summary>
public class Program
{
    // Uygulama ömrü boyunca tek bir HttpClient örneği kullanmak 'Best Practice'tir.
    private static readonly HttpClient client = new HttpClient();

    // Tüm yıllara ait tatil verilerinin bellekte tutulduğu ana koleksiyon.
    private static List<Holiday> AllHolidays = new List<Holiday>();

    private const string BaseUrl = "https://date.nager.at/api/v3/PublicHolidays/";
    private static readonly int[] Years = { 2023, 2024, 2025 }; // Uygulamanın desteklediği yıllar.

    /// <summary>
    /// Uygulamanın ana giriş noktası. Asenkron işlemleri başlatır ve menüyü çalıştırır.
    /// </summary>
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Resmi Tatil Verileri Yükleniyor...");

        // Uygulama başlatılırken gerekli tüm veriler API'den çekilip belleğe yüklenir.
        await LoadAllHolidaysAsync(Years);

        Console.WriteLine($"\nToplam {AllHolidays.Count} resmi tatil kaydı belleğe yüklendi (2023-2025).");

        await RunMenuAsync();
    }

    #region API VE VERİ İŞLEMLERİ

    /// <summary>
    /// Belirtilen yıl için API'den Türkiye resmi tatil verilerini asenkron olarak çeker.
    /// </summary>
    /// <param name="year">Verilerin çekileceği yıl (Örn: 2024).</param>
    /// <returns>Çekilen tatil listesi veya hata durumunda boş liste.</returns>
    private static async Task<List<Holiday>> GetHolidaysForYearAsync(int year)
    {
        try
        {
            string url = $"{BaseUrl}{year}/TR";

            // API'ye GET isteği gönderilir.
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // HTTP 200 (OK) dışında bir yanıt gelirse hata fırlatılır.

            string responseBody = await response.Content.ReadAsStringAsync();

            // Gelen JSON dizesi, List<Holiday> nesnesine dönüştürülür.
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var holidays = JsonSerializer.Deserialize<List<Holiday>>(responseBody, options);

            return holidays ?? new List<Holiday>();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"\n[HATA] API isteği başarısız oldu ({year}): {e.Message}");
            return new List<Holiday>();
        }
        catch (JsonException e)
        {
            Console.WriteLine($"\n[HATA] JSON dönüştürme hatası ({year}): {e.Message}");
            return new List<Holiday>();
        }
    }

    /// <summary>
    /// Desteklenen tüm yılların (2023-2025) verilerini çeker ve ana listeye ekler.
    /// </summary>
    private static async Task LoadAllHolidaysAsync(int[] years)
    {
        AllHolidays.Clear();
        foreach (var year in years)
        {
            var yearHolidays = await GetHolidaysForYearAsync(year);
            AllHolidays.AddRange(yearHolidays);
        }
    }

    #endregion

    #region MENÜ VE KULLANICI ETKİLEŞİMİ

    /// <summary>
    /// Ana menüyü konsola yazdırır.
    /// </summary>
    private static void DisplayMenu()
    {
        Console.WriteLine("\n===== PublicHolidayTracker =====");
        Console.WriteLine("1. Tatil listesini göster (yıl seçmeli)");
        Console.WriteLine("2. Tarihe göre tatil ara (gg-aa formatı)");
        Console.WriteLine("3. İsme göre tatil ara");
        Console.WriteLine("4. Tüm tatilleri 3 yıl boyunca göster (2023–2025)");
        Console.WriteLine("5. Çıkış");
        Console.Write("Seçiminiz: ");
    }

    /// <summary>
    /// Menü döngüsünü yönetir ve kullanıcı seçimlerine göre ilgili metotları çağırır.
    /// </summary>
    private static async Task RunMenuAsync()
    {
        bool isRunning = true;
        while (isRunning)
        {
            DisplayMenu();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DisplayHolidaysByYear();
                    break;
                case "2":
                    SearchHolidayByDate();
                    break;
                case "3":
                    SearchHolidayByName();
                    break;
                case "4":
                    DisplayAllHolidays();
                    break;
                case "5":
                    isRunning = false;
                    Console.WriteLine("Uygulamadan çıkılıyor. Güle güle!");
                    break;
                default:
                    Console.WriteLine("Geçersiz seçim. Lütfen 1 ile 5 arasında bir sayı girin.");
                    break;
            }
        }
    }

    #endregion

    #region MENÜ İŞLEV METOTLARI (LINQ KULLANIMI)

    /// <summary>
    /// Seçilen yıla ait tatil listesini bellekteki verilerden filtreleyerek gösterir.
    /// </summary>
    private static void DisplayHolidaysByYear()
    {
        Console.Write("Hangi yılın tatillerini görmek istersiniz? (Örn: 2023): ");
        if (int.TryParse(Console.ReadLine(), out int year) && Years.Contains(year))
        {
            // LINQ: Tarih dizesinin yıl ile başladığı kayıtları filtrele ve tarihe göre sırala.
            var holidaysInYear = AllHolidays
                                .Where(h => h.date.StartsWith(year.ToString()))
                                .OrderBy(h => h.date)
                                .ToList();

            if (holidaysInYear.Any())
            {
                Console.WriteLine($"\n--- {year} Resmi Tatil Listesi ---");
                holidaysInYear.ForEach(h => Console.WriteLine(h));
                Console.WriteLine("----------------------------------");
            }
            else
            {
                Console.WriteLine($"\n{year} yılına ait tatil bulunamadı.");
            }
        }
        else
        {
            Console.WriteLine("Geçersiz veya desteklenmeyen yıl girişi.");
        }
    }

    /// <summary>
    /// Kullanıcıdan alınan gg-aa formatındaki tarihe göre tatilleri arar.
    /// </summary>
    private static void SearchHolidayByDate()
    {
        Console.Write("Aramak istediğiniz günü ve ayı girin (gg-aa formatında, Örn: 01-05): ");
        string dayMonth = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(dayMonth) && dayMonth.Length == 5 && dayMonth[2] == '-')
        {
            // Kullanıcının (GG-AA) formatını, API'nin (YYYY-MM-DD) formatındaki son MM-DD kısmına çeviriyoruz.
            // Örn: 01-05 -> "-05-01" 
            string searchString = $"-{dayMonth.Substring(3, 2)}-{dayMonth.Substring(0, 2)}";

            // LINQ: Tarih dizesinin arama stringi ile bittiği kayıtları filtrele.
            var foundHolidays = AllHolidays
                                .Where(h => h.date.EndsWith(searchString))
                                .OrderBy(h => h.date)
                                .ToList();

            if (foundHolidays.Any())
            {
                Console.WriteLine($"\n--- {dayMonth} Tarihine Denk Gelen Tatiller (2023-2025) ---");
                foundHolidays.ForEach(h => Console.WriteLine(h));
                Console.WriteLine("-------------------------------------------------");
            }
            else
            {
                Console.WriteLine($"\n{dayMonth} tarihine denk gelen bir resmi tatil bulunamadı.");
            }
        }
        else
        {
            Console.WriteLine("Geçersiz tarih formatı. Lütfen gg-aa formatında girin.");
        }
    }

    /// <summary>
    /// Kullanıcının girdiği metni içeren tatil isimlerini (yerel veya İngilizce) arar.
    /// </summary>
    private static void SearchHolidayByName()
    {
        Console.Write("Aramak istediğiniz tatil adının bir kısmını girin: ");
        string searchName = Console.ReadLine().Trim().ToLower();

        if (!string.IsNullOrWhiteSpace(searchName))
        {
            // LINQ: localName veya name alanında arama kelimesini içeren kayıtları bul. Büyük/küçük harf duyarlılığını kaldırmak için ToLower() kullanıldı.
            var foundHolidays = AllHolidays
                                .Where(h => h.localName.ToLower().Contains(searchName) || h.name.ToLower().Contains(searchName))
                                .OrderBy(h => h.date)
                                .ToList();

            if (foundHolidays.Any())
            {
                Console.WriteLine($"\n--- '{searchName}' İçeren Tatiller (2023-2025) ---");
                foundHolidays.ForEach(h => Console.WriteLine(h));
                Console.WriteLine("-------------------------------------------------");
            }
            else
            {
                Console.WriteLine($"\n'{searchName}' ile eşleşen bir resmi tatil bulunamadı.");
            }
        }
        else
        {
            Console.WriteLine("Lütfen bir arama kelimesi girin.");
        }
    }

    /// <summary>
    /// Bellekteki 3 yıla ait tüm tatil kayıtlarını tarihe göre sıralayıp konsola yazdırır.
    /// </summary>
    private static void DisplayAllHolidays()
    {
        if (AllHolidays.Any())
        {
            Console.WriteLine("\n--- 2023, 2024 ve 2025 Yılındaki Tüm Resmi Tatiller ---");
            // LINQ: Tüm listeyi tarihe göre (YYYY-MM-DD) sırala.
            AllHolidays
                .OrderBy(h => h.date)
                .ToList()
                .ForEach(h => Console.WriteLine(h));
            Console.WriteLine("----------------------------------------------------");
        }
        else
        {
            Console.WriteLine("Bellekte yüklü resmi tatil verisi bulunmamaktadır.");
        }
    }

    #endregion
}