using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ChatBot_LLM.Interfaces;
using ChatBot_LLM.Models;

namespace ChatBot_LLM.Services
{
    /// <summary>
    /// Ayar servisi
    /// API Key ve diğer konfigürasyonları JSON dosyasında saklar
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsFilePath;
        private AppSettings _currentSettings;
        private readonly object _lockObject = new();

        public event EventHandler? SettingsChanged;

        public SettingsService()
        {
            // Ayarları kullanıcının AppData klasörüne kaydet
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "ChatBot_LLM");
            
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            _settingsFilePath = Path.Combine(appFolder, "settings.json");
            _currentSettings = LoadSettings();
        }

        public bool HasApiKey => !string.IsNullOrWhiteSpace(_currentSettings.ApiKey);

        public string GetApiKey()
        {
            lock (_lockObject)
            {
                return _currentSettings.ApiKey;
            }
        }

        public AppSettings GetSettings()
        {
            lock (_lockObject)
            {
                // Kopya döndür (immutability için)
                return new AppSettings
                {
                    ApiKey = _currentSettings.ApiKey,
                    ModelName = _currentSettings.ModelName,
                    ApiBaseUrl = _currentSettings.ApiBaseUrl,
                    MaxTokens = _currentSettings.MaxTokens,
                    Temperature = _currentSettings.Temperature
                };
            }
        }

        public void SaveSettings(AppSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            lock (_lockObject)
            {
                _currentSettings = new AppSettings
                {
                    ApiKey = settings.ApiKey,
                    ModelName = settings.ModelName,
                    ApiBaseUrl = settings.ApiBaseUrl,
                    MaxTokens = settings.MaxTokens,
                    Temperature = settings.Temperature
                };

                try
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    var json = JsonSerializer.Serialize(_currentSettings, jsonOptions);
                    File.WriteAllText(_settingsFilePath, json);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Ayarlar kaydedilemedi: {ex.Message}", ex);
                }
            }

            // Event'i lock dışında tetikle
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    return settings ?? new AppSettings();
                }
            }
            catch
            {
                // Dosya okunamadıysa varsayılan ayarları kullan
            }

            return new AppSettings();
        }
    }
}
