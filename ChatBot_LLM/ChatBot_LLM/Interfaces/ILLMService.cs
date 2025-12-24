using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChatBot_LLM.Models;

namespace ChatBot_LLM.Interfaces
{
    /// <summary>
    /// LLM servis kontratı
    /// OpenAI veya diğer LLM sağlayıcıları için ortak interface
    /// </summary>
    public interface ILLMService
    {
        /// <summary>
        /// Tek bir prompt'a yanıt alır
        /// </summary>
        /// <param name="prompt">Kullanıcı mesajı</param>
        /// <param name="cancellationToken">İptal token'ı</param>
        /// <returns>LLM yanıtı</returns>
        Task<string> GetResponseAsync(string prompt, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sohbet geçmişi ile birlikte yanıt alır (context-aware)
        /// </summary>
        /// <param name="conversationHistory">Önceki mesajlar</param>
        /// <param name="cancellationToken">İptal token'ı</param>
        /// <returns>LLM yanıtı</returns>
        Task<string> GetResponseAsync(IEnumerable<ChatMessage> conversationHistory, CancellationToken cancellationToken = default);

        /// <summary>
        /// Servisin kullanıma hazır olup olmadığını kontrol eder
        /// </summary>
        bool IsConfigured { get; }
    }
}
