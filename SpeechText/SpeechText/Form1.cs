using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Windows.Forms;

namespace SpeechText
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        private SpeechRecognitionEngine recognizer;
        private bool isListening = false;
        private Timer flashTimer; // Timer memory leak fix
        private bool isInitialized = false; // Initialization flag

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Speech Recognition Engine'i hazırla
            InitializeSpeechRecognition();
        }

        /// <summary>
        /// Speech Recognition motorunu başlat
        /// </summary>
        private void InitializeSpeechRecognition()
        {
            try
            {
                // Önce yüklü recognizer'ları kontrol et
                var installedRecognizers = SpeechRecognitionEngine.InstalledRecognizers();
                
                if (installedRecognizers.Count == 0)
                {
                    ShowNoRecognizerError();
                    return;
                }

                // Yüklü dilleri listele (debug için)
                string availableLanguages = string.Join(", ", 
                    installedRecognizers.Select(r => r.Culture.DisplayName));
                
                // Önce Türkçe'yi dene
                var turkishRecognizer = installedRecognizers
                    .FirstOrDefault(r => r.Culture.Name.StartsWith("tr"));
                
                // Türkçe yoksa İngilizce'yi dene
                var englishRecognizer = installedRecognizers
                    .FirstOrDefault(r => r.Culture.Name.StartsWith("en"));
                
                // Hangisi varsa onu kullan
                RecognizerInfo selectedRecognizer = turkishRecognizer ?? englishRecognizer ?? installedRecognizers[0];
                
                recognizer = new SpeechRecognitionEngine(selectedRecognizer);
                
                // Kullanılan dili göster
                string languageInfo = selectedRecognizer.Culture.DisplayName;

                recognizer.SetInputToDefaultAudioDevice();
                
                // Dictation grammar ile serbest konuşma tanıma
                Grammar grammar = new DictationGrammar();
                recognizer.LoadGrammar(grammar);
                
                // Event handlers
                recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
                recognizer.SpeechRecognitionRejected += Recognizer_SpeechRejected;
                recognizer.RecognizeCompleted += Recognizer_RecognizeCompleted;
                
                isInitialized = true;
                UpdateStatus($"⏸️ Hazır ({languageInfo})", false);
            }
            catch (InvalidOperationException ex)
            {
                // Mikrofon bulunamadı
                XtraMessageBox.Show(
                    $"Mikrofon bulunamadı veya erişilemiyor:\n{ex.Message}\n\n" +
                    "Lütfen mikrofonunuzun bağlı olduğundan ve izinlerin verildiğinden emin olun.",
                    "Mikrofon Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                btnStartStop.Enabled = false;
                UpdateStatus("❌ Mikrofon bulunamadı", false);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Ses tanıma motoru başlatılamadı:\n{ex.Message}",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                btnStartStop.Enabled = false;
                UpdateStatus("❌ Başlatma hatası", false);
            }
        }

        /// <summary>
        /// Yüklü recognizer bulunamadığında hata göster
        /// </summary>
        private void ShowNoRecognizerError()
        {
            string message = 
                "Sistemde yüklü bir konuşma tanıma motoru bulunamadı.\n\n" +
                "Çözüm için şu adımları izleyin:\n\n" +
                "1. Windows Ayarları'nı açın\n" +
                "2. Zaman ve Dil > Konuşma bölümüne gidin\n" +
                "3. 'Konuşma dili' altından bir dil seçin\n" +
                "4. 'İndir' butonuna tıklayın\n" +
                "5. İndirme tamamlandıktan sonra uygulamayı yeniden başlatın\n\n" +
                "Veya Denetim Masası > Konuşma Tanıma'dan ayarlayın.";
            
            XtraMessageBox.Show(
                message,
                "Konuşma Tanıma Kurulu Değil",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            
            btnStartStop.Enabled = false;
            UpdateStatus("❌ Dil paketi yüklü değil", false);
        }

        /// <summary>
        /// Konuşma tanındığında çağrılır
        /// </summary>
        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null && e.Result.Confidence > 0.3)
            {
                // UI thread kontrolü
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        AppendRecognizedText(e.Result.Text, e.Result.Confidence);
                    });
                }
                else
                {
                    AppendRecognizedText(e.Result.Text, e.Result.Confidence);
                }
            }
        }

        /// <summary>
        /// Tanınan metni ekle
        /// </summary>
        private void AppendRecognizedText(string text, float confidence)
        {
            // Mevcut metin boş değilse, bir boşluk ekle
            if (!string.IsNullOrEmpty(memoText.Text))
            {
                memoText.Text += " ";
            }
            
            memoText.Text += text;
            
            // Scroll en sona
            memoText.SelectionStart = memoText.Text.Length;
            memoText.ScrollToCaret();
            
            // Güven skoru ile görsel geribildirim
            string confidenceStr = (confidence * 100).ToString("F0");
            FlashStatus($"✅ Tanındı (%{confidenceStr}): \"{text}\"");
        }

        /// <summary>
        /// Konuşma reddedildiğinde (tanınamadığında) çağrılır
        /// </summary>
        private void Recognizer_SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    FlashStatus("⚠️ Anlaşılamadı, tekrar deneyin...");
                });
            }
            else
            {
                FlashStatus("⚠️ Anlaşılamadı, tekrar deneyin...");
            }
        }

        /// <summary>
        /// Tanıma tamamlandığında çağrılır
        /// </summary>
        private void Recognizer_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        HandleRecognitionError(e.Error);
                    });
                }
                else
                {
                    HandleRecognitionError(e.Error);
                }
            }
        }

        /// <summary>
        /// Tanıma hatasını işle
        /// </summary>
        private void HandleRecognitionError(Exception error)
        {
            isListening = false;
            ResetStartButton();
            UpdateStatus($"❌ Hata: {error.Message}", false);
        }

        /// <summary>
        /// Başlat/Durdur butonu
        /// </summary>
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (!isInitialized)
            {
                XtraMessageBox.Show(
                    "Konuşma tanıma motoru başlatılamadı.\nLütfen uygulamayı yeniden başlatın.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!isListening)
            {
                StartListening();
            }
            else
            {
                StopListening();
            }
        }

        /// <summary>
        /// Dinlemeyi başlat
        /// </summary>
        private void StartListening()
        {
            try
            {
                if (recognizer == null)
                {
                    XtraMessageBox.Show(
                        "Konuşma tanıma motoru kullanılamıyor.",
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                isListening = true;
                
                // Buton görünümünü güncelle - STOP görünümü
                btnStartStop.Text = "⏹️ Dinlemeyi Durdur";
                btnStartStop.Appearance.BackColor = Color.FromArgb(255, 152, 0); // Turuncu
                btnStartStop.AppearanceHovered.BackColor = Color.FromArgb(245, 124, 0);
                btnStartStop.AppearancePressed.BackColor = Color.FromArgb(230, 81, 0);
                
                UpdateStatus("🎙️ Dinleniyor... Konuşun!", true);
            }
            catch (InvalidOperationException ex)
            {
                XtraMessageBox.Show(
                    $"Dinleme zaten aktif veya başlatılamadı:\n{ex.Message}",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Dinleme başlatılamadı:\n{ex.Message}",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Dinlemeyi durdur
        /// </summary>
        private void StopListening()
        {
            try
            {
                if (recognizer != null)
                {
                    recognizer.RecognizeAsyncStop();
                }
                isListening = false;
                
                ResetStartButton();
                UpdateStatus("⏸️ Dinleme durduruldu", false);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Dinleme durdurulamadı:\n{ex.Message}",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Başlat butonunu sıfırla
        /// </summary>
        private void ResetStartButton()
        {
            btnStartStop.Text = "🎙️ Dinlemeyi Başlat";
            btnStartStop.Appearance.BackColor = Color.FromArgb(76, 175, 80); // Yeşil
            btnStartStop.AppearanceHovered.BackColor = Color.FromArgb(67, 160, 71);
            btnStartStop.AppearancePressed.BackColor = Color.FromArgb(56, 142, 60);
        }

        /// <summary>
        /// Temizle butonu
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(memoText.Text))
            {
                var result = XtraMessageBox.Show(
                    "Tüm metin silinecek. Emin misiniz?",
                    "Onay",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    memoText.Text = string.Empty;
                    FlashStatus("🗑️ Metin temizlendi");
                }
            }
            else
            {
                FlashStatus("ℹ️ Temizlenecek metin yok");
            }
        }

        /// <summary>
        /// Kopyala butonu
        /// </summary>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(memoText.Text))
            {
                try
                {
                    Clipboard.SetText(memoText.Text);
                    FlashStatus("📋 Metin panoya kopyalandı!");
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(
                        $"Panoya kopyalanamadı:\n{ex.Message}",
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else
            {
                FlashStatus("⚠️ Kopyalanacak metin yok");
            }
        }

        /// <summary>
        /// Durum mesajını güncelle
        /// </summary>
        private void UpdateStatus(string message, bool showProgress)
        {
            lblStatus.Text = message;
            progressIndicator.Visible = showProgress;
        }

        /// <summary>
        /// Geçici durum mesajı göster (Timer memory leak düzeltildi)
        /// </summary>
        private void FlashStatus(string message)
        {
            string originalStatus = isListening ? "🎙️ Dinleniyor... Konuşun!" : "⏸️ Hazır";
            lblStatus.Text = message;
            
            // Önceki timer'ı temizle
            if (flashTimer != null)
            {
                flashTimer.Stop();
                flashTimer.Dispose();
                flashTimer = null;
            }
            
            // 2 saniye sonra orijinal mesaja dön
            flashTimer = new Timer();
            flashTimer.Interval = 2000;
            flashTimer.Tick += (s, e) =>
            {
                lblStatus.Text = originalStatus;
                flashTimer.Stop();
            };
            flashTimer.Start();
        }

        /// <summary>
        /// Form kapanırken kaynakları temizle
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            
            // Timer'ı temizle
            if (flashTimer != null)
            {
                flashTimer.Stop();
                flashTimer.Dispose();
                flashTimer = null;
            }
            
            // Recognizer'ı temizle
            if (recognizer != null)
            {
                try
                {
                    if (isListening)
                    {
                        recognizer.RecognizeAsyncStop();
                    }
                    recognizer.SpeechRecognized -= Recognizer_SpeechRecognized;
                    recognizer.SpeechRecognitionRejected -= Recognizer_SpeechRejected;
                    recognizer.RecognizeCompleted -= Recognizer_RecognizeCompleted;
                    recognizer.Dispose();
                }
                catch
                {
                    // Dispose sırasında hata olursa yoksay
                }
                recognizer = null;
            }
        }
    }
}