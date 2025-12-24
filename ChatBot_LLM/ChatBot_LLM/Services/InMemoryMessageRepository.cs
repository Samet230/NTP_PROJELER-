using ChatBot_LLM.Interfaces;
using ChatBot_LLM.Models;

namespace ChatBot_LLM.Services
{
    /// <summary>
    /// Bellek içi mesaj deposu
    /// Sohbet geçmişini List yapısında tutar
    /// </summary>
    public class InMemoryMessageRepository : IMessageRepository
    {
        private readonly List<ChatMessage> _messages;
        private readonly object _lockObject = new();

        public InMemoryMessageRepository()
        {
            _messages = new List<ChatMessage>();
        }

        public int MessageCount
        {
            get
            {
                lock (_lockObject)
                {
                    return _messages.Count;
                }
            }
        }

        public void AddMessage(ChatMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            lock (_lockObject)
            {
                _messages.Add(message);
            }
        }

        public IReadOnlyList<ChatMessage> GetAllMessages()
        {
            lock (_lockObject)
            {
                // Dışarıya kopya döndür (thread-safe)
                return _messages.ToList().AsReadOnly();
            }
        }

        public void ClearHistory()
        {
            lock (_lockObject)
            {
                _messages.Clear();
            }
        }
    }
}
