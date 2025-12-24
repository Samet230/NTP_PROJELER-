using System;
using System.Collections.Generic;
using ChatBot_LLM.Models;

namespace ChatBot_LLM.Interfaces
{
    /// <summary>
    /// Mesaj deposu kontratı
    /// Sohbet geçmişini yönetir
    /// </summary>
    public interface IMessageRepository
    {
        /// <summary>
        /// Yeni mesaj ekler
        /// </summary>
        void AddMessage(ChatMessage message);

        /// <summary>
        /// Tüm mesajları getirir
        /// </summary>
        IReadOnlyList<ChatMessage> GetAllMessages();

        /// <summary>
        /// Sohbet geçmişini temizler
        /// </summary>
        void ClearHistory();

        /// <summary>
        /// Toplam mesaj sayısı
        /// </summary>
        int MessageCount { get; }
    }
}
