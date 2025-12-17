using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace OOPNotepad
{
    // ABSTRACTION: Interface ile soyutlama
    public interface IDocument
    {
        string FilePath { get; set; }
        string Content { get; set; }
        bool IsDirty { get; set; }
        void New();
        void Open(string path);
        void Save();
        void SaveAs(string path);
    }

    // ABSTRACTION & ENCAPSULATION: Abstract sınıf
    public abstract class DocumentBase : IDocument
    {
        private string _filePath;
        private string _content;
        private bool _isDirty;

        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                IsDirty = true;
            }
        }

        public bool IsDirty
        {
            get => _isDirty;
            set => _isDirty = value;
        }

        public abstract void New();
        public abstract void Open(string path);
        public abstract void Save();
        public abstract void SaveAs(string path);
    }

    // INHERITANCE: Somut belge sınıfı
    public class TextDocument : DocumentBase
    {
        public override void New()
        {
            Content = string.Empty;
            FilePath = null;
            IsDirty = false;
        }

        public override void Open(string path)
        {
            if (File.Exists(path))
            {
                Content = File.ReadAllText(path);
                FilePath = path;
                IsDirty = false;
            }
        }

        public override void Save()
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                File.WriteAllText(FilePath, Content);
                IsDirty = false;
            }
            else
            {
                throw new InvalidOperationException("Dosya yolu belirtilmemiş. SaveAs kullanın.");
            }
        }

        public override void SaveAs(string path)
        {
            FilePath = path;
            Save();
        }
    }

    // ENCAPSULATION: Dosya işlemlerini yöneten sınıf
    public class FileOperations
    {
        private readonly IDocument _document;

        public FileOperations(IDocument document)
        {
            _document = document;
        }

        public bool CheckUnsavedChanges()
        {
            if (_document.IsDirty)
            {
                var result = MessageBox.Show(
                    "Değişiklikleri kaydetmek istiyor musunuz?",
                    "Kaydedilmemiş Değişiklikler",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    return SaveDocument();
                }
                return result != DialogResult.Cancel;
            }
            return true;
        }

        public bool SaveDocument()
        {
            if (string.IsNullOrEmpty(_document.FilePath))
            {
                return SaveDocumentAs();
            }

            try
            {
                _document.Save();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kaydetme hatası: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool SaveDocumentAs()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Metin Dosyaları (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*";
                dialog.DefaultExt = "txt";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _document.SaveAs(dialog.FileName);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Kaydetme hatası: {ex.Message}", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return false;
        }

        public bool OpenDocument()
        {
            if (!CheckUnsavedChanges()) return false;

            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Metin Dosyaları (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _document.Open(dialog.FileName);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Açma hatası: {ex.Message}", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return false;
        }

        public void NewDocument()
        {
            if (CheckUnsavedChanges())
            {
                _document.New();
            }
        }
    }

    // COMPOSITION: Ana form sınıfı
    public class MainNotepadForm : Form
    {
        // Menü kontrolleri
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem editMenu;
        private ToolStripMenuItem formatMenu;
        private ToolStripMenuItem helpMenu;

        // Dosya menüsü öğeleri
        private ToolStripMenuItem newMenuItem;
        private ToolStripMenuItem openMenuItem;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripMenuItem saveAsMenuItem;
        private ToolStripMenuItem exitMenuItem;

        // Düzenle menüsü öğeleri
        private ToolStripMenuItem undoMenuItem;
        private ToolStripMenuItem redoMenuItem;
        private ToolStripMenuItem cutMenuItem;
        private ToolStripMenuItem copyMenuItem;
        private ToolStripMenuItem pasteMenuItem;
        private ToolStripMenuItem selectAllMenuItem;
        private ToolStripMenuItem findMenuItem;
        private ToolStripMenuItem replaceMenuItem;

        // Format menüsü öğeleri
        private ToolStripMenuItem fontMenuItem;
        private ToolStripMenuItem wordWrapMenuItem;

        // Hakkında menüsü öğeleri
        private ToolStripMenuItem aboutMenuItem;

        // Toolbar
        private ToolStrip toolStrip;
        private ToolStripButton newButton;
        private ToolStripButton openButton;
        private ToolStripButton saveButton;
        private ToolStripSeparator separator1;
        private ToolStripButton cutButton;
        private ToolStripButton copyButton;
        private ToolStripButton pasteButton;
        private ToolStripSeparator separator2;
        private ToolStripButton undoButton;
        private ToolStripButton redoButton;

        // Ana text editör
        private RichTextBox richTextBox;

        // Status bar
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolStripStatusLabel lineColLabel;

        // Belge yönetimi
        private readonly TextDocument _document;
        private readonly FileOperations _fileOps;

        public MainNotepadForm()
        {
            _document = new TextDocument();
            _fileOps = new FileOperations(_document);

            InitializeComponents();
            SetupEvents();
            UpdateTitle();
        }

        private void InitializeComponents()
        {
            // Form ayarları
            this.Text = "OOP Notepad";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = SystemIcons.Application;

            // MenuStrip oluşturma
            CreateMenuStrip();

            // ToolStrip oluşturma
            CreateToolStrip();

            // RichTextBox oluşturma
            richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 11F),
                BorderStyle = BorderStyle.None,
                AcceptsTab = true,
                DetectUrls = true,
                EnableAutoDragDrop = true,
                ScrollBars = RichTextBoxScrollBars.Both,
                WordWrap = true
            };

            // StatusStrip oluşturma
            CreateStatusStrip();

            // Form'a kontrolleri ekleme
            this.Controls.Add(richTextBox);
            this.Controls.Add(toolStrip);
            this.Controls.Add(menuStrip);
            this.Controls.Add(statusStrip);

            // MainMenuStrip ayarı
            this.MainMenuStrip = menuStrip;
        }

        private void CreateMenuStrip()
        {
            menuStrip = new MenuStrip();

            // DOSYA MENÜSÜ
            fileMenu = new ToolStripMenuItem("&Dosya");

            newMenuItem = new ToolStripMenuItem("&Yeni", null, null, Keys.Control | Keys.N);
            newMenuItem.Image = ResizeImage(SystemIcons.Application.ToBitmap(), 16, 16);

            openMenuItem = new ToolStripMenuItem("&Aç...", null, null, Keys.Control | Keys.O);
            openMenuItem.Image = ResizeImage(SystemIcons.Shield.ToBitmap(), 16, 16);

            saveMenuItem = new ToolStripMenuItem("&Kaydet", null, null, Keys.Control | Keys.S);
            saveMenuItem.Image = ResizeImage(SystemIcons.WinLogo.ToBitmap(), 16, 16);

            saveAsMenuItem = new ToolStripMenuItem("Farklı Ka&ydet...", null, null, Keys.Control | Keys.Shift | Keys.S);

            exitMenuItem = new ToolStripMenuItem("Çı&kış", null, null, Keys.Alt | Keys.F4);

            fileMenu.DropDownItems.AddRange(new ToolStripItem[] {
                newMenuItem,
                openMenuItem,
                new ToolStripSeparator(),
                saveMenuItem,
                saveAsMenuItem,
                new ToolStripSeparator(),
                exitMenuItem
            });

            // DÜZENLE MENÜSÜ
            editMenu = new ToolStripMenuItem("&Düzenle");

            undoMenuItem = new ToolStripMenuItem("&Geri Al", null, null, Keys.Control | Keys.Z);
            redoMenuItem = new ToolStripMenuItem("&Yinele", null, null, Keys.Control | Keys.Y);
            cutMenuItem = new ToolStripMenuItem("Ke&s", null, null, Keys.Control | Keys.X);
            copyMenuItem = new ToolStripMenuItem("&Kopyala", null, null, Keys.Control | Keys.C);
            pasteMenuItem = new ToolStripMenuItem("Ya&pıştır", null, null, Keys.Control | Keys.V);
            selectAllMenuItem = new ToolStripMenuItem("&Tümünü Seç", null, null, Keys.Control | Keys.A);
            findMenuItem = new ToolStripMenuItem("&Bul...", null, null, Keys.Control | Keys.F);
            replaceMenuItem = new ToolStripMenuItem("&Değiştir...", null, null, Keys.Control | Keys.H);

            editMenu.DropDownItems.AddRange(new ToolStripItem[] {
                undoMenuItem,
                redoMenuItem,
                new ToolStripSeparator(),
                cutMenuItem,
                copyMenuItem,
                pasteMenuItem,
                new ToolStripSeparator(),
                selectAllMenuItem,
                new ToolStripSeparator(),
                findMenuItem,
                replaceMenuItem
            });

            // FORMAT MENÜSÜ
            formatMenu = new ToolStripMenuItem("F&ormat");

            fontMenuItem = new ToolStripMenuItem("&Yazı Tipi...");
            wordWrapMenuItem = new ToolStripMenuItem("&Sözcük Kaydırma");
            wordWrapMenuItem.Checked = true;
            wordWrapMenuItem.CheckOnClick = true;

            formatMenu.DropDownItems.AddRange(new ToolStripItem[] {
                fontMenuItem,
                wordWrapMenuItem
            });

            // YARDIM MENÜSÜ
            helpMenu = new ToolStripMenuItem("&Yardım");
            aboutMenuItem = new ToolStripMenuItem("&Hakkında");

            helpMenu.DropDownItems.Add(aboutMenuItem);

            // Menü çubuğuna ekleme
            menuStrip.Items.AddRange(new ToolStripItem[] {
                fileMenu,
                editMenu,
                formatMenu,
                helpMenu
            });
        }

        private void CreateToolStrip()
        {
            toolStrip = new ToolStrip();
            toolStrip.ImageScalingSize = new Size(24, 24);

            newButton = new ToolStripButton("Yeni");
            newButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            newButton.Image = CreateNewIcon();
            newButton.ToolTipText = "Yeni Belge (Ctrl+N)";

            openButton = new ToolStripButton("Aç");
            openButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            openButton.Image = CreateOpenIcon();
            openButton.ToolTipText = "Dosya Aç (Ctrl+O)";

            saveButton = new ToolStripButton("Kaydet");
            saveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            saveButton.Image = CreateSaveIcon();
            saveButton.ToolTipText = "Kaydet (Ctrl+S)";

            separator1 = new ToolStripSeparator();

            cutButton = new ToolStripButton("Kes");
            cutButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            cutButton.Image = CreateCutIcon();
            cutButton.ToolTipText = "Kes (Ctrl+X)";

            copyButton = new ToolStripButton("Kopyala");
            copyButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            copyButton.Image = CreateCopyIcon();
            copyButton.ToolTipText = "Kopyala (Ctrl+C)";

            pasteButton = new ToolStripButton("Yapıştır");
            pasteButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            pasteButton.Image = CreatePasteIcon();
            pasteButton.ToolTipText = "Yapıştır (Ctrl+V)";

            separator2 = new ToolStripSeparator();

            undoButton = new ToolStripButton("Geri Al");
            undoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            undoButton.Image = CreateUndoIcon();
            undoButton.ToolTipText = "Geri Al (Ctrl+Z)";

            redoButton = new ToolStripButton("Yinele");
            redoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            redoButton.Image = CreateRedoIcon();
            redoButton.ToolTipText = "Yinele (Ctrl+Y)";

            toolStrip.Items.AddRange(new ToolStripItem[] {
                newButton,
                openButton,
                saveButton,
                separator1,
                cutButton,
                copyButton,
                pasteButton,
                separator2,
                undoButton,
                redoButton
            });
        }

        // Özel ikonlar oluşturma metotları
        private Bitmap CreateNewIcon()
        {
            var bmp = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Beyaz sayfa
                g.FillRectangle(Brushes.White, 4, 2, 14, 20);
                g.DrawRectangle(new Pen(Color.DarkGray, 1.5f), 4, 2, 14, 20);

                // Köşe kıvrımı
                g.FillPolygon(Brushes.LightGray, new Point[] {
                    new Point(14, 2), new Point(18, 6), new Point(14, 6)
                });
                g.DrawLine(new Pen(Color.DarkGray, 1.5f), 14, 2, 18, 6);

                // Artı işareti
                g.DrawLine(new Pen(Color.Green, 2f), 12, 18, 20, 18);
                g.DrawLine(new Pen(Color.Green, 2f), 16, 14, 16, 22);
            }
            return bmp;
        }

        private Bitmap CreateOpenIcon()
        {
            var bmp = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Klasör
                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 230, 150)), 2, 8, 20, 12);
                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 200, 100)), 2, 6, 10, 3);
                g.DrawRectangle(new Pen(Color.DarkGoldenrod, 1.5f), 2, 8, 20, 12);
            }
            return bmp;
        }

        private Bitmap CreateSaveIcon()
        {
            var bmp = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Disket şekli
                g.FillRectangle(new SolidBrush(Color.FromArgb(100, 150, 255)), 3, 2, 18, 20);
                g.DrawRectangle(new Pen(Color.DarkBlue, 1.5f), 3, 2, 18, 20);

                // Üst etiket
                g.FillRectangle(Brushes.White, 5, 4, 14, 6);
                g.DrawRectangle(Pens.DarkBlue, 5, 4, 14, 6);

                // Alt düğme
                g.FillRectangle(new SolidBrush(Color.FromArgb(70, 120, 200)), 5, 14, 14, 6);
            }
            return bmp;
        }

        private Bitmap CreateCutIcon()
        {
            var bmp = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Makas
                var pen = new Pen(Color.DarkRed, 2f);

                // Sol halka
                g.DrawEllipse(pen, 2, 2, 6, 6);
                // Sağ halka
                g.DrawEllipse(pen, 2, 16, 6, 6);

                // Keskin uçlar
                g.DrawLine(pen, 6, 5, 18, 12);
                g.DrawLine(pen, 6, 19, 18, 12);

                // Kesik çizgi
                using (var dashedPen = new Pen(Color.Gray, 1f))
                {
                    dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    g.DrawLine(dashedPen, 18, 12, 22, 12);
                }
            }
            return bmp;
        }

        private Bitmap CreateCopyIcon()
        {
            var bmp = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Arka belge (gölge)
                g.FillRectangle(new SolidBrush(Color.FromArgb(200, 200, 200)), 8, 4, 14, 16);
                g.DrawRectangle(new Pen(Color.Gray, 1.5f), 8, 4, 14, 16);

                // Ön belge
                g.FillRectangle(Brushes.White, 2, 8, 14, 16);
                g.DrawRectangle(new Pen(Color.Blue, 1.5f), 2, 8, 14, 16);

                // Çizgiler
                g.DrawLine(new Pen(Color.LightBlue, 1f), 4, 12, 12, 12);
                g.DrawLine(new Pen(Color.LightBlue, 1f), 4, 15, 12, 15);
                g.DrawLine(new Pen(Color.LightBlue, 1f), 4, 18, 12, 18);
            }
            return bmp;
        }

        private Bitmap CreatePasteIcon()
        {
            var bmp = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Pano
                g.FillRectangle(new SolidBrush(Color.FromArgb(150, 150, 150)), 6, 2, 12, 3);
                g.FillRectangle(Brushes.White, 4, 5, 16, 17);
                g.DrawRectangle(new Pen(Color.DarkGreen, 1.5f), 4, 5, 16, 17);

                // Klips
                g.FillRectangle(new SolidBrush(Color.Silver), 10, 2, 4, 4);

                // İçerik çizgileri
                g.DrawLine(new Pen(Color.LightGreen, 1f), 6, 9, 16, 9);
                g.DrawLine(new Pen(Color.LightGreen, 1f), 6, 12, 16, 12);
                g.DrawLine(new Pen(Color.LightGreen, 1f), 6, 15, 14, 15);
            }
            return bmp;
        }

        private Bitmap CreateUndoIcon()
        {
            var bmp = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Geri dönüş oku
                var pen = new Pen(Color.OrangeRed, 2.5f);
                pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

                g.DrawArc(pen, 6, 6, 12, 12, 90, 270);

                // Ok başı
                Point[] arrow = { new Point(6, 12), new Point(2, 8), new Point(6, 8) };
                g.FillPolygon(new SolidBrush(Color.OrangeRed), arrow);
            }
            return bmp;
        }

        private Bitmap CreateRedoIcon()
        {
            var bmp = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // İleri dönüş oku
                var pen = new Pen(Color.DodgerBlue, 2.5f);
                pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

                g.DrawArc(pen, 6, 6, 12, 12, 0, 270);

                // Ok başı
                Point[] arrow = { new Point(18, 12), new Point(22, 8), new Point(18, 8) };
                g.FillPolygon(new SolidBrush(Color.DodgerBlue), arrow);
            }
            return bmp;
        }

        private void CreateStatusStrip()
        {
            statusStrip = new StatusStrip();

            statusLabel = new ToolStripStatusLabel("Hazır");
            statusLabel.Spring = true;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;

            lineColLabel = new ToolStripStatusLabel("Satır: 1, Sütun: 1");
            lineColLabel.BorderSides = ToolStripStatusLabelBorderSides.Left;

            statusStrip.Items.AddRange(new ToolStripItem[] {
                statusLabel,
                lineColLabel
            });
        }

        private Image ResizeImage(Image img, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, width, height);
            }
            return bitmap;
        }

        private void SetupEvents()
        {
            // Dosya menüsü olayları
            newMenuItem.Click += (s, e) => NewDocument();
            newButton.Click += (s, e) => NewDocument();

            openMenuItem.Click += (s, e) => OpenDocument();
            openButton.Click += (s, e) => OpenDocument();

            saveMenuItem.Click += (s, e) => SaveDocument();
            saveButton.Click += (s, e) => SaveDocument();

            saveAsMenuItem.Click += (s, e) => SaveDocumentAs();
            exitMenuItem.Click += (s, e) => Close();

            // Düzenle menüsü olayları
            undoMenuItem.Click += (s, e) => richTextBox.Undo();
            undoButton.Click += (s, e) => richTextBox.Undo();

            redoMenuItem.Click += (s, e) => richTextBox.Redo();
            redoButton.Click += (s, e) => richTextBox.Redo();

            cutMenuItem.Click += (s, e) => richTextBox.Cut();
            cutButton.Click += (s, e) => richTextBox.Cut();

            copyMenuItem.Click += (s, e) => richTextBox.Copy();
            copyButton.Click += (s, e) => richTextBox.Copy();

            pasteMenuItem.Click += (s, e) => richTextBox.Paste();
            pasteButton.Click += (s, e) => richTextBox.Paste();

            selectAllMenuItem.Click += (s, e) => richTextBox.SelectAll();

            findMenuItem.Click += (s, e) => ShowFindDialog();
            replaceMenuItem.Click += (s, e) => ShowReplaceDialog();

            // Format menüsü olayları
            fontMenuItem.Click += (s, e) => ChangeFontDialog();
            wordWrapMenuItem.CheckedChanged += (s, e) =>
            {
                richTextBox.WordWrap = wordWrapMenuItem.Checked;
            };

            // Hakkında menüsü
            aboutMenuItem.Click += (s, e) => ShowAboutDialog();

            // TextBox olayları
            richTextBox.TextChanged += (s, e) =>
            {
                _document.Content = richTextBox.Text;
                UpdateTitle();
                UpdateStatusBar();
            };

            richTextBox.SelectionChanged += (s, e) => UpdateStatusBar();

            // Form olayları
            this.FormClosing += MainForm_FormClosing;
        }

        private void NewDocument()
        {
            _fileOps.NewDocument();
            richTextBox.Clear();
            UpdateTitle();
        }

        private void OpenDocument()
        {
            if (_fileOps.OpenDocument())
            {
                richTextBox.Text = _document.Content;
                UpdateTitle();
            }
        }

        private void SaveDocument()
        {
            _document.Content = richTextBox.Text;
            if (_fileOps.SaveDocument())
            {
                UpdateTitle();
                statusLabel.Text = "Dosya kaydedildi";
            }
        }

        private void SaveDocumentAs()
        {
            _document.Content = richTextBox.Text;
            if (_fileOps.SaveDocumentAs())
            {
                UpdateTitle();
                statusLabel.Text = "Dosya kaydedildi";
            }
        }

        private void ChangeFontDialog()
        {
            using (var fontDialog = new FontDialog())
            {
                fontDialog.Font = richTextBox.Font;
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBox.Font = fontDialog.Font;
                }
            }
        }

        private void ShowFindDialog()
        {
            MessageBox.Show("Bul özelliği yakında eklenecek!", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowReplaceDialog()
        {
            MessageBox.Show("Değiştir özelliği yakında eklenecek!", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAboutDialog()
        {
            MessageBox.Show(
                "OOP Notepad v1.0\n\n" +
                "C# Windows Forms ile geliştirilmiştir.\n" +
                "OOP prensipleri kullanılarak kodlanmıştır.\n\n" +
                "© 2024 - Tüm hakları saklıdır.",
                "Hakkında",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void UpdateTitle()
        {
            string fileName = string.IsNullOrEmpty(_document.FilePath)
                ? "Adsız"
                : Path.GetFileName(_document.FilePath);
            string dirty = _document.IsDirty ? "*" : "";
            this.Text = $"{fileName}{dirty} - OOP Notepad";
        }

        private void UpdateStatusBar()
        {
            int line = richTextBox.GetLineFromCharIndex(richTextBox.SelectionStart) + 1;
            int column = richTextBox.SelectionStart -
                         richTextBox.GetFirstCharIndexFromLine(line - 1) + 1;

            lineColLabel.Text = $"Satır: {line}, Sütun: {column}";

            if (richTextBox.Text.Length > 0)
            {
                statusLabel.Text = $"Karakter: {richTextBox.Text.Length}";
            }
            else
            {
                statusLabel.Text = "Hazır";
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _document.Content = richTextBox.Text;
            if (!_fileOps.CheckUnsavedChanges())
            {
                e.Cancel = true;
            }
        }
    }

    // Program giriş noktası
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainNotepadForm());
        }
    }
}