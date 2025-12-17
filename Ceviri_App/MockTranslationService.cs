using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceviri_App
{
    // ÇOK BİÇİMLİLİK (POLYMORPHISM) ve MOCKING:
    // Bu sınıf, ITranslationService arayüzünü implemente eder ancak gerçek bir API'ye bağlanmaz.
    // Test aşamasında internet bağlantısı veya API anahtarı gerektirmeden uygulamanın çalışmasını sağlar.
    public class MockTranslationService : ITranslationService
    {
        public string Translate(string text, string fromLang, string toLang)
        {
            // Basit bir simülasyon:
            // Gerçek bir çeviri yapmaz, sadece test amaçlı çıktı üretir.
            
            if (string.IsNullOrWhiteSpace(text))
                return "";

            // Örnek senaryo: Eğer "Hello" yazılırsa "Merhaba" döndür.
            if (text.Trim().Equals("Hello", StringComparison.OrdinalIgnoreCase) && fromLang == "English" && toLang == "Turkish")
            {
                return "Merhaba (Mock)";
            }

            // Genel durum: Metnin sonuna [Dil -> Dil] ekleyerek çevrildiğini simüle et.
            return $"[MOCK ÇEVİRİ] {text} ({fromLang} -> {toLang})";
        }
    }
}
