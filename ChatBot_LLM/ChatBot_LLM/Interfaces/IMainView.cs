using System;
using ChatBot_LLM.Models;

namespace ChatBot_LLM.Interfaces
{
    /// <summary>
    /// Ana görünüm kontratı (MVP Pattern)
    /// Form'un Presenter'a sunduğu interface
    /// </summary>
    public interface IMainView
    {
        /// <summary>
        /// Mesaj gönderme isteği
        /// </summary>
        event EventHandler<string>? SendMessageRequested;

        /// <summary>
        /// Geçmişi temizleme isteği
        /// </summary>
        event EventHandler? ClearHistoryRequested;

        /// <summary>
        /// Ayarlar açma isteği
        /// </summary>
        event EventHandler? OpenSettingsRequested;

        /// <summary>
        /// Mesaj listesine yeni mesaj ekler
        /// </summary>
        void AddMessageToView(ChatMessage message);

        /// <summary>
        /// Tüm mesajları görünümden temizler
        /// </summary>
        void ClearMessagesFromView();

        /// <summary>
        /// Yükleniyor durumunu gösterir/gizler
        /// </summary>
        void SetLoadingState(bool isLoading);

        /// <summary>
        /// Hata mesajı gösterir
        /// </summary>
        void ShowError(string message);

        /// <summary>
        /// Bilgi mesajı gösterir
        /// </summary>
        void ShowInfo(string message);

        /// <summary>
        /// Mesaj giriş kutusunu temizler
        /// </summary>
        void ClearInputBox();

        /// <summary>
        /// Gönder butonunu etkinleştirir/devre dışı bırakır
        /// </summary>
        void SetSendButtonEnabled(bool enabled);
    }
}
