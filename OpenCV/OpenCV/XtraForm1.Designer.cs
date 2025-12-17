using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenCV
{
    partial class XtraForm1
    {
        private System.ComponentModel.IContainer components = null;

        // Modern Theme Colors
        public static class Theme
        {
            public static readonly Color Background = Color.FromArgb(30, 30, 40);
            public static readonly Color PanelBg = Color.FromArgb(40, 40, 50);
            public static readonly Color Accent = Color.FromArgb(0, 122, 204);
            public static readonly Color Text = Color.White;
        }

        private PanelControl leftPanel;
        private GroupControl controlsGroup;
        private SimpleButton btnLoadImage;
        private SimpleButton btnDetectFaces;
        private SimpleButton btnSave;
        
        private GroupControl settingsGroup;
        private LabelControl lblScaleFactor;
        private ZoomTrackBarControl trackBarScale;
        private LabelControl lblMinNeighbors;
        private ZoomTrackBarControl trackBarNeighbors;

        private PanelControl mainPanel;
        private PictureEdit pictureEdit;
        private LabelControl statusLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            // Form Settings
            this.Text = "📷 OpenCV Face Detection - Modern UI";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Appearance.BackColor = Theme.Background;
            this.Appearance.Options.UseBackColor = true;

            // Left Panel (Controls)
            this.leftPanel = new PanelControl();
            this.leftPanel.Dock = DockStyle.Left;
            this.leftPanel.Width = 300;
            this.leftPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.leftPanel.Appearance.BackColor = Theme.PanelBg;
            this.leftPanel.Appearance.Options.UseBackColor = true;
            this.leftPanel.Padding = new Padding(20);

            // Controls Group
            this.controlsGroup = new GroupControl();
            this.controlsGroup.Text = "İŞLEMLER";
            this.controlsGroup.Dock = DockStyle.Top;
            this.controlsGroup.Height = 220;
            this.controlsGroup.AppearanceCaption.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // Buttons
            this.btnLoadImage = CreateButton("📂 Resim Yükle", 40);
            this.btnDetectFaces = CreateButton("🔍 Yüzleri Bul", 90);
            this.btnSave = CreateButton("💾 Kaydet", 140);
            this.btnDetectFaces.Enabled = false;
            this.btnSave.Enabled = false;

            this.controlsGroup.Controls.AddRange(new Control[] { btnSave, btnDetectFaces, btnLoadImage });

            // Settings Group
            this.settingsGroup = new GroupControl();
            this.settingsGroup.Text = "AYARLAR";
            this.settingsGroup.Dock = DockStyle.Top;
            this.settingsGroup.Height = 200;
            this.settingsGroup.AppearanceCaption.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            
            // Settings - Scale Factor
            this.lblScaleFactor = new LabelControl();
            this.lblScaleFactor.Text = "Hassasiyet (Scale): 1.1";
            this.lblScaleFactor.Location = new Point(20, 40);
            
            this.trackBarScale = new ZoomTrackBarControl();
            this.trackBarScale.Location = new Point(20, 60);
            this.trackBarScale.Width = 240;
            this.trackBarScale.Properties.Minimum = 11; // 1.1
            this.trackBarScale.Properties.Maximum = 20; // 2.0
            this.trackBarScale.Value = 11;
            
            // Settings - Min Neighbors
            this.lblMinNeighbors = new LabelControl();
            this.lblMinNeighbors.Text = "Minimum Komşu (Neighbors): 4";
            this.lblMinNeighbors.Location = new Point(20, 110);

            this.trackBarNeighbors = new ZoomTrackBarControl();
            this.trackBarNeighbors.Location = new Point(20, 130);
            this.trackBarNeighbors.Width = 240;
            this.trackBarNeighbors.Properties.Minimum = 1;
            this.trackBarNeighbors.Properties.Maximum = 10;
            this.trackBarNeighbors.Value = 4;

            this.settingsGroup.Controls.AddRange(new Control[] { lblScaleFactor, trackBarScale, lblMinNeighbors, trackBarNeighbors });

            // Add Groups to Left Panel with spacers
            PanelControl spacer = new PanelControl { Height = 20, Dock = DockStyle.Top, BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder };
            spacer.Appearance.BackColor = Theme.PanelBg;
            spacer.Appearance.Options.UseBackColor = true;

            this.leftPanel.Controls.Add(this.settingsGroup);
            this.leftPanel.Controls.Add(spacer);
            this.leftPanel.Controls.Add(this.controlsGroup);


            // Main Panel (Image)
            this.mainPanel = new PanelControl();
            this.mainPanel.Dock = DockStyle.Fill;
            this.mainPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.mainPanel.Appearance.BackColor = Theme.Background;
            this.mainPanel.Padding = new Padding(20);

            this.pictureEdit = new PictureEdit();
            this.pictureEdit.Dock = DockStyle.Fill;
            this.pictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            this.pictureEdit.Properties.ShowMenu = false;
            this.pictureEdit.Properties.Appearance.BackColor = Color.FromArgb(20, 20, 25);
            this.pictureEdit.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;

            this.statusLabel = new LabelControl();
            this.statusLabel.Dock = DockStyle.Bottom;
            this.statusLabel.Height = 30;
            this.statusLabel.Text = "Hazır - Resim bekleniyor...";
            this.statusLabel.Appearance.Font = new Font("Segoe UI", 10);
            this.statusLabel.Appearance.ForeColor = Theme.Text;
            this.statusLabel.Padding = new Padding(10, 5, 0, 0);

            this.mainPanel.Controls.Add(this.pictureEdit);
            this.mainPanel.Controls.Add(this.statusLabel);

            // Add Panels to Form
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.leftPanel);
        }

        private SimpleButton CreateButton(string text, int yPos)
        {
            var btn = new SimpleButton();
            btn.Text = text;
            btn.Location = new Point(20, yPos);
            btn.Size = new Size(240, 40);
            btn.Appearance.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Appearance.BackColor = Theme.Accent;
            btn.Appearance.ForeColor = Color.White;
            btn.Appearance.Options.UseBackColor = true;
            btn.Appearance.Options.UseForeColor = true;
            btn.Appearance.Options.UseFont = true;
            btn.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            return btn;
        }
    }
}