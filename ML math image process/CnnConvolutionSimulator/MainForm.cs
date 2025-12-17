using System;
using System.Drawing;
using System.Windows.Forms;

namespace CnnConvolutionSimulator
{
    public partial class MainForm : Form
    {
        private Bitmap _sourceImage;
        private ConvolutionEngine _engine;

        public MainForm()
        {
            InitializeComponent();
            _engine = new ConvolutionEngine();
            cmbFilters.SelectedIndex = 0; // Varsayılan olarak ilk filtreyi seç
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Varsa önceki resmi temizle
                        if (_sourceImage != null) _sourceImage.Dispose();

                        // Yeni resmi yükle
                        // Dosya kilidini önlemek için kopyasını oluşturuyoruz
                        using (var temp = new Bitmap(ofd.FileName))
                        {
                            _sourceImage = new Bitmap(temp);
                        }

                        pbSource.Image = _sourceImage;
                        pbResult.Image = null; // Önceki sonucu temizle
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Resim yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (_sourceImage == null)
            {
                MessageBox.Show("Lütfen önce bir resim yükleyin.", "Resim Yok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Çift tıklamayı önlemek için butonu devre dışı bırak
            btnProcess.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                MatrixKernel kernel = null;

                // Factory Pattern: Kullanıcı seçimine göre ilgili çekirdeği oluştur
                switch (cmbFilters.SelectedIndex)
                {
                    case 0: // Kenar Algılama
                        kernel = MatrixKernel.EdgeDetection();
                        break;
                    case 1: // Keskinleştirme
                        kernel = MatrixKernel.Sharpen();
                        break;
                    case 2: // Gaussian Bulanıklaştırma
                        kernel = MatrixKernel.GaussianBlur();
                        break;
                    case 3: // Kutu Bulanıklaştırma
                        kernel = MatrixKernel.BoxBlur();
                        break;
                    case 4: // Kabartma (Emboss)
                        kernel = MatrixKernel.Emboss();
                        break;
                    case 5: // Sobel Yatay
                        kernel = MatrixKernel.SobelHorizontal();
                        break;
                    case 6: // Sobel Dikey
                        kernel = MatrixKernel.SobelVertical();
                        break;
                    default:
                        kernel = MatrixKernel.EdgeDetection();
                        break;
                }

                // Ağır işlemi gerçekleştir
                // Gerçek bir uygulamada UI donmasını önlemek için async/await kullanılmalıdır.
                // Ancak eğitim amaçlı bu simülasyonda senkron işlem daha anlaşılırdır.
                Bitmap result = _engine.ApplyFilter(_sourceImage, kernel);

                // Sonucu göster
                if (pbResult.Image != null) pbResult.Image.Dispose();
                pbResult.Image = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Resim işlenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnProcess.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (pbResult.Image == null)
            {
                MessageBox.Show("Kaydedilecek bir işlem sonucu yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Resmi|*.png|JPEG Resmi|*.jpg|Bitmap Resmi|*.bmp";
                sfd.Title = "İşlenmiş Resmi Kaydet";
                sfd.FileName = "sonuc.png";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Dosya uzantısına göre formatı belirle
                        System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
                        string ext = System.IO.Path.GetExtension(sfd.FileName).ToLower();
                        switch (ext)
                        {
                            case ".jpg":
                            case ".jpeg":
                                format = System.Drawing.Imaging.ImageFormat.Jpeg;
                                break;
                            case ".bmp":
                                format = System.Drawing.Imaging.ImageFormat.Bmp;
                                break;
                        }

                        pbResult.Image.Save(sfd.FileName, format);
                        MessageBox.Show("Resim başarıyla kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Kaydetme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnApplyCustom_Click(object sender, EventArgs e)
        {
            if (_sourceImage == null)
            {
                MessageBox.Show("Lütfen önce bir resim yükleyin.", "Resim Yok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                double[,] customKernel = new double[3, 3];

                // TextBox'lardan değerleri oku
                customKernel[0, 0] = double.Parse(txt00.Text);
                customKernel[0, 1] = double.Parse(txt01.Text);
                customKernel[0, 2] = double.Parse(txt02.Text);
                customKernel[1, 0] = double.Parse(txt10.Text);
                customKernel[1, 1] = double.Parse(txt11.Text);
                customKernel[1, 2] = double.Parse(txt12.Text);
                customKernel[2, 0] = double.Parse(txt20.Text);
                customKernel[2, 1] = double.Parse(txt21.Text);
                customKernel[2, 2] = double.Parse(txt22.Text);

                MatrixKernel kernel = new MatrixKernel(customKernel);

                // İşlemi başlat
                btnApplyCustom.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                Bitmap result = _engine.ApplyFilter(_sourceImage, kernel);

                if (pbResult.Image != null) pbResult.Image.Dispose();
                pbResult.Image = result;
            }
            catch (FormatException)
            {
                MessageBox.Show("Lütfen tüm kutulara geçerli sayısal değerler girin.", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnApplyCustom.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }
    }
}
