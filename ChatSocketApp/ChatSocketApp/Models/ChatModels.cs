using System;
using System.Drawing;

namespace ChatSocketApp.Models
{
    /// <summary>
    /// Mesaj türlerini tanımlar
    /// </summary>
    public enum MessageType
    {
        Normal,     // Normal chat mesajı
        System,     // Sistem bildirimi
        Private,    // Özel mesaj
        Notification // Bildirim
    }

    /// <summary>
    /// Chat mesajı modeli
    /// </summary>
    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public MessageType Type { get; set; }
        public bool IsFromMe { get; set; }
        public string PrivateRecipient { get; set; }

        public ChatMessage()
        {
            Timestamp = DateTime.Now;
            Type = MessageType.Normal;
        }

        public ChatMessage(string sender, string content, bool isFromMe = false) : this()
        {
            Sender = sender;
            Content = content;
            IsFromMe = isFromMe;
        }

        /// <summary>
        /// Mesajın görüntüleme formatı
        /// </summary>
        public string DisplayText => $"{Timestamp:HH:mm} {Sender}: {Content}";

        /// <summary>
        /// Sistem mesajı oluşturur
        /// </summary>
        public static ChatMessage CreateSystemMessage(string content)
        {
            return new ChatMessage
            {
                Sender = "SİSTEM",
                Content = content,
                Type = MessageType.System,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Kullanıcı modeli
    /// </summary>
    public class User
    {
        public string Username { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }
        public Color AvatarColor { get; set; }
        public bool IsTyping { get; set; }

        private static readonly Color[] AvatarColors = new Color[]
        {
            Color.FromArgb(79, 195, 247),   // Açık Mavi
            Color.FromArgb(129, 199, 132),  // Yeşil
            Color.FromArgb(255, 183, 77),   // Turuncu
            Color.FromArgb(240, 98, 146),   // Pembe
            Color.FromArgb(149, 117, 205),  // Mor
            Color.FromArgb(77, 208, 225),   // Cyan
            Color.FromArgb(255, 138, 128),  // Kırmızı
            Color.FromArgb(174, 213, 129),  // Açık Yeşil
        };

        public User()
        {
            IsOnline = true;
            LastSeen = DateTime.Now;
            // Rastgele avatar rengi ata
            AvatarColor = AvatarColors[Math.Abs(Username?.GetHashCode() ?? 0) % AvatarColors.Length];
        }

        public User(string username) : this()
        {
            Username = username;
            AvatarColor = AvatarColors[Math.Abs(username.GetHashCode()) % AvatarColors.Length];
        }

        /// <summary>
        /// Kullanıcı adının baş harfini döndürür
        /// </summary>
        public string Initials => string.IsNullOrEmpty(Username) ? "?" : Username[0].ToString().ToUpper();
    }

    /// <summary>
    /// Ağ mesaj protokolü için paket türleri
    /// </summary>
    public enum PacketType : byte
    {
        Message = 0x01,
        Connect = 0x02,
        Disconnect = 0x03,
        Private = 0x04,
        Typing = 0x05,
        UserList = 0x06,
        Ping = 0x07,
        Pong = 0x08
    }

    /// <summary>
    /// Ağ üzerinden gönderilen paket
    /// </summary>
    public class NetworkPacket
    {
        public PacketType Type { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public string Target { get; set; }
        public DateTime Timestamp { get; set; }

        public NetworkPacket()
        {
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// JSON formatına çevirir
        /// </summary>
        public string ToJson()
        {
            // Basit JSON serileştirme (harici kütüphane olmadan)
            return $"{{\"type\":{(int)Type},\"sender\":\"{EscapeJson(Sender ?? "")}\",\"content\":\"{EscapeJson(Content ?? "")}\",\"target\":\"{EscapeJson(Target ?? "")}\",\"timestamp\":\"{Timestamp:O}\"}}";
        }

        /// <summary>
        /// JSON'dan parse eder
        /// </summary>
        public static NetworkPacket FromJson(string json)
        {
            try
            {
                var packet = new NetworkPacket();
                
                // Basit JSON parse
                packet.Type = (PacketType)ExtractInt(json, "type");
                packet.Sender = ExtractString(json, "sender");
                packet.Content = ExtractString(json, "content");
                packet.Target = ExtractString(json, "target");
                
                var timestampStr = ExtractString(json, "timestamp");
                if (!string.IsNullOrEmpty(timestampStr))
                    DateTime.TryParse(timestampStr, out var ts);
                
                return packet;
            }
            catch
            {
                return null;
            }
        }

        private static string EscapeJson(string s)
        {
            return s?.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r") ?? "";
        }

        private static string ExtractString(string json, string key)
        {
            var pattern = $"\"{key}\":\"";
            var startIdx = json.IndexOf(pattern);
            if (startIdx < 0) return "";
            startIdx += pattern.Length;
            
            var endIdx = startIdx;
            while (endIdx < json.Length)
            {
                if (json[endIdx] == '"' && json[endIdx - 1] != '\\')
                    break;
                endIdx++;
            }
            
            return json.Substring(startIdx, endIdx - startIdx)
                .Replace("\\\"", "\"")
                .Replace("\\\\", "\\")
                .Replace("\\n", "\n")
                .Replace("\\r", "\r");
        }

        private static int ExtractInt(string json, string key)
        {
            var pattern = $"\"{key}\":";
            var startIdx = json.IndexOf(pattern);
            if (startIdx < 0) return 0;
            startIdx += pattern.Length;
            
            var endIdx = startIdx;
            while (endIdx < json.Length && (char.IsDigit(json[endIdx]) || json[endIdx] == '-'))
                endIdx++;
            
            int.TryParse(json.Substring(startIdx, endIdx - startIdx), out var result);
            return result;
        }
    }
}
