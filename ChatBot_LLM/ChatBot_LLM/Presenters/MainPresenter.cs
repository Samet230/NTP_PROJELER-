using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChatBot_LLM.Interfaces;
using ChatBot_LLM.Models;

namespace ChatBot_LLM.Presenters
{
    /// <summary>
    /// Ana Presenter (MVP Pattern)
    /// Tüm business logic burada yer alır
    /// Form'dan (View) tamamen bağımsızdır
    /// </summary>
    public class MainPresenter : IDisposable
    {
        private readonly IMainView _view;
        private readonly ILLMService _llmService;
        private readonly IMessageRepository _messageRepository;
        private readonly ISettingsService _settingsService;
        private CancellationTokenSource? _currentRequestCts;
        private bool _disposed;

        public MainPresenter(
            IMainView view,
            ILLMService llmService,
            IMessageRepository messageRepository,
            ISettingsService settingsService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _llmService = llmService ?? throw new ArgumentNullException(nameof(llmService));
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

            // View event'lerine abone ol
            _view.SendMessageRequested += OnSendMessageRequested;
            _view.ClearHistoryRequested += OnClearHistoryRequested;
            _view.OpenSettingsRequested += OnOpenSettingsRequested;
        }

        /// <summary>
        /// Mesaj gönderme işlemi
        /// </summary>
        private async void OnSendMessageRequested(object? sender, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            // API Key kontrolü
            if (!_llmService.IsConfigured)
            {
                _view.ShowError("Lütfen önce Ayarlar'dan API Key'inizi girin.");
                return;
            }

            // Önceki isteği iptal et
            _currentRequestCts?.Cancel();
            _currentRequestCts = new CancellationTokenSource();
            var cancellationToken = _currentRequestCts.Token;

            try
            {
                // UI'ı güncelle
                _view.SetLoadingState(true);
                _view.SetSendButtonEnabled(false);
                _view.ClearInputBox();

                // Kullanıcı mesajını ekle
                var userMessage = new ChatMessage(message, MessageRole.User);
                _messageRepository.AddMessage(userMessage);
                _view.AddMessageToView(userMessage);

                // LLM'den yanıt al
                var conversationHistory = _messageRepository.GetAllMessages();
                var response = await _llmService.GetResponseAsync(conversationHistory, cancellationToken);

                // Yanıtı ekle
                var assistantMessage = new ChatMessage(response, MessageRole.Assistant);
                _messageRepository.AddMessage(assistantMessage);
                _view.AddMessageToView(assistantMessage);
            }
            catch (OperationCanceledException)
            {
                // İstek iptal edildi, sessizce geç
            }
            catch (Exception ex)
            {
                _view.ShowError($"Hata: {ex.Message}");
            }
            finally
            {
                _view.SetLoadingState(false);
                _view.SetSendButtonEnabled(true);
            }
        }

        /// <summary>
        /// Geçmişi temizle
        /// </summary>
        private void OnClearHistoryRequested(object? sender, EventArgs e)
        {
            _messageRepository.ClearHistory();
            _view.ClearMessagesFromView();
            _view.ShowInfo("Sohbet geçmişi temizlendi.");
        }

        /// <summary>
        /// Ayarları aç
        /// </summary>
        private void OnOpenSettingsRequested(object? sender, EventArgs e)
        {
            // Bu event, View tarafından yakalanıp SettingsPopup açılacak
            // Presenter sadece event'i bildirir
        }

        /// <summary>
        /// Mevcut LLM isteğini iptal eder
        /// </summary>
        public void CancelCurrentRequest()
        {
            _currentRequestCts?.Cancel();
        }

        /// <summary>
        /// Presenter'ı başlatır ve mevcut geçmişi yükler
        /// </summary>
        public void Initialize()
        {
            // Eğer önceki oturumdan mesajlar varsa yükle
            var existingMessages = _messageRepository.GetAllMessages();
            foreach (var message in existingMessages)
            {
                _view.AddMessageToView(message);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _view.SendMessageRequested -= OnSendMessageRequested;
                _view.ClearHistoryRequested -= OnClearHistoryRequested;
                _view.OpenSettingsRequested -= OnOpenSettingsRequested;

                _currentRequestCts?.Cancel();
                _currentRequestCts?.Dispose();

                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
