using System;
using System.Windows.Forms;

namespace Ceviri_App
{
    public partial class Form1 : Form
    {
        // KAPSÜLLEME (ENCAPSULATION) ve DEPENDENCY INJECTION:
        // Form sınıfı, çeviri işleminin "nasıl" yapıldığını bilmez.
        // Sadece ITranslationService arayüzünü bilir ve onu kullanır.
        // Bu servis, dışarıdan (Constructor Injection) verilir.
        private readonly ITranslationService _translationService;

        // Constructor Injection (Yapıcı Metot Enjeksiyonu)
        // Program.cs içerisinde bu form oluşturulurken, uygun servis (Mock veya Online) buraya parametre olarak geçilir.
        public Form1(ITranslationService translationService)
        {
            InitializeComponent();
            
            // Servisi private değişkene atıyoruz.
            _translationService = translationService;

            // Dilleri yükle
            LoadLanguages();
        }

        private void LoadLanguages()
        {
            // Örnek diller
            string[] languages = { "English", "Turkish", "German", "French", "Spanish" };
            
            cmbFromLang.Items.AddRange(languages);
            cmbToLang.Items.AddRange(languages);

            // Varsayılan seçimler
            cmbFromLang.SelectedIndex = 0; // English
            cmbToLang.SelectedIndex = 1;   // Turkish
        }

        private void btnTranslate_Click(object sender, EventArgs e)
        {
            string text = txtInput.Text;
            string fromLang = cmbFromLang.SelectedItem?.ToString() ?? "";
            string toLang = cmbToLang.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Lütfen çevrilecek bir metin girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (fromLang == toLang)
            {
                MessageBox.Show("Kaynak ve hedef dil aynı olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // İŞ MANTIĞI ÇAĞRISI:
                // Form, işi kendisi yapmaz, servise devreder.
                string result = _translationService.Translate(text, fromLang, toLang);
                txtOutput.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtInput.Clear();
            txtOutput.Clear();
            txtInput.Focus();
        }
    }
}
