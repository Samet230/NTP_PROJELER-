using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ChatBot_LLM.Infrastructure;
using ChatBot_LLM.Interfaces;
using ChatBot_LLM.Models;

namespace ChatBot_LLM.Views
{
    /// <summary>
    /// API Key ayarları popup formu
    /// </summary>
    public partial class SettingsPopup : XtraForm
    {
        private readonly ISettingsService _settingsService;

        // DevExpress Kontrolleri
        private GroupControl settingsGroup = null!;
        private TextEdit apiKeyInput = null!;
        private TextEdit modelNameInput = null!;
        private SpinEdit maxTokensInput = null!;
        private SpinEdit temperatureInput = null!;
        private SimpleButton saveButton = null!;
        private SimpleButton cancelButton = null!;
        private LabelControl apiKeyLabel = null!;
        private LabelControl modelLabel = null!;
        private LabelControl maxTokensLabel = null!;
        private LabelControl temperatureLabel = null!;

        public SettingsPopup()
        {
            _settingsService = ServiceContainer.Resolve<ISettingsService>();
            InitializeComponent();
            InitializeDevExpressControls();
            LoadCurrentSettings();
        }

        private void InitializeDevExpressControls()
        {
            // Form ayarları
            this.Text = "Ayarlar";
            this.Size = new Size(450, 350);
            this.MinimumSize = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Ana grup
            settingsGroup = new GroupControl
            {
                Text = "OpenAI API Ayarları",
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            // Layout paneli
            var layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 6,
                ColumnCount = 2,
                Padding = new Padding(10)
            };
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));

            // API Key
            apiKeyLabel = new LabelControl
            {
                Text = "API Key:",
                Anchor = AnchorStyles.Left
            };

            apiKeyInput = new TextEdit
            {
                Dock = DockStyle.Fill,
                Properties = 
                {
                    UseSystemPasswordChar = true,
                    NullText = "sk-..."
                }
            };

            // Model adı
            modelLabel = new LabelControl
            {
                Text = "Model:",
                Anchor = AnchorStyles.Left
            };

            modelNameInput = new TextEdit
            {
                Dock = DockStyle.Fill,
                Properties = 
                {
                    NullText = "gpt-3.5-turbo"
                }
            };

            // Max Tokens
            maxTokensLabel = new LabelControl
            {
                Text = "Max Tokens:",
                Anchor = AnchorStyles.Left
            };

            maxTokensInput = new SpinEdit
            {
                Dock = DockStyle.Fill,
                Properties = 
                {
                    MinValue = 100,
                    MaxValue = 8000,
                    IsFloatValue = false
                }
            };

            // Temperature
            temperatureLabel = new LabelControl
            {
                Text = "Temperature:",
                Anchor = AnchorStyles.Left
            };

            temperatureInput = new SpinEdit
            {
                Dock = DockStyle.Fill,
                Properties = 
                {
                    MinValue = 0,
                    MaxValue = 2,
                    Increment = 0.1M,
                    IsFloatValue = true
                }
            };

            // Butonlar paneli
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            saveButton = new SimpleButton
            {
                Text = "💾 Kaydet",
                Width = 100,
                Height = 35,
                Appearance = 
                {
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                }
            };

            cancelButton = new SimpleButton
            {
                Text = "❌ İptal",
                Width = 100,
                Height = 35,
                Margin = new Padding(0, 0, 10, 0)
            };

            // Buton event'leri
            saveButton.Click += SaveButton_Click;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(cancelButton);

            // Layout'a ekle
            layoutPanel.Controls.Add(apiKeyLabel, 0, 0);
            layoutPanel.Controls.Add(apiKeyInput, 1, 0);
            layoutPanel.Controls.Add(modelLabel, 0, 1);
            layoutPanel.Controls.Add(modelNameInput, 1, 1);
            layoutPanel.Controls.Add(maxTokensLabel, 0, 2);
            layoutPanel.Controls.Add(maxTokensInput, 1, 2);
            layoutPanel.Controls.Add(temperatureLabel, 0, 3);
            layoutPanel.Controls.Add(temperatureInput, 1, 3);
            layoutPanel.Controls.Add(buttonPanel, 1, 5);

            settingsGroup.Controls.Add(layoutPanel);
            this.Controls.Add(settingsGroup);
        }

        private void LoadCurrentSettings()
        {
            var settings = _settingsService.GetSettings();
            apiKeyInput.Text = settings.ApiKey;
            modelNameInput.Text = settings.ModelName;
            maxTokensInput.Value = settings.MaxTokens;
            temperatureInput.Value = (decimal)settings.Temperature;
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {
                var settings = new AppSettings
                {
                    ApiKey = apiKeyInput.Text?.Trim() ?? string.Empty,
                    ModelName = string.IsNullOrWhiteSpace(modelNameInput.Text) ? "gpt-3.5-turbo" : modelNameInput.Text.Trim(),
                    MaxTokens = (int)maxTokensInput.Value,
                    Temperature = (double)temperatureInput.Value
                };

                // Validasyon
                if (string.IsNullOrWhiteSpace(settings.ApiKey))
                {
                    XtraMessageBox.Show(this, "API Key boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _settingsService.SaveSettings(settings);
                XtraMessageBox.Show(this, "Ayarlar başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(this, $"Ayarlar kaydedilemedi: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
