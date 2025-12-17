using DevExpress.XtraEditors;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenCV
{
    public partial class XtraForm1 : XtraForm
    {
        private Image<Bgr, byte> currentImage;
        private Image<Bgr, byte> processedImage;
        private string originalFilePath;
        private CascadeClassifier faceClassifier;

        public XtraForm1()
        {
            InitializeComponent();
            InitializeEvents();
            LoadClassifier();
        }

        private void InitializeEvents()
        {
            btnLoadImage.Click += BtnLoadImage_Click;
            btnDetectFaces.Click += BtnDetectFaces_Click;
            btnSave.Click += BtnSave_Click;
            
            trackBarScale.ValueChanged += (s, e) => 
                lblScaleFactor.Text = $"Hassasiyet (Scale): {trackBarScale.Value / 10.0:0.0}";
                
            trackBarNeighbors.ValueChanged += (s, e) =>
                lblMinNeighbors.Text = $"Minimum Komşu (Neighbors): {trackBarNeighbors.Value}";
        }

        private void LoadClassifier()
        {
            try 
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml");
                if (File.Exists(path))
                {
                    faceClassifier = new CascadeClassifier(path);
                    statusLabel.Text = "✅ Haar Cascade yüklendi ve hazır.";
                }
                else
                {
                    statusLabel.Text = "❌ Haar Cascade dosyası bulunamadı!";
                    // Async context, don't show message box on constructor usually, but safe here
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = "❌ Başlatma hatası";
            }
        }

        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    originalFilePath = ofd.FileName;
                    try
                    {
                        var bmp = new Bitmap(originalFilePath);
                        currentImage = bmp.ToImage<Bgr, byte>();
                        pictureEdit.Image = bmp;
                        
                        btnDetectFaces.Enabled = true;
                        btnSave.Enabled = true;
                        
                        statusLabel.Text = $"📂 Resim yüklendi: {Path.GetFileName(originalFilePath)} ({bmp.Width}x{bmp.Height})";
                        statusLabel.Appearance.ForeColor = Theme.Text;
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show($"Resim yüklenemedi: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void BtnDetectFaces_Click(object sender, EventArgs e)
        {
            if (currentImage == null) return;
            
            if (faceClassifier == null)
            {
                 XtraMessageBox.Show("Haar Cascade yüklenemediği için işlem yapılamıyor.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 return;
            }

            btnDetectFaces.Enabled = false;
            statusLabel.Text = "⏳ Yüzler aranıyor...";
            
            double scaleFactor = trackBarScale.Value / 10.0;
            int minNeighbors = trackBarNeighbors.Value;

            try
            {
                await Task.Run(() =>
                {
                    using (var gray = currentImage.Convert<Gray, byte>())
                    {
                        var faces = faceClassifier.DetectMultiScale(gray, scaleFactor, minNeighbors, Size.Empty);
                        
                        processedImage = currentImage.Clone();
                        foreach (var face in faces)
                        {
                            processedImage.Draw(face, new Bgr(Color.FromArgb(0, 255, 0)), 3); // Yeşil çerçeve
                        }

                        this.BeginInvoke((Action)(() =>
                        {
                            pictureEdit.Image = processedImage.ToBitmap();
                            
                            int count = faces.Length;
                            statusLabel.Text = count > 0 
                                ? $"✅ {count} yüz tespit edildi!" 
                                : "⚠️ Yüz bulunamadı.";
                                
                            statusLabel.Appearance.ForeColor = count > 0 ? Color.LightGreen : Color.Orange;
                        }));
                    }
                });
            }
            catch (Exception ex)
            {
                statusLabel.Text = "❌ İşlem hatası";
                XtraMessageBox.Show($"Algılama hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDetectFaces.Enabled = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (pictureEdit.Image == null) return;

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "JPG Dosyası|*.jpg|PNG Dosyası|*.png";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    pictureEdit.Image.Save(sfd.FileName);
                    statusLabel.Text = $"💾 Kaydedildi: {Path.GetFileName(sfd.FileName)}";
                }
            }
        }
    }
}