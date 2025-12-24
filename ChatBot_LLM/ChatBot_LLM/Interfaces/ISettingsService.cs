using System;
using ChatBot_LLM.Models;

namespace ChatBot_LLM.Interfaces
{
    /// <summary>
    /// Ayar servisi kontratı
    /// API Key ve diğer konfigürasyonları yönetir
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Mevcut ayarları getirir
        /// </summary>
        AppSettings GetSettings();

        /// <summary>
        /// Ayarları kaydeder
        /// </summary>
        void SaveSettings(AppSettings settings);

        /// <summary>
        /// API Key'in tanımlı olup olmadığını kontrol eder
        /// </summary>
        bool HasApiKey { get; }

        /// <summary>
        /// API Key değerini getirir
        /// </summary>
        string GetApiKey();

        /// <summary>
        /// Ayarlar değiştiğinde tetiklenir
        /// </summary>
        event EventHandler? SettingsChanged;
    }
}
