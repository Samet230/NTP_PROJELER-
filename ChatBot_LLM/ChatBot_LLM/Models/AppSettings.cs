using System;

namespace ChatBot_LLM.Models
{
    /// <summary>
    /// Uygulama ayarları modeli
    /// API Key ve diğer konfigürasyonları tutar
    /// </summary>
    public class AppSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ModelName { get; set; } = "gpt-3.5-turbo";
        public string ApiBaseUrl { get; set; } = "https://api.openai.com/v1";
        public int MaxTokens { get; set; } = 2048;
        public double Temperature { get; set; } = 0.7;
    }
}
