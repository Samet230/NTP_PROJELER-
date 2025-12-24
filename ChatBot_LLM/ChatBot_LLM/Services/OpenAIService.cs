using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ChatBot_LLM.Interfaces;
using ChatBot_LLM.Models;

namespace ChatBot_LLM.Services
{
    /// <summary>
    /// OpenAI API servisi
    /// ILLMService implementasyonu
    /// </summary>
    public class OpenAIService : ILLMService, IDisposable
    {
        private readonly ISettingsService _settingsService;
        private readonly HttpClient _httpClient;
        private bool _disposed;

        public OpenAIService(ISettingsService settingsService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(120);
        }

        public bool IsConfigured => _settingsService.HasApiKey;

        public async Task<string> GetResponseAsync(string prompt, CancellationToken cancellationToken = default)
        {
            var message = new ChatMessage(prompt, MessageRole.User);
            return await GetResponseAsync(new[] { message }, cancellationToken);
        }

        public async Task<string> GetResponseAsync(IEnumerable<ChatMessage> conversationHistory, CancellationToken cancellationToken = default)
        {
            if (!IsConfigured)
            {
                throw new InvalidOperationException("API Key ayarlanmamış.");
            }

            var settings = _settingsService.GetSettings();
            var requestUrl = $"{settings.ApiBaseUrl}/chat/completions";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);

            var messages = conversationHistory.Select(m => new
            {
                role = m.Role.ToString().ToLower(),
                content = m.Content
            }).ToList();

            var requestBody = new
            {
                model = settings.ModelName,
                messages = messages,
                max_tokens = settings.MaxTokens,
                temperature = settings.Temperature
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(requestUrl, content, cancellationToken);
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var error = "API Hatası";
                    try 
                    {
                        using var doc = JsonDocument.Parse(responseJson);
                        if (doc.RootElement.TryGetProperty("error", out var errorEl) && 
                            errorEl.TryGetProperty("message", out var msgEl))
                        {
                            error = msgEl.GetString() ?? error;
                        }
                    }
                    catch { /* Parse hatası olursa varsayılan mesaj kalsın */ }
                    
                    throw new HttpRequestException($"API Hatası ({response.StatusCode}): {error}");
                }

                using var document = JsonDocument.Parse(responseJson);
                var root = document.RootElement;
                return root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                throw new Exception($"LLM yanıtı alınamadı: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
