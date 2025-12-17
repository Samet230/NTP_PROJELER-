using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatSocketApp
{
    partial class XtraForm1
    {
        private System.ComponentModel.IContainer components = null;

        // Renk şeması - Modern Dark Theme
        private static class Theme
        {
            public static readonly Color Background = Color.FromArgb(26, 26, 46);        // #1a1a2e
            public static readonly Color PanelBg = Color.FromArgb(22, 33, 62);           // #16213e
            public static readonly Color Accent = Color.FromArgb(15, 52, 96);            // #0f3460
            public static readonly Color AccentHover = Color.FromArgb(25, 72, 126);      // Hover state
            public static readonly Color TextPrimary = Color.FromArgb(232, 232, 232);    // #e8e8e8
            public static readonly Color TextSecondary = Color.FromArgb(160, 160, 160);  // #a0a0a0
            public static readonly Color Online = Color.FromArgb(78, 204, 163);          // #4ecca3
            public static readonly Color Offline = Color.FromArgb(200, 80, 80);          // Red
            public static readonly Color MyMessage = Color.FromArgb(0, 132, 255);        // #0084ff
            public static readonly Color OtherMessage = Color.FromArgb(48, 48, 48);      // #303030
            public static readonly Color Border = Color.FromArgb(60, 60, 80);
            public static readonly Color InputBg = Color.FromArgb(35, 35, 55);
        }

        // Sol Panel
        private PanelControl leftPanel;
        private GroupControl serverGroup;
        private LabelControl serverIpLabel;
        private TextEdit serverTextEdit;
        private LabelControl portLabel;
        private SpinEdit portSpinEdit;
        private SimpleButton serverStartButton;
        private SimpleButton serverStopButton;
        private LabelControl serverStatusLabel;
        private LabelControl clientCountLabel;

        private GroupControl connectionGroup;
        private LabelControl usernameLabel;
        private TextEdit usernameTextEdit;
        private SimpleButton connectButton;
        private SimpleButton disconnectButton;
        private LabelControl connectionStatusLabel;

        private GroupControl usersGroup;
        private ListBoxControl usersListControl;

        // Sağ Panel
        private PanelControl rightPanel;
        private GroupControl chatGroup;
        private ListBoxControl messageListControl;
        private PanelControl typingPanel;
        private LabelControl typingLabel;
        private PanelControl inputPanel;
        private MemoEdit messageTextEdit;
        private SimpleButton sendButton;
        private SimpleButton emojiButton;
        private SimpleButton clearButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form ayarları
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "XtraForm1";
            this.Text = "💬 Modern Chat Application";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Theme.Background;
            this.Load += new System.EventHandler(this.XtraForm1_Load);

            CreateLeftPanel();
            CreateRightPanel();

            // Panel'leri ekle
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.leftPanel);

            this.ResumeLayout(false);
        }

        private void CreateLeftPanel()
        {
            // Left Panel
            leftPanel = new PanelControl();
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = 300;
            leftPanel.Appearance.BackColor = Theme.PanelBg;
            leftPanel.Appearance.Options.UseBackColor = true;
            leftPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            leftPanel.Padding = new Padding(10);

            // ===== SERVER GROUP =====
            serverGroup = new GroupControl();
            serverGroup.Text = "🖥️ SUNUCU";
            serverGroup.Dock = DockStyle.Top;
            serverGroup.Height = 180;
            serverGroup.Appearance.BackColor = Theme.PanelBg;
            serverGroup.Appearance.ForeColor = Theme.TextPrimary;
            serverGroup.Appearance.Options.UseBackColor = true;
            serverGroup.Appearance.Options.UseForeColor = true;
            serverGroup.AppearanceCaption.ForeColor = Theme.TextPrimary;
            serverGroup.AppearanceCaption.Font = new Font("Segoe UI Semibold", 10F);
            serverGroup.Padding = new Padding(10);

            int y = 30;

            // Server IP
            serverIpLabel = CreateLabel("IP Adresi:", 15, y);
            serverGroup.Controls.Add(serverIpLabel);

            serverTextEdit = new TextEdit();
            serverTextEdit.Location = new Point(100, y - 3);
            serverTextEdit.Size = new Size(165, 24);
            serverTextEdit.EditValue = "127.0.0.1";
            ApplyTextEditStyle(serverTextEdit);
            serverGroup.Controls.Add(serverTextEdit);

            y += 35;

            // Port
            portLabel = CreateLabel("Port:", 15, y);
            serverGroup.Controls.Add(portLabel);

            portSpinEdit = new SpinEdit();
            portSpinEdit.Location = new Point(100, y - 3);
            portSpinEdit.Size = new Size(165, 24);
            portSpinEdit.Properties.MinValue = 1;
            portSpinEdit.Properties.MaxValue = 65535;
            portSpinEdit.Properties.IsFloatValue = false;
            portSpinEdit.EditValue = 5000;
            ApplySpinEditStyle(portSpinEdit);
            serverGroup.Controls.Add(portSpinEdit);

            y += 40;

            // Server buttons
            serverStartButton = CreateButton("▶ Başlat", 15, y, 120, 32);
            serverStartButton.Click += ServerStartButton_Click;
            serverGroup.Controls.Add(serverStartButton);

            serverStopButton = CreateButton("⏹ Durdur", 145, y, 120, 32);
            serverStopButton.Enabled = false;
            serverStopButton.Click += ServerStopButton_Click;
            serverGroup.Controls.Add(serverStopButton);

            y += 42;

            // Server status
            serverStatusLabel = new LabelControl();
            serverStatusLabel.Text = "⚫ Kapalı";
            serverStatusLabel.Location = new Point(15, y);
            serverStatusLabel.Appearance.ForeColor = Theme.TextSecondary;
            serverStatusLabel.Appearance.Font = new Font("Segoe UI", 9F);
            serverGroup.Controls.Add(serverStatusLabel);

            clientCountLabel = new LabelControl();
            clientCountLabel.Text = "";
            clientCountLabel.Location = new Point(150, y);
            clientCountLabel.Appearance.ForeColor = Theme.TextSecondary;
            clientCountLabel.Appearance.Font = new Font("Segoe UI", 9F);
            serverGroup.Controls.Add(clientCountLabel);

            // ===== CONNECTION GROUP =====
            connectionGroup = new GroupControl();
            connectionGroup.Text = "🔌 BAĞLANTI";
            connectionGroup.Dock = DockStyle.Top;
            connectionGroup.Height = 160;
            connectionGroup.Appearance.BackColor = Theme.PanelBg;
            connectionGroup.Appearance.ForeColor = Theme.TextPrimary;
            connectionGroup.Appearance.Options.UseBackColor = true;
            connectionGroup.Appearance.Options.UseForeColor = true;
            connectionGroup.AppearanceCaption.ForeColor = Theme.TextPrimary;
            connectionGroup.AppearanceCaption.Font = new Font("Segoe UI Semibold", 10F);
            connectionGroup.Padding = new Padding(10);

            y = 30;

            // Username
            usernameLabel = CreateLabel("Kullanıcı Adı:", 15, y);
            connectionGroup.Controls.Add(usernameLabel);

            usernameTextEdit = new TextEdit();
            usernameTextEdit.Location = new Point(100, y - 3);
            usernameTextEdit.Size = new Size(165, 24);
            ApplyTextEditStyle(usernameTextEdit);
            connectionGroup.Controls.Add(usernameTextEdit);

            y += 40;

            // Connection buttons
            connectButton = CreateButton("🔗 Bağlan", 15, y, 120, 32);
            connectButton.Click += ConnectButton_Click;
            connectionGroup.Controls.Add(connectButton);

            disconnectButton = CreateButton("❌ Kes", 145, y, 120, 32);
            disconnectButton.Enabled = false;
            disconnectButton.Click += DisconnectButton_Click;
            connectionGroup.Controls.Add(disconnectButton);

            y += 42;

            // Connection status
            connectionStatusLabel = new LabelControl();
            connectionStatusLabel.Text = "🔴 Bağlı Değil";
            connectionStatusLabel.Location = new Point(15, y);
            connectionStatusLabel.Appearance.ForeColor = Theme.Offline;
            connectionStatusLabel.Appearance.Font = new Font("Segoe UI Semibold", 9F);
            connectionGroup.Controls.Add(connectionStatusLabel);

            // ===== USERS GROUP =====
            usersGroup = new GroupControl();
            usersGroup.Text = "👥 KULLANICILAR";
            usersGroup.Dock = DockStyle.Fill;
            usersGroup.Appearance.BackColor = Theme.PanelBg;
            usersGroup.Appearance.ForeColor = Theme.TextPrimary;
            usersGroup.Appearance.Options.UseBackColor = true;
            usersGroup.Appearance.Options.UseForeColor = true;
            usersGroup.AppearanceCaption.ForeColor = Theme.TextPrimary;
            usersGroup.AppearanceCaption.Font = new Font("Segoe UI Semibold", 10F);

            usersListControl = new ListBoxControl();
            usersListControl.Dock = DockStyle.Fill;
            usersListControl.Appearance.BackColor = Theme.Accent;
            usersListControl.Appearance.ForeColor = Theme.TextPrimary;
            usersListControl.Appearance.Options.UseBackColor = true;
            usersListControl.Appearance.Options.UseForeColor = true;
            usersListControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            usersListControl.ItemHeight = 35;
            usersListControl.DoubleClick += UsersListControl_DoubleClick;
            usersGroup.Controls.Add(usersListControl);

            // Left panel'e grupları ekle (ters sırada - dock order)
            leftPanel.Controls.Add(usersGroup);
            leftPanel.Controls.Add(connectionGroup);
            leftPanel.Controls.Add(serverGroup);
        }

        private void CreateRightPanel()
        {
            // Right Panel
            rightPanel = new PanelControl();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Appearance.BackColor = Theme.Background;
            rightPanel.Appearance.Options.UseBackColor = true;
            rightPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            rightPanel.Padding = new Padding(10);

            // Chat Group
            chatGroup = new GroupControl();
            chatGroup.Text = "💬 SOHBET";
            chatGroup.Dock = DockStyle.Fill;
            chatGroup.Appearance.BackColor = Theme.Background;
            chatGroup.Appearance.ForeColor = Theme.TextPrimary;
            chatGroup.Appearance.Options.UseBackColor = true;
            chatGroup.Appearance.Options.UseForeColor = true;
            chatGroup.AppearanceCaption.ForeColor = Theme.TextPrimary;
            chatGroup.AppearanceCaption.Font = new Font("Segoe UI Semibold", 10F);

            // Message List - Owner Draw için hazırlık
            messageListControl = new ListBoxControl();
            messageListControl.Dock = DockStyle.Fill;
            messageListControl.Appearance.BackColor = Theme.Accent;
            messageListControl.Appearance.ForeColor = Theme.TextPrimary;
            messageListControl.Appearance.Options.UseBackColor = true;
            messageListControl.Appearance.Options.UseForeColor = true;
            messageListControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            messageListControl.ItemHeight = 45;
            messageListControl.HorizontalScrollbar = true;
            messageListControl.SelectionMode = SelectionMode.None;

            // Typing Panel
            typingPanel = new PanelControl();
            typingPanel.Dock = DockStyle.Bottom;
            typingPanel.Height = 25;
            typingPanel.Appearance.BackColor = Theme.Accent;
            typingPanel.Appearance.Options.UseBackColor = true;
            typingPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            typingPanel.Visible = false;

            typingLabel = new LabelControl();
            typingLabel.Text = "";
            typingLabel.Dock = DockStyle.Fill;
            typingLabel.Appearance.ForeColor = Theme.TextSecondary;
            typingLabel.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            typingLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            typingLabel.Padding = new Padding(10, 5, 0, 0);
            typingPanel.Controls.Add(typingLabel);

            // Input Panel
            inputPanel = new PanelControl();
            inputPanel.Dock = DockStyle.Bottom;
            inputPanel.Height = 100;
            inputPanel.Appearance.BackColor = Theme.PanelBg;
            inputPanel.Appearance.Options.UseBackColor = true;
            inputPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            inputPanel.Padding = new Padding(10);

            // Message TextEdit
            messageTextEdit = new MemoEdit();
            messageTextEdit.Location = new Point(12, 12);
            messageTextEdit.Size = new Size(inputPanel.Width - 170, 76);
            messageTextEdit.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            messageTextEdit.Properties.ScrollBars = ScrollBars.Vertical;
            ApplyMemoEditStyle(messageTextEdit);
            messageTextEdit.KeyDown += MessageTextEdit_KeyDown;
            messageTextEdit.EditValueChanged += MessageTextEdit_EditValueChanged;
            inputPanel.Controls.Add(messageTextEdit);

            // Emoji Button
            emojiButton = new SimpleButton();
            emojiButton.Text = "😊";
            emojiButton.Location = new Point(inputPanel.Width - 150, 12);
            emojiButton.Size = new Size(40, 35);
            emojiButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ApplyButtonStyle(emojiButton);
            emojiButton.Click += EmojiButton_Click;
            inputPanel.Controls.Add(emojiButton);

            // Send Button
            sendButton = new SimpleButton();
            sendButton.Text = "📤 Gönder";
            sendButton.Location = new Point(inputPanel.Width - 150, 52);
            sendButton.Size = new Size(130, 35);
            sendButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            sendButton.Enabled = false;
            ApplyButtonStyle(sendButton);
            sendButton.Appearance.BackColor = Theme.MyMessage;
            sendButton.Click += SendButton_Click;
            inputPanel.Controls.Add(sendButton);

            // Clear Button
            clearButton = new SimpleButton();
            clearButton.Text = "🗑️";
            clearButton.Location = new Point(inputPanel.Width - 105, 12);
            clearButton.Size = new Size(40, 35);
            clearButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ApplyButtonStyle(clearButton);
            clearButton.Click += ClearButton_Click;
            inputPanel.Controls.Add(clearButton);

            // Chat group'a ekle
            chatGroup.Controls.Add(messageListControl);
            chatGroup.Controls.Add(typingPanel);
            chatGroup.Controls.Add(inputPanel);

            rightPanel.Controls.Add(chatGroup);
        }

        // Helper Methods
        private LabelControl CreateLabel(string text, int x, int y)
        {
            var label = new LabelControl();
            label.Text = text;
            label.Location = new Point(x, y);
            label.Appearance.ForeColor = Theme.TextSecondary;
            label.Appearance.Font = new Font("Segoe UI", 9F);
            return label;
        }

        private SimpleButton CreateButton(string text, int x, int y, int width, int height)
        {
            var button = new SimpleButton();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(width, height);
            ApplyButtonStyle(button);
            return button;
        }

        private void ApplyButtonStyle(SimpleButton button)
        {
            button.Appearance.BackColor = Theme.Accent;
            button.Appearance.ForeColor = Theme.TextPrimary;
            button.Appearance.BorderColor = Theme.Border;
            button.Appearance.Options.UseBackColor = true;
            button.Appearance.Options.UseForeColor = true;
            button.Appearance.Options.UseBorderColor = true;
            button.Appearance.Font = new Font("Segoe UI", 9F);
            button.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        }

        private void ApplyTextEditStyle(TextEdit textEdit)
        {
            textEdit.Properties.Appearance.BackColor = Theme.InputBg;
            textEdit.Properties.Appearance.ForeColor = Theme.TextPrimary;
            textEdit.Properties.Appearance.BorderColor = Theme.Border;
            textEdit.Properties.Appearance.Options.UseBackColor = true;
            textEdit.Properties.Appearance.Options.UseForeColor = true;
            textEdit.Properties.Appearance.Options.UseBorderColor = true;
            textEdit.Properties.AppearanceFocused.BackColor = Theme.Accent;
            textEdit.Properties.AppearanceFocused.ForeColor = Theme.TextPrimary;
            textEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        }

        private void ApplySpinEditStyle(SpinEdit spinEdit)
        {
            spinEdit.Properties.Appearance.BackColor = Theme.InputBg;
            spinEdit.Properties.Appearance.ForeColor = Theme.TextPrimary;
            spinEdit.Properties.Appearance.BorderColor = Theme.Border;
            spinEdit.Properties.Appearance.Options.UseBackColor = true;
            spinEdit.Properties.Appearance.Options.UseForeColor = true;
            spinEdit.Properties.Appearance.Options.UseBorderColor = true;
            spinEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        }

        private void ApplyMemoEditStyle(MemoEdit memoEdit)
        {
            memoEdit.Properties.Appearance.BackColor = Theme.InputBg;
            memoEdit.Properties.Appearance.ForeColor = Theme.TextPrimary;
            memoEdit.Properties.Appearance.BorderColor = Theme.Border;
            memoEdit.Properties.Appearance.Options.UseBackColor = true;
            memoEdit.Properties.Appearance.Options.UseForeColor = true;
            memoEdit.Properties.Appearance.Options.UseBorderColor = true;
            memoEdit.Properties.AppearanceFocused.BackColor = Theme.Accent;
            memoEdit.Properties.AppearanceFocused.ForeColor = Theme.TextPrimary;
            memoEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        }

        #endregion
    }
}