using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ceviri_App
{
    public class OnlineTranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;

        public OnlineTranslationService()
        {
            _httpClient = new HttpClient();
        }

        public string Translate(string text, string fromLang, string toLang)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";

            // MyMemory API expects language codes like "en|tr" (English to Turkish)
            // We need to map our full language names to codes.
            string fromCode = GetLanguageCode(fromLang);
            string toCode = GetLanguageCode(toLang);

            if (fromCode == null || toCode == null)
                return "Hata: Desteklenmeyen dil seçimi.";

            string url = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(text)}&langpair={fromCode}|{toCode}";

            try
            {
                // Synchronous call for simplicity in this student project context, 
                // though async/await is better practice for UI apps.
                // To keep the interface simple (string return), we use .Result.
                // In a real pro app, we would make the Interface async (Task<string>).
                HttpResponseMessage response = _httpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                
                using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                {
                    string translatedText = doc.RootElement
                        .GetProperty("responseData")
                        .GetProperty("translatedText")
                        .GetString();

                    return translatedText;
                }
            }
            catch (Exception ex)
            {
                return $"Hata: {ex.Message}";
            }
        }

        private string GetLanguageCode(string languageName)
        {
            return languageName switch
            {
                "English" => "en",
                "Turkish" => "tr",
                "German" => "de",
                "French" => "fr",
                "Spanish" => "es",
                _ => null
            };
        }
    }
}
