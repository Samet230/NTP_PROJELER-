using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Navigation;
using DevExpress.Utils.Html;
using DevExpress.Utils.Svg;
using ChatBot_LLM.Interfaces;
using ChatBot_LLM.Models;

namespace ChatBot_LLM.Views
{
    /// <summary>
    /// Modern ana sohbet formu
    /// Premium DevExpress tasarımı - DirectX destekli
    /// </summary>
    public partial class MainForm : XtraForm, IMainView
    {
        // Events (MVP Pattern)
        public event EventHandler<string>? SendMessageRequested;
        public event EventHandler? ClearHistoryRequested;
        public event EventHandler? OpenSettingsRequested;

        // Premium UI Kontrolleri
        private AccordionControl sidebarAccordion = null!;
        private HtmlContentControl chatHtmlControl = null!;
        private MemoEdit messageInput = null!;
        private SimpleButton sendButton = null!;
        private SimpleButton clearButton = null!;
        private LabelControl statusLabel = null!;
        private PanelControl mainChatPanel = null!;
        private PanelControl inputPanel = null!;

        // Mesaj HTML içeriği
        private readonly StringBuilder _chatHtmlContent = new();
        private int _messageCount = 0;

        public MainForm()
        {
            InitializeComponent();
            InitializePremiumControls();
            SetupEventHandlers();
            ApplyPremiumStyling();
        }

        private void InitializePremiumControls()
        {
            // Form ayarları - Premium görünüm
            this.Text = "🤖 LLM ChatBot";
            this.Size = new Size(1100, 750);
            this.MinimumSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            // Premium görünüm için koyu tema

            // Ana layout
            var mainSplitContainer = new SplitContainerControl
            {
                Dock = DockStyle.Fill,
                Horizontal = true,
                SplitterPosition = 220,
                ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.False
            };

            // ========== SOL SİDEBAR ==========
            sidebarAccordion = new AccordionControl
            {
                Dock = DockStyle.Fill,
                ViewType = AccordionControlViewType.HamburgerMenu,
                ExpandGroupOnHeaderClick = true
            };

            // Sidebar grupları
            var chatGroup = new AccordionControlElement
            {
                Text = "💬 Sohbet",
                Style = ElementStyle.Group,
                Expanded = true
            };

            var newChatItem = new AccordionControlElement { Text = "🆕 Yeni Sohbet", Style = ElementStyle.Item };
            var historyItem = new AccordionControlElement { Text = "📜 Geçmiş", Style = ElementStyle.Item };
            chatGroup.Elements.AddRange(new[] { newChatItem, historyItem });

            var settingsGroup = new AccordionControlElement
            {
                Text = "⚙️ Ayarlar",
                Style = ElementStyle.Group
            };

            var apiKeyItem = new AccordionControlElement { Text = "🔑 API Anahtarı", Style = ElementStyle.Item };
            var modelItem = new AccordionControlElement { Text = "🧠 Model Seçimi", Style = ElementStyle.Item };
            var themeItem = new AccordionControlElement { Text = "🎨 Tema", Style = ElementStyle.Item };
            settingsGroup.Elements.AddRange(new[] { apiKeyItem, modelItem, themeItem });

            sidebarAccordion.Elements.AddRange(new[] { chatGroup, settingsGroup });

            // Sidebar olayları
            apiKeyItem.Click += (s, e) => 
            {
                OpenSettingsRequested?.Invoke(this, EventArgs.Empty);
                ShowSettingsPopup();
            };
            
            modelItem.Click += (s, e) => 
            {
                OpenSettingsRequested?.Invoke(this, EventArgs.Empty);
                ShowSettingsPopup();
            };
            
            themeItem.Click += (s, e) =>
            {
                // Tema değiştir
                var currentSkin = DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName;
                if (currentSkin == "Office 2019 Colorful")
                {
                    DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(DevExpress.LookAndFeel.SkinStyle.Office2019DarkGray);
                    ShowInfo("🌙 Koyu tema aktif edildi!");
                }
                else
                {
                    DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(DevExpress.LookAndFeel.SkinStyle.Office2019Colorful);
                    ShowInfo("☀️ Açık tema aktif edildi!");
                }
            };
            
            newChatItem.Click += (s, e) => 
            {
                ClearHistoryRequested?.Invoke(this, EventArgs.Empty);
                ClearMessagesFromView();
                ShowInfo("🆕 Yeni sohbet başlatıldı!");
            };
            
            historyItem.Click += (s, e) =>
            {
                ShowInfo("📜 Geçmiş özelliği yakında eklenecek!");
            };

            // ========== SAĞ ANA PANEL ==========
            mainChatPanel = new PanelControl
            {
                Dock = DockStyle.Fill,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };

            // Chat HTML alanı - Modern mesaj balonları
            chatHtmlControl = new HtmlContentControl
            {
                Dock = DockStyle.Fill
            };
            InitializeChatHtmlTemplate();

            // Input paneli
            inputPanel = new PanelControl
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder,
                Padding = new Padding(15, 10, 15, 15)
            };

            var inputLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 3,
                Padding = new Padding(0)
            };
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));

            // Mesaj giriş alanı - Premium stil
            messageInput = new MemoEdit
            {
                Dock = DockStyle.Fill,
                Properties =
                {
                    NullText = "Mesajınızı buraya yazın... (Ctrl+Enter ile gönderin)",
                    Appearance = { Font = new Font("Segoe UI", 11f) }
                }
            };

            // Gönder butonu - Gradient efekt
            sendButton = new SimpleButton
            {
                Text = "📤 Gönder",
                Dock = DockStyle.Fill,
                Appearance = { Font = new Font("Segoe UI Semibold", 11f) },
                Margin = new Padding(10, 0, 5, 0)
            };

            // Temizle butonu
            clearButton = new SimpleButton
            {
                Text = "🗑️ Temizle",
                Dock = DockStyle.Fill,
                Margin = new Padding(5, 0, 0, 0)
            };

            inputLayout.Controls.Add(messageInput, 0, 0);
            inputLayout.Controls.Add(sendButton, 1, 0);
            inputLayout.Controls.Add(clearButton, 2, 0);
            inputPanel.Controls.Add(inputLayout);

            // Status bar
            statusLabel = new LabelControl
            {
                Text = "🟢 Hazır",
                Dock = DockStyle.Top,
                Padding = new Padding(15, 8, 15, 8),
                Appearance = { Font = new Font("Segoe UI", 9f, FontStyle.Italic) }
            };

            // Kontrolleri yerleştir
            mainChatPanel.Controls.Add(chatHtmlControl);
            mainChatPanel.Controls.Add(statusLabel);
            mainChatPanel.Controls.Add(inputPanel);

            mainSplitContainer.Panel1.Controls.Add(sidebarAccordion);
            mainSplitContainer.Panel2.Controls.Add(mainChatPanel);

            this.Controls.Add(mainSplitContainer);
        }

        private void InitializeChatHtmlTemplate()
        {
            _chatHtmlContent.Clear();
            _chatHtmlContent.Append(@"
<html>
<head>
<style>
    body {
        font-family: 'Segoe UI', sans-serif;
        background: linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%);
        margin: 0;
        padding: 20px;
        min-height: 100%;
    }
    .message-container {
        display: flex;
        flex-direction: column;
        gap: 15px;
    }
    .message {
        max-width: 75%;
        padding: 15px 20px;
        border-radius: 20px;
        animation: fadeIn 0.3s ease-out;
        box-shadow: 0 4px 15px rgba(0,0,0,0.2);
    }
    .user-message {
        align-self: flex-end;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        border-bottom-right-radius: 5px;
    }
    .bot-message {
        align-self: flex-start;
        background: rgba(255,255,255,0.1);
        backdrop-filter: blur(10px);
        color: #e0e0e0;
        border: 1px solid rgba(255,255,255,0.1);
        border-bottom-left-radius: 5px;
    }
    .message-time {
        font-size: 11px;
        opacity: 0.7;
        margin-top: 8px;
        text-align: right;
    }
    .message-content {
        line-height: 1.5;
        word-wrap: break-word;
    }
    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(10px); }
        to { opacity: 1; transform: translateY(0); }
    }
    .typing-indicator {
        display: flex;
        gap: 5px;
        padding: 15px 20px;
    }
    .typing-dot {
        width: 8px;
        height: 8px;
        background: #667eea;
        border-radius: 50%;
        animation: typing 1.4s infinite;
    }
    .typing-dot:nth-child(2) { animation-delay: 0.2s; }
    .typing-dot:nth-child(3) { animation-delay: 0.4s; }
    @keyframes typing {
        0%, 60%, 100% { transform: translateY(0); }
        30% { transform: translateY(-10px); }
    }
</style>
</head>
<body>
<div class='message-container' id='messages'>
");
            UpdateChatHtml();
        }

        private void UpdateChatHtml()
        {
            var fullHtml = _chatHtmlContent.ToString() + "</div></body></html>";
            chatHtmlControl.HtmlTemplate.Template = fullHtml;
        }

        private void SetupEventHandlers()
        {
            sendButton.Click += (s, e) => SendCurrentMessage();

            messageInput.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    SendCurrentMessage();
                }
            };

            clearButton.Click += (s, e) =>
            {
                ClearHistoryRequested?.Invoke(this, EventArgs.Empty);
                ShowSettingsPopup();
            };
        }

        private void SendCurrentMessage()
        {
            var message = messageInput.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(message))
            {
                SendMessageRequested?.Invoke(this, message);
            }
        }

        private void ShowSettingsPopup()
        {
            using var settingsPopup = new SettingsPopup();
            settingsPopup.ShowDialog(this);
        }

        private void ApplyPremiumStyling()
        {
            // Premium görünüm ayarları - koyu tema
            this.BackColor = Color.FromArgb(26, 26, 46);
        }

        #region IMainView Implementation

        public void AddMessageToView(ChatMessage message)
        {
            if (InvokeRequired)
            {
                Invoke(() => AddMessageToView(message));
                return;
            }

            var isUser = message.Role == MessageRole.User;
            var cssClass = isUser ? "user-message" : "bot-message";
            var escapedContent = System.Web.HttpUtility.HtmlEncode(message.Content).Replace("\n", "<br>");

            _chatHtmlContent.AppendFormat(@"
<div class='message {0}'>
    <div class='message-content'>{1}</div>
    <div class='message-time'>{2}</div>
</div>
", cssClass, escapedContent, message.Timestamp.ToString("HH:mm"));

            _messageCount++;
            UpdateChatHtml();
        }

        public void ClearMessagesFromView()
        {
            if (InvokeRequired)
            {
                Invoke(ClearMessagesFromView);
                return;
            }

            _messageCount = 0;
            InitializeChatHtmlTemplate();
        }

        public void SetLoadingState(bool isLoading)
        {
            if (InvokeRequired)
            {
                Invoke(() => SetLoadingState(isLoading));
                return;
            }

            statusLabel.Text = isLoading ? "⏳ Yanıt bekleniyor..." : "🟢 Hazır";
            sendButton.Enabled = !isLoading;
        }

        public void ShowError(string message)
        {
            if (InvokeRequired)
            {
                Invoke(() => ShowError(message));
                return;
            }

            XtraMessageBox.Show(this, message, "❌ Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowInfo(string message)
        {
            if (InvokeRequired)
            {
                Invoke(() => ShowInfo(message));
                return;
            }

            XtraMessageBox.Show(this, message, "ℹ️ Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ClearInputBox()
        {
            if (InvokeRequired)
            {
                Invoke(ClearInputBox);
                return;
            }

            messageInput.Text = string.Empty;
        }

        public void SetSendButtonEnabled(bool enabled)
        {
            if (InvokeRequired)
            {
                Invoke(() => SetSendButtonEnabled(enabled));
                return;
            }

            sendButton.Enabled = enabled;
        }

        #endregion
    }
}
