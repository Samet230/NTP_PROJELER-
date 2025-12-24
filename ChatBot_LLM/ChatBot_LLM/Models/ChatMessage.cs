using System;

namespace ChatBot_LLM.Models
{
    /// <summary>
    /// Sohbet mesajı veri modeli
    /// Kullanıcı veya asistan mesajlarını temsil eder
    /// </summary>
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageRole Role { get; set; }
        public DateTime Timestamp { get; set; }

        public ChatMessage()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.Now;
        }

        public ChatMessage(string content, MessageRole role) : this()
        {
            Content = content;
            Role = role;
        }
    }

    /// <summary>
    /// Mesaj rolü - Kullanıcı veya Asistan
    /// </summary>
    public enum MessageRole
    {
        User,
        Assistant
    }
}
