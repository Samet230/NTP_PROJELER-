using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatSocketApp
{
    public partial class XtraForm1 : XtraForm
    {
        // Connection state
        private Socket clientSocket;
        private string username;
        private bool isConnected = false;
        private readonly BindingList<string> messages = new BindingList<string>();
        private readonly List<string> onlineUsers = new List<string>();

        // Server control
        private Task serverTask;
        private CancellationTokenSource serverCts;
        private ChatServer chatServer;

        // Typing indicator
        private DateTime lastTypingNotification = DateTime.MinValue;
        private System.Windows.Forms.Timer typingTimer;
        private readonly Dictionary<string, DateTime> typingUsers = new Dictionary<string, DateTime>();

        // Auto-reconnect
        private int reconnectAttempts = 0;
        private const int MaxReconnectAttempts = 3;

        // Random names for quick start
        private static readonly string[] randomNames = {
            "Falcon", "Pixel", "Nova", "Shadow", "Ranger", "Dash", "Sky", "Jazz",
            "Blaze", "Echo", "Bolt", "Flash", "Violet", "Drake", "Maverick", "Quinn",
            "Phoenix", "Storm", "Titan", "Luna", "Orion", "Neon", "Cyber", "Matrix"
        };
        private static readonly Random rng = new Random();

        // Emojis for quick picker
        private static readonly string[] quickEmojis = {
            "😊", "😂", "❤️", "👍", "🎉", "🔥", "💯", "✨", 
            "😎", "🤔", "👋", "🙏", "💪", "🚀", "⭐", "🌟"
        };

        public XtraForm1()
        {
            InitializeComponent();
            InitializeTimers();
        }

        private void InitializeTimers()
        {
            // Typing indicator timer
            typingTimer = new System.Windows.Forms.Timer();
            typingTimer.Interval = 1000;
            typingTimer.Tick += TypingTimer_Tick;
            typingTimer.Start();
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
            try
            {
                // Assign random username
                if (usernameTextEdit != null && string.IsNullOrWhiteSpace(usernameTextEdit.Text))
                {
                    usernameTextEdit.Text = randomNames[rng.Next(randomNames.Length)] + rng.Next(100);
                }

                // Focus username
                usernameTextEdit?.Focus();

                // Initial UI state
                UpdateUIState();
            }
            catch { }
        }

        #region Server Controls

        private void ServerStartButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (serverTask != null && !serverTask.IsCompleted)
                {
                    XtraMessageBox.Show("Sunucu zaten çalışıyor!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                serverCts = new CancellationTokenSource();
                var token = serverCts.Token;
                int port = Convert.ToInt32(portSpinEdit.Value);

                chatServer = new ChatServer();
                chatServer.OnLog += (msg) =>
                {
                    this.BeginInvoke((Action)(() => AddSystemMessage(msg)));
                };
                chatServer.OnClientCountChanged += (count) =>
                {
                    this.BeginInvoke((Action)(() =>
                    {
                        if (clientCountLabel != null)
                            clientCountLabel.Text = $"👥 {count} kullanıcı";
                    }));
                };

                serverTask = Task.Run(() =>
                {
                    try
                    {
                        chatServer.StartServer(port, token);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception ex)
                    {
                        this.BeginInvoke((Action)(() =>
                            XtraMessageBox.Show($"Sunucu hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }
                }, token);

                serverStartButton.Enabled = false;
                serverStopButton.Enabled = true;
                serverStatusLabel.Text = "🟢 Çalışıyor";
                serverStatusLabel.Appearance.ForeColor = Color.FromArgb(78, 204, 163);
                portSpinEdit.Enabled = false;

                AddSystemMessage($"Sunucu {port} portunda başlatıldı");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Sunucu başlatılamadı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ServerStopButton_Click(object sender, EventArgs e)
        {
            try
            {
                serverCts?.Cancel();

                serverStartButton.Enabled = true;
                serverStopButton.Enabled = false;
                serverStatusLabel.Text = "⚫ Kapalı";
                serverStatusLabel.Appearance.ForeColor = Color.FromArgb(160, 160, 160);
                portSpinEdit.Enabled = true;
                clientCountLabel.Text = "";

                AddSystemMessage("Sunucu durduruldu");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Sunucu durdurulamadı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Connection Controls

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameTextEdit.Text))
            {
                usernameTextEdit.Text = randomNames[rng.Next(randomNames.Length)] + rng.Next(100);
            }

            username = usernameTextEdit.Text.Trim();
            reconnectAttempts = 0;

            Connect();
        }

        private void Connect()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endpoint = new IPEndPoint(IPAddress.Parse(serverTextEdit.Text.Trim()), Convert.ToInt32(portSpinEdit.Value));
                
                clientSocket.Connect(endpoint);
                isConnected = true;

                // Send connect message
                string connectMsg = $"CONNECT:{username}";
                clientSocket.Send(Encoding.UTF8.GetBytes(connectMsg));

                UpdateUIState();
                connectionStatusLabel.Text = "🟢 Bağlı";
                connectionStatusLabel.Appearance.ForeColor = Color.FromArgb(78, 204, 163);

                AddSystemMessage($"✅ {username} olarak bağlandınız");
                reconnectAttempts = 0;

                // Start receiving
                Task.Run(() => ReceiveMessages());
            }
            catch (Exception ex)
            {
                isConnected = false;
                
                if (reconnectAttempts < MaxReconnectAttempts)
                {
                    reconnectAttempts++;
                    AddSystemMessage($"⚠️ Bağlantı hatası. Yeniden deneniyor ({reconnectAttempts}/{MaxReconnectAttempts})...");
                    Task.Delay(2000).ContinueWith(_ => 
                    {
                        if (!isConnected) 
                            this.BeginInvoke((Action)Connect);
                    });
                }
                else
                {
                    XtraMessageBox.Show($"Bağlantı hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateUIState();
                }
            }
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void Disconnect()
        {
            if (clientSocket != null)
            {
                try { clientSocket.Shutdown(SocketShutdown.Both); } catch { }
                try { clientSocket.Close(); } catch { }
                clientSocket = null;
            }

            isConnected = false;
            onlineUsers.Clear();
            
            this.BeginInvoke((Action)(() =>
            {
                UpdateUIState();
                connectionStatusLabel.Text = "🔴 Bağlı Değil";
                connectionStatusLabel.Appearance.ForeColor = Color.FromArgb(200, 80, 80);
                usersListControl?.Items.Clear();
                typingPanel.Visible = false;

                AddSystemMessage($"❌ {username} bağlantısı kesildi");
            }));
        }

        private void UpdateUIState()
        {
            try
            {
                connectButton.Enabled = !isConnected;
                disconnectButton.Enabled = isConnected;
                sendButton.Enabled = isConnected;
                usernameTextEdit.Enabled = !isConnected;
                serverTextEdit.Enabled = !isConnected && !serverStopButton.Enabled;
            }
            catch { }
        }

        #endregion

        #region Message Handling

        private void SendButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            if (!isConnected || string.IsNullOrWhiteSpace(messageTextEdit.Text))
                return;

            try
            {
                string message = messageTextEdit.Text.Trim();
                string fullMsg = $"{username}: {message}";
                byte[] data = Encoding.UTF8.GetBytes(fullMsg);
                clientSocket.Send(data);

                AddMessage(username, message, true);
                messageTextEdit.Text = "";
                messageTextEdit.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Mesaj gönderme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MessageTextEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                if (sendButton.Enabled)
                    SendMessage();
            }
        }

        private void MessageTextEdit_EditValueChanged(object sender, EventArgs e)
        {
            // Send typing indicator
            if (isConnected && DateTime.Now - lastTypingNotification > TimeSpan.FromSeconds(2))
            {
                lastTypingNotification = DateTime.Now;
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes($"TYPING:{username}");
                    clientSocket?.Send(data);
                }
                catch { }
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[8192];

            try
            {
                while (isConnected && clientSocket != null && clientSocket.Connected)
                {
                    int bytesRead;
                    try
                    {
                        bytesRead = clientSocket.Receive(buffer);
                    }
                    catch
                    {
                        break;
                    }

                    if (bytesRead > 0)
                    {
                        string receivedMsg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        ProcessReceivedMessage(receivedMsg);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch { }
            finally
            {
                if (isConnected)
                {
                    this.BeginInvoke((Action)(() =>
                    {
                        if (reconnectAttempts < MaxReconnectAttempts)
                        {
                            reconnectAttempts++;
                            AddSystemMessage($"⚠️ Bağlantı koptu. Yeniden bağlanılıyor ({reconnectAttempts}/{MaxReconnectAttempts})...");
                            isConnected = false;
                            Task.Delay(2000).ContinueWith(_ => this.BeginInvoke((Action)Connect));
                        }
                        else
                        {
                            XtraMessageBox.Show("Sunucu bağlantısı kesildi!", "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Disconnect();
                        }
                    }));
                }
            }
        }

        private void ProcessReceivedMessage(string message)
        {
            this.BeginInvoke((Action)(() =>
            {
                try
                {
                    if (message.StartsWith("CONNECT:"))
                    {
                        string user = message.Substring(8).Trim();
                        if (user != username)
                        {
                            AddSystemMessage($"👋 {user} sohbete katıldı");
                            PlayNotificationSound();
                        }
                        if (!onlineUsers.Contains(user))
                        {
                            onlineUsers.Add(user);
                            RefreshUserList();
                        }
                    }
                    else if (message.StartsWith("DISCONNECT:"))
                    {
                        string user = message.Substring(11).Trim();
                        AddSystemMessage($"👋 {user} ayrıldı");
                        onlineUsers.Remove(user);
                        RefreshUserList();
                    }
                    else if (message.StartsWith("TYPING:"))
                    {
                        string user = message.Substring(7).Trim();
                        if (user != username)
                        {
                            typingUsers[user] = DateTime.Now;
                            UpdateTypingIndicator();
                        }
                    }
                    else if (message.StartsWith("USERLIST:"))
                    {
                        string userListStr = message.Substring(9);
                        onlineUsers.Clear();
                        if (!string.IsNullOrEmpty(userListStr))
                        {
                            foreach (var user in userListStr.Split(','))
                            {
                                if (!string.IsNullOrWhiteSpace(user))
                                    onlineUsers.Add(user.Trim());
                            }
                        }
                        RefreshUserList();
                    }
                    else if (message.StartsWith("PRIVATE:"))
                    {
                        // PRIVATE:sender:message
                        var parts = message.Substring(8).Split(new[] { ':' }, 2);
                        if (parts.Length >= 2)
                        {
                            AddPrivateMessage(parts[0], parts[1]);
                            PlayNotificationSound();
                        }
                    }
                    else if (message.Contains(":"))
                    {
                        string[] parts = message.Split(new[] { ':' }, 2);
                        if (parts.Length == 2)
                        {
                            string sender = parts[0].Trim();
                            string content = parts[1].Trim();
                            if (sender != username)
                            {
                                AddMessage(sender, content, false);
                                PlayNotificationSound();
                            }
                        }
                    }
                }
                catch { }
            }));
        }

        #endregion

        #region UI Updates

        private void AddMessage(string sender, string content, bool isFromMe)
        {
            string time = DateTime.Now.ToString("HH:mm");
            string prefix = isFromMe ? "➤" : "◀";
            string displayText = $"{time}  {prefix} {sender}: {content}";
            
            messageListControl.Items.Add(displayText);
            ScrollToBottom();
        }

        private void AddSystemMessage(string content)
        {
            string time = DateTime.Now.ToString("HH:mm");
            string displayText = $"{time}  ℹ️ {content}";
            
            messageListControl.Items.Add(displayText);
            ScrollToBottom();
        }

        private void AddPrivateMessage(string sender, string content)
        {
            string time = DateTime.Now.ToString("HH:mm");
            string displayText = $"{time}  🔒 [Özel] {sender}: {content}";
            
            messageListControl.Items.Add(displayText);
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            try
            {
                if (messageListControl.Items.Count > 0)
                    messageListControl.SelectedIndex = messageListControl.Items.Count - 1;
            }
            catch { }
        }

        private void RefreshUserList()
        {
            try
            {
                usersListControl.Items.Clear();
                foreach (var user in onlineUsers)
                {
                    string prefix = user == username ? "⭐" : "🟢";
                    usersListControl.Items.Add($"{prefix} {user}");
                }
                
                // Update group title
                usersGroup.Text = $"👥 KULLANICILAR ({onlineUsers.Count})";
            }
            catch { }
        }

        private void UpdateTypingIndicator()
        {
            var activeTypers = new List<string>();
            var now = DateTime.Now;

            foreach (var kvp in typingUsers)
            {
                if (now - kvp.Value < TimeSpan.FromSeconds(3))
                    activeTypers.Add(kvp.Key);
            }

            if (activeTypers.Count > 0)
            {
                string text = activeTypers.Count == 1
                    ? $"✍️ {activeTypers[0]} yazıyor..."
                    : $"✍️ {string.Join(", ", activeTypers)} yazıyor...";
                
                typingLabel.Text = text;
                typingPanel.Visible = true;
            }
            else
            {
                typingPanel.Visible = false;
            }
        }

        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            UpdateTypingIndicator();
        }

        private void PlayNotificationSound()
        {
            try
            {
                SystemSounds.Asterisk.Play();
            }
            catch { }
        }

        #endregion

        #region User Interaction

        private void UsersListControl_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (usersListControl.SelectedItem != null)
                {
                    string selectedUser = usersListControl.SelectedItem.ToString();
                    // Remove prefix (🟢 or ⭐)
                    selectedUser = selectedUser.Substring(2).Trim();
                    
                    if (selectedUser != username)
                    {
                        messageTextEdit.Text = $"@{selectedUser} ";
                        messageTextEdit.Focus();
                        messageTextEdit.SelectionStart = messageTextEdit.Text.Length;
                    }
                }
            }
            catch { }
        }

        private void EmojiButton_Click(object sender, EventArgs e)
        {
            // Create a simple emoji picker form
            using (var emojiForm = new XtraForm())
            {
                emojiForm.Text = "😊 Emoji Seç";
                emojiForm.Size = new Size(320, 180);
                emojiForm.StartPosition = FormStartPosition.CenterParent;
                emojiForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                emojiForm.BackColor = Color.FromArgb(26, 26, 46);

                var flowPanel = new FlowLayoutPanel();
                flowPanel.Dock = DockStyle.Fill;
                flowPanel.Padding = new Padding(10);
                flowPanel.BackColor = Color.FromArgb(26, 26, 46);

                foreach (var emoji in quickEmojis)
                {
                    var btn = new SimpleButton();
                    btn.Text = emoji;
                    btn.Size = new Size(50, 50);
                    btn.Appearance.Font = new Font("Segoe UI Emoji", 18F);
                    btn.Appearance.BackColor = Color.FromArgb(15, 52, 96);
                    btn.Appearance.ForeColor = Color.White;
                    btn.Appearance.Options.UseBackColor = true;
                    btn.Appearance.Options.UseForeColor = true;
                    btn.Appearance.Options.UseFont = true;
                    string selectedEmoji = emoji;
                    btn.Click += (s, args) =>
                    {
                        messageTextEdit.Text += selectedEmoji;
                        emojiForm.Close();
                    };
                    flowPanel.Controls.Add(btn);
                }

                emojiForm.Controls.Add(flowPanel);
                emojiForm.ShowDialog(this);
                
                messageTextEdit.Focus();
                messageTextEdit.SelectionStart = messageTextEdit.Text.Length;
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            messageListControl.Items.Clear();
            AddSystemMessage("🗑️ Mesajlar temizlendi");
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Disconnect();
            serverCts?.Cancel();
            typingTimer?.Stop();
            base.OnFormClosing(e);
        }
    }
}