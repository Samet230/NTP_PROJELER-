using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Alerter;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using Antivirus.Core.Interfaces;
using Antivirus.Core.Models;
using Antivirus.Core.Services;

namespace Antivirus.UI.Forms;

/// <summary>
/// Main application form with DevExpress RibbonControl
/// DevExpress RibbonControl ile ana uygulama formu
/// </summary>
public partial class MainForm : RibbonForm
{
    // Services
    private readonly ISignatureDatabase _signatureDatabase;
    private readonly IQuarantineService _quarantineService;
    private readonly IScanEngine _scanEngine;
    private readonly LogService _logService;

    // Cancellation support
    private CancellationTokenSource? _scanCancellationTokenSource;

    // UI Components
    private RibbonControl _ribbonControl = null!;
    private RibbonPage _ribbonPageHome = null!;
    private RibbonPageGroup _ribbonGroupScan = null!;
    private RibbonPageGroup _ribbonGroupQuarantine = null!;
    private RibbonPageGroup _ribbonGroupSettings = null!;

    private BarButtonItem _btnQuickScan = null!;
    private BarButtonItem _btnFullScan = null!;
    private BarButtonItem _btnCustomScan = null!;
    private BarButtonItem _btnStopScan = null!;
    private BarButtonItem _btnQuarantine = null!;
    private BarButtonItem _btnUpdate = null!;

    private GridControl _gridControl = null!;
    private GridView _gridView = null!;
    private BindingSource _scanResultsBinding = null!;

    private LabelControl _lblProgressPercent = null!;
    private PanelControl _progressPanel = null!;

    private LabelControl _lblStatus = null!;
    private ProgressBarControl _progressBar = null!;
    private LabelControl _lblStats = null!;

    private AlertControl _alertControl = null!;

    // Data
    private readonly BindingList<ScanResultViewModel> _scanResults = new();
    private DevExpress.Utils.SvgImageCollection _statusIcons;
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    public MainForm()
    {
        // Initialize services
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var dataPath = Path.Combine(basePath, "data");
        var signatureFilePath = Path.Combine(dataPath, "signatures.json");
        var quarantinePath = Path.Combine(dataPath, "Quarantine");
        var logFilePath = Path.Combine(dataPath, "logs.json");

        _signatureDatabase = new SignatureDatabase(signatureFilePath);
        _quarantineService = new QuarantineService(quarantinePath);
        _scanEngine = new ScanEngine(_signatureDatabase, _quarantineService);
        _logService = new LogService(logFilePath);

        // Subscribe to events
        _scanEngine.ThreatDetected += OnThreatDetected;
        _scanEngine.ScanCompleted += OnScanCompleted;

        InitializeComponents();
        LoadInitialData();
    }

    private void InitializeComponents()
    {
        // Form settings
        this.Text = "Antivirüs Güvenlik Merkezi";
        this.Size = new Size(1280, 800);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(1024, 700);
        this.Icon = SystemIcons.Shield;

        InitializeRibbon();
        InitializeGrid();
        InitializeGrid();
        InitializeProgressPanel();
        InitializeStatusBar();
        InitializeStatusBar();
        InitializeAlertControl();
        LayoutComponents();
    }

    private void InitializeRibbon()
    {
        _ribbonControl = new RibbonControl();
        _ribbonControl.Dock = DockStyle.Top;

        // Ribbon Page - Home
        _ribbonPageHome = new RibbonPage("Ana Sayfa");

        // Scan Group
        _ribbonGroupScan = new RibbonPageGroup("Tarama");
        
        _btnQuickScan = new BarButtonItem
        {
            Caption = "Hızlı Tarama",
            ImageOptions = { SvgImage = GetSvgImage("scan_radar.svg") },
            RibbonStyle = RibbonItemStyles.Large
        };
        _btnQuickScan.ItemClick += BtnQuickScan_Click;

        _btnFullScan = new BarButtonItem
        {
            Caption = "Derin Tarama",
            ImageOptions = { SvgImage = GetSvgImage("shield_check.svg") },
            RibbonStyle = RibbonItemStyles.Large
        };
        _btnFullScan.ItemClick += BtnFullScan_Click;

        _btnCustomScan = new BarButtonItem
        {
            Caption = "Klasör Tara",
            ImageOptions = { SvgImage = GetSvgImage("custom_scan.svg") },
            RibbonStyle = RibbonItemStyles.Large
        };
        _btnCustomScan.ItemClick += BtnCustomScan_Click;

        _btnStopScan = new BarButtonItem
        {
            Caption = "Taramayı Durdur",
            Enabled = false,
            RibbonStyle = RibbonItemStyles.Large
        };
        _btnStopScan.ItemClick += BtnStopScan_Click;

        _ribbonGroupScan.ItemLinks.Add(_btnQuickScan);
        _ribbonGroupScan.ItemLinks.Add(_btnFullScan);
        _ribbonGroupScan.ItemLinks.Add(_btnCustomScan);
        
        // Single File Scan Button
        var btnFileScan = new BarButtonItem
        {
            Caption = "Tek Dosya Tara",
            ImageOptions = { SvgImage = GetSvgImage("status_infected.svg") },
            RibbonStyle = RibbonItemStyles.Large
        };
        btnFileScan.ItemClick += BtnFileScan_Click;
        _ribbonGroupScan.ItemLinks.Add(btnFileScan);
        
        _ribbonGroupScan.ItemLinks.Add(_btnStopScan);

        // Quarantine Group
        _ribbonGroupQuarantine = new RibbonPageGroup("Karantina");
        
        _btnQuarantine = new BarButtonItem
        {
            Caption = "Karantina",
            ImageOptions = { SvgImage = GetSvgImage("quarantine.svg") },
            RibbonStyle = RibbonItemStyles.Large
        };
        _btnQuarantine.ItemClick += BtnQuarantine_Click;
        
        var btnQuarantineSelected = new BarButtonItem
        {
            Caption = "Seçileni Karantinaya Al",
            ImageOptions = { SvgImage = GetSvgImage("shield_warning.svg") },
            RibbonStyle = RibbonItemStyles.SmallWithText
        };
        btnQuarantineSelected.ItemClick += BtnQuarantineSelected_Click;

        _ribbonGroupQuarantine.ItemLinks.Add(_btnQuarantine);
        _ribbonGroupQuarantine.ItemLinks.Add(btnQuarantineSelected);

        // Settings Group
        _ribbonGroupSettings = new RibbonPageGroup("Ayarlar");
        
        _btnUpdate = new BarButtonItem
        {
            Caption = "Veritabanını Güncelle",
            ImageOptions = { SvgImage = GetSvgImage("update.svg") },
            RibbonStyle = RibbonItemStyles.Large
        };
        _btnUpdate.ItemClick += BtnUpdate_Click;

        var btnHistory = new BarButtonItem
        {
            Caption = "Tarama Geçmişi",
            ImageOptions = { SvgImage = GetSvgImage("scan_radar.svg") }, // Reuse scan icon or similar
            RibbonStyle = RibbonItemStyles.Large
        };
        btnHistory.ItemClick += (s, e) => ShowHistory();

        _ribbonGroupSettings.ItemLinks.Add(_btnUpdate);
        _ribbonGroupSettings.ItemLinks.Add(btnHistory);

        _ribbonPageHome.Groups.Add(_ribbonGroupScan);
        _ribbonPageHome.Groups.Add(_ribbonGroupQuarantine);
        _ribbonPageHome.Groups.Add(_ribbonGroupSettings);

        _ribbonControl.Pages.Add(_ribbonPageHome);
        this.Controls.Add(_ribbonControl);
    }

    private void InitializeGrid()
    {
        // Initialize Icons
        _statusIcons = new DevExpress.Utils.SvgImageCollection(this.components);
        _statusIcons.Add(GetSvgImage("status_clean.svg"));
        _statusIcons.Add(GetSvgImage("status_infected.svg"));

        _gridControl = new GridControl
        {
            Dock = DockStyle.Fill
        };

        _gridView = new GridView(_gridControl);
        _gridControl.MainView = _gridView;

        // Configure columns
        var colStatus = new GridColumn
        {
            FieldName = "StatusIndex",
            Caption = " ",
            Width = 40,
            OptionsColumn = { AllowEdit = false, AllowSort = DefaultBoolean.False }
        };

        var repoStatus = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
        repoStatus.SmallImages = _statusIcons;
        repoStatus.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 0, 0));
        repoStatus.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 1, 1));
        repoStatus.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
        
        _gridControl.RepositoryItems.Add(repoStatus);
        colStatus.ColumnEdit = repoStatus;
        
        var colFileName = new GridColumn
        {
            FieldName = "FileName",
            Caption = "Dosya Adı",
            Width = 200,
            OptionsColumn = { AllowEdit = false }
        };
        
        var colStatus2 = new GridColumn
        {
            FieldName = "StatusText",
            Caption = "Durum",
            Width = 100,
            OptionsColumn = { AllowEdit = false }
        };
        
        var colFilePath = new GridColumn
        {
            FieldName = "FilePath",
            Caption = "Tam Yol",
            Width = 400,
            OptionsColumn = { AllowEdit = false }
        };
        
        var colThreatType = new GridColumn
        {
            FieldName = "ThreatTypeName",
            Caption = "Tehdit Türü",
            Width = 120,
            OptionsColumn = { AllowEdit = false }
        };
        
        var colHash = new GridColumn
        {
            FieldName = "Hash",
            Caption = "Hash",
            Width = 200,
            OptionsColumn = { AllowEdit = false }
        };

        _gridView.Columns.AddRange(new[] { colStatus, colFileName, colStatus2, colFilePath, colThreatType, colHash });
        
        // Configure view
        _gridView.OptionsView.ShowGroupPanel = false;
        _gridView.OptionsView.RowAutoHeight = false;
        _gridView.RowHeight = 32; // Increased for icons
        _gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
        
        // Row styling for infected files
        _gridView.RowStyle += GridView_RowStyle;

        // Bind data
        _scanResultsBinding = new BindingSource { DataSource = _scanResults };
        _gridControl.DataSource = _scanResultsBinding;
    }

    private void GridView_RowStyle(object sender, RowStyleEventArgs e)
    {
        if (_gridView.GetRow(e.RowHandle) is ScanResultViewModel result)
        {
            if (result.IsInfected)
            {
                e.Appearance.BackColor = Color.FromArgb(255, 235, 235);
                e.Appearance.ForeColor = Color.DarkRed;
            }
            else if (result.Status == ScanStatus.Clean)
            {
                e.Appearance.BackColor = Color.FromArgb(235, 255, 235);
            }
        }
    }

    private void InitializeProgressPanel()
    {
        // Create a simple progress panel with percentage display
        _progressPanel = new PanelControl
        {
            Size = new Size(200, 150),
            BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        };

        var lblTitle = new LabelControl
        {
            Text = "Tarama İlerlemesi",
            Location = new Point(10, 10),
            AutoSizeMode = LabelAutoSizeMode.None,
            Size = new Size(180, 25),
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Center } }
        };

        _lblProgressPercent = new LabelControl
        {
            Text = "0%",
            Location = new Point(10, 50),
            AutoSizeMode = LabelAutoSizeMode.None,
            Size = new Size(180, 60),
            Appearance = 
            { 
                TextOptions = { HAlignment = HorzAlignment.Center },
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204) // DevExpress blue
            }
        };

        var lblSubtitle = new LabelControl
        {
            Text = "Tamamlandı",
            Location = new Point(10, 115),
            AutoSizeMode = LabelAutoSizeMode.None,
            Size = new Size(180, 20),
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Center }, ForeColor = Color.Gray }
        };

        _progressPanel.Controls.Add(lblTitle);
        _progressPanel.Controls.Add(_lblProgressPercent);
        _progressPanel.Controls.Add(lblSubtitle);
    }

    private void InitializeStatusBar()
    {
        _lblStatus = new LabelControl
        {
            Text = "Hazır",
            Dock = DockStyle.Bottom,
            Height = 25,
            Padding = new Padding(10, 5, 10, 5),
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Near } }
        };

        _progressBar = new ProgressBarControl
        {
            Dock = DockStyle.Bottom,
            Height = 20,
            Properties = { ShowTitle = true },
            Visible = false
        };

        _lblStats = new LabelControl
        {
            Text = $"İmza Veritabanı: {_signatureDatabase.SignatureCount} imza | Karantina: {_quarantineService.QuarantinedCount} dosya",
            Dock = DockStyle.Bottom,
            Height = 25,
            Padding = new Padding(10, 5, 10, 5),
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Far } }
        };
    }

    private void InitializeAlertControl()
    {
        _alertControl = new AlertControl(this.components ?? (this.components = new System.ComponentModel.Container()));
        _alertControl.AutoFormDelay = 5000;
        _alertControl.FormLocation = AlertFormLocation.BottomRight;
        _alertControl.FormMaxCount = 5;
    }

    private void LayoutComponents()
    {
        // Create main panel
        var mainPanel = new PanelControl { Dock = DockStyle.Fill };
        
        // Create split panel for grid and gauge
        var splitContainer = new SplitContainerControl
        {
            Dock = DockStyle.Fill,
            Horizontal = false,
            SplitterPosition = 500,
            Panel1 = { MinSize = 300 },
            Panel2 = { MinSize = 150 }
        };

        // Dashboard panel with stats
        var dashboardPanel = new PanelControl
        {
            Dock = DockStyle.Top,
            Height = 120,
            Padding = new Padding(10)
        };

        // TableLayoutPanel for proportional scaling
        var statsTable = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            BackColor = Color.Transparent
        };

        // Proportional columns (25% each)
        statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

        statsTable.Controls.Add(CreateStatCard("🛡️ Koruma Durumu", "Aktif", Color.Green), 0, 0);
        statsTable.Controls.Add(CreateStatCard("📊 İmza Sayısı", _signatureDatabase.SignatureCount.ToString(), Color.DeepSkyBlue), 1, 0);
        statsTable.Controls.Add(CreateStatCard("🔒 Karantina", _quarantineService.QuarantinedCount.ToString(), Color.Orange), 2, 0);
        statsTable.Controls.Add(CreateStatCard("📅 Son Güncelleme", _signatureDatabase.LastUpdated.ToString("dd.MM.yyyy"), Color.Gray), 3, 0);

        foreach (Control ctrl in statsTable.Controls) ctrl.Dock = DockStyle.Fill;

        dashboardPanel.Controls.Add(statsTable);

        splitContainer.Panel1.Controls.Add(_gridControl);
        
        // Bottom panel with progress indicator
        var bottomPanel = new PanelControl { Dock = DockStyle.Fill };
        bottomPanel.Controls.Add(_progressPanel);
        
        // Center the progress panel
        _progressPanel.Anchor = AnchorStyles.None;
        _progressPanel.Location = new Point((bottomPanel.Width - _progressPanel.Width) / 2, (bottomPanel.Height - _progressPanel.Height) / 2);
        bottomPanel.Resize += (s, e) => {
            _progressPanel.Location = new Point((bottomPanel.Width - _progressPanel.Width) / 2, (bottomPanel.Height - _progressPanel.Height) / 2);
        };
        
        splitContainer.Panel2.Controls.Add(bottomPanel);

        mainPanel.Controls.Add(splitContainer);
        mainPanel.Controls.Add(dashboardPanel);

        this.Controls.Add(mainPanel);
        this.Controls.Add(_progressBar);
        this.Controls.Add(_lblStatus);
        this.Controls.Add(_lblStats);
    }

    private PanelControl CreateStatCard(string title, string value, Color accentColor, string iconName = null)
    {
        var card = new PanelControl
        {
            Size = new Size(220, 100),
            Margin = new Padding(10),
            BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder,
            BackColor = Color.FromArgb(30, accentColor) // Light tinted background
        };
        // Add a colored strip at left
        var strip = new LabelControl
        {
            AutoSizeMode = LabelAutoSizeMode.None,
            Dock = DockStyle.Left,
            Width = 5,
            BackColor = accentColor
        };

        var lblTitle = new LabelControl
        {
            Text = title,
            Location = new Point(15, 15),
            AutoSizeMode = LabelAutoSizeMode.None,
            Size = new Size(190, 20),
            Appearance = { ForeColor = Color.Gray, Font = new Font("Segoe UI", 9, FontStyle.Regular) }
        };

        var lblValue = new LabelControl
        {
            Name = "lblValue",
            Text = value,
            Location = new Point(15, 45),
            AutoSizeMode = LabelAutoSizeMode.None,
            Size = new Size(190, 40),
            Appearance = 
            { 
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = accentColor
            }
        };

        card.Controls.Add(lblValue);
        card.Controls.Add(lblTitle);
        card.Controls.Add(strip);
        
        return card;
    }

    private DevExpress.Utils.Svg.SvgImage? GetSvgImage(string fileName)
    {
        try
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", fileName);
            if (File.Exists(path))
            {
                return DevExpress.Utils.Svg.SvgImage.FromFile(path);
            }
        }
        catch { /* Handle loading error */ }
        return null;
    }

    private async void LoadInitialData()
    {
        await _signatureDatabase.LoadSignaturesAsync();
        await _quarantineService.LoadMetadataAsync();
        UpdateStats();
    }

    private void UpdateStats()
    {
        if (InvokeRequired)
        {
            Invoke(new Action(UpdateStats));
            return;
        }
        
        _lblStats.Text = $"İmza Veritabanı: {_signatureDatabase.SignatureCount} imza | Karantina: {_quarantineService.QuarantinedCount} dosya";
        
        // Update responsive cards if they exist
        foreach (Control c in this.Controls)
        {
            if (c is PanelControl mainPanel)
            {
                foreach (Control sub in mainPanel.Controls)
                {
                    if (sub is PanelControl dashboardPanel)
                    {
                        foreach (Control table in dashboardPanel.Controls)
                        {
                            if (table is TableLayoutPanel statsTable)
                            {
                                UpdateCardValue(statsTable, 1, _signatureDatabase.SignatureCount.ToString());
                                UpdateCardValue(statsTable, 2, _quarantineService.QuarantinedCount.ToString());
                            }
                        }
                    }
                }
            }
        }
    }

    private void UpdateCardValue(TableLayoutPanel table, int index, string value)
    {
        if (table.Controls.Count > index && table.Controls[index] is PanelControl card)
        {
            var lbl = card.Controls["lblValue"] as LabelControl;
            if (lbl != null) lbl.Text = value;
        }
    }

    #region Event Handlers

    private async void BtnQuickScan_Click(object? sender, ItemClickEventArgs e)
    {
        await StartScanAsync(ScanType.Quick);
    }

    private async void BtnFullScan_Click(object? sender, ItemClickEventArgs e)
    {
        var result = XtraMessageBox.Show(
            "Tüm sistem taraması uzun sürebilir. Devam etmek istiyor musunuz?",
            "Derin Tarama",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            await StartScanAsync(ScanType.Full, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        }
    }

    private async void BtnCustomScan_Click(object? sender, ItemClickEventArgs e)
    {
        using var folderDialog = new FolderBrowserDialog
        {
            Description = "Taranacak klasörü seçin",
            ShowNewFolderButton = false
        };

        if (folderDialog.ShowDialog() == DialogResult.OK)
        {
            await StartScanAsync(ScanType.Custom, folderDialog.SelectedPath);
        }
    }

    private async void BtnFileScan_Click(object? sender, ItemClickEventArgs e)
    {
        using var fileDialog = new OpenFileDialog
        {
            Title = "Taranacak dosyayı seçin",
            Filter = "Tüm Dosyalar (*.*)|*.*",
            Multiselect = false
        };

        if (fileDialog.ShowDialog() == DialogResult.OK)
        {
            await ScanSingleFileAsync(fileDialog.FileName);
        }
    }

    private async Task ScanSingleFileAsync(string filePath)
    {
        if (_scanEngine.IsScanning)
        {
            XtraMessageBox.Show("Zaten bir tarama devam ediyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _scanResults.Clear();
        _lblStatus.Text = $"Taranıyor: {filePath}";
        _progressBar.Visible = true;
        _progressBar.Position = 50;

        try
        {
            var result = await _scanEngine.ScanFileAsync(filePath);
            
            _scanResults.Add(new ScanResultViewModel(result));
            
            if (result.IsInfected)
            {
                ShowAlert("⚠️ Tehdit Bulundu!", $"{result.FileName}\n{result.ThreatInfo?.ThreatName}", AlertType.Warning);
                _lblStatus.Text = $"TEHDİT TESPIT EDİLDİ: {result.FileName}";
            }
            else
            {
                ShowAlert("✅ Temiz", $"{result.FileName} dosyası temiz.", AlertType.Info);
                _lblStatus.Text = $"Dosya temiz: {result.FileName}";
            }
            
            UpdateStats();
        }
        catch (Exception ex)
        {
            ShowAlert("Hata", $"Tarama sırasında hata: {ex.Message}", AlertType.Error);
            _lblStatus.Text = $"Hata: {ex.Message}";
        }
        finally
        {
            _progressBar.Visible = false;
        }
    }

    private void BtnStopScan_Click(object? sender, ItemClickEventArgs e)
    {
        _scanCancellationTokenSource?.Cancel();
        _lblStatus.Text = "Tarama durduruluyor...";
    }

    private void BtnQuarantine_Click(object? sender, ItemClickEventArgs e)
    {
        using var quarantineForm = new QuarantineForm(_quarantineService);
        quarantineForm.ShowDialog(this);
        UpdateStats();
    }

    private void BtnQuarantineSelected_Click(object? sender, ItemClickEventArgs e)
    {
        var selectedRows = _gridView.GetSelectedRows();
        if (selectedRows.Length == 0)
        {
            XtraMessageBox.Show("Lütfen karantinaya alınacak dosyayı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var quarantinedCount = 0;
        foreach (var rowHandle in selectedRows)
        {
            if (_gridView.GetRow(rowHandle) is ScanResultViewModel result && result.IsInfected)
            {
                var qResult = _quarantineService.MoveToQuarantine(result.FilePath, result.ThreatInfo);
                if (qResult.Success)
                {
                    quarantinedCount++;
                }
            }
        }

        if (quarantinedCount > 0)
        {
            ShowAlert("Karantina", $"{quarantinedCount} dosya karantinaya alındı.", AlertType.Warning);
            UpdateStats();
        }
    }

    private async void BtnUpdate_Click(object? sender, ItemClickEventArgs e)
    {
        _lblStatus.Text = "Veritabanı güncelleniyor...";
        
        // Simulate database update (in real scenario, this would fetch from a server)
        await Task.Delay(1000);
        await _signatureDatabase.SaveSignaturesAsync();
        
        _lblStatus.Text = "Veritabanı güncellendi.";
        ShowAlert("Güncelleme", "İmza veritabanı başarıyla güncellendi.", AlertType.Info);
        UpdateStats();
    }

    private void OnThreatDetected(object? sender, FileScanResult result)
    {
        if (InvokeRequired)
        {
            Invoke(new Action(() => OnThreatDetected(sender, result)));
            return;
        }

        ShowAlert("⚠️ Tehdit Bulundu!", 
            $"{result.FileName}\n{result.ThreatInfo?.ThreatName ?? "Bilinmeyen Tehdit"}", 
            AlertType.Warning);
    }

    private void OnScanCompleted(object? sender, ScanReport report)
    {
        if (InvokeRequired)
        {
            Invoke(new Action(() => OnScanCompleted(sender, report)));
            return;
        }

        var alertType = report.InfectedCount > 0 ? AlertType.Warning : AlertType.Info;
        var message = report.WasCancelled 
            ? "Tarama iptal edildi." 
            : report.GetSummary();
        
        ShowAlert("Tarama Tamamlandı", message, alertType);
    }

    #endregion

    #region Scan Methods

    private void ShowHistory()
    {
        using (var historyForm = new HistoryForm(_logService))
        {
            historyForm.ShowDialog();
        }
    }

    private async Task StartScanAsync(ScanType scanType, string? path = null)
    {
        if (_scanEngine.IsScanning)
        {
            XtraMessageBox.Show("Zaten bir tarama devam ediyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Clear previous results
        _scanResults.Clear();
        
        // Setup UI
        SetScanningUI(true);
        _progressBar.Position = 0;
        _progressBar.Visible = true;

        // Create cancellation token
        _scanCancellationTokenSource = new CancellationTokenSource();
        var token = _scanCancellationTokenSource.Token;

        // Create progress reporter
        var progress = new Progress<ScanProgress>(UpdateProgress);

        try
        {
            ScanReport report;
            
            if (scanType == ScanType.Quick)
            {
                _lblStatus.Text = "Hızlı tarama başlatılıyor...";
                report = await _scanEngine.QuickScanAsync(progress, token);
            }
            else
            {
                _lblStatus.Text = $"Taranıyor: {path}";
                report = await _scanEngine.ScanDirectoryAsync(path!, progress, token);
            }

            // Add all infected files to the grid
            foreach (var infected in report.InfectedFiles)
            {
                _scanResults.Add(new ScanResultViewModel(infected));
            }

            _lblStatus.Text = report.WasCancelled 
                ? "Tarama iptal edildi." 
                : $"Tarama tamamlandı. {report.TotalFilesScanned} dosya tarandı, {report.InfectedCount} tehdit bulundu.";
            
            // Save to history log
            if (!report.WasCancelled)
            {
                await _logService.LogScanAsync(report);
            }
            
            UpdateStats();
        }
        catch (Exception ex)
        {
            _lblStatus.Text = $"Tarama hatası: {ex.Message}";
            ShowAlert("Hata", $"Tarama sırasında hata oluştu: {ex.Message}", AlertType.Error);
        }
        finally
        {
            SetScanningUI(false);
            _progressBar.Visible = false;
            _scanCancellationTokenSource?.Dispose();
            _scanCancellationTokenSource = null;
        }
    }

    private void UpdateProgress(ScanProgress progress)
    {
        if (InvokeRequired)
        {
            Invoke(new Action(() => UpdateProgress(progress)));
            return;
        }

        _progressBar.Position = (int)progress.PercentComplete;
        _progressBar.Properties.Maximum = 100;
        
        // Shorten the displayed path
        var displayPath = progress.CurrentFile.Length > 60 
            ? "..." + progress.CurrentFile.Substring(progress.CurrentFile.Length - 57) 
            : progress.CurrentFile;
        
        _lblStatus.Text = $"Taranıyor: {displayPath} ({progress.ScannedFiles}/{progress.TotalFiles})";

        // Update progress label
        _lblProgressPercent.Text = $"{(int)progress.PercentComplete}%";
    }

    private void SetScanningUI(bool isScanning)
    {
        _btnQuickScan.Enabled = !isScanning;
        _btnFullScan.Enabled = !isScanning;
        _btnCustomScan.Enabled = !isScanning;
        _btnStopScan.Enabled = isScanning;
    }

    #endregion

    #region Alert Methods

    private enum AlertType { Info, Warning, Error }

    private void ShowAlert(string caption, string text, AlertType type)
    {
        var alertInfo = new AlertInfo(caption, text);
        _alertControl.Show(this, alertInfo);
    }

    #endregion

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _scanCancellationTokenSource?.Cancel();
        base.OnFormClosing(e);
    }
}

/// <summary>
/// ViewModel for displaying scan results in the grid
/// </summary>
public class ScanResultViewModel
{
    public string StatusIcon { get; set; }
    public string FileName { get; set; }
    public string StatusText { get; set; }
    public string FilePath { get; set; }
    public string ThreatTypeName { get; set; }
    public string Hash { get; set; }
    public bool IsInfected { get; set; }
    public ThreatInfo? ThreatInfo { get; set; }
    public ScanStatus Status { get; set; }

    public ScanResultViewModel(FileScanResult result)
    {
        FileName = result.FileName;
        FilePath = result.FilePath;
        Hash = result.Hash;
        IsInfected = result.IsInfected;
        ThreatInfo = result.ThreatInfo;
        Status = result.Status;
        
        StatusIcon = result.IsInfected ? "🔴" : "🟢";
        StatusText = result.IsInfected ? "Tehdit" : "Temiz";
        ThreatTypeName = result.ThreatInfo?.ThreatType.ToString() ?? "-";
        StatusIndex = result.IsInfected ? 1 : 0;
    }

    public int StatusIndex { get; set; }
}
