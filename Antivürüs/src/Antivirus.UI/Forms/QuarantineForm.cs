using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using Antivirus.Core.Interfaces;
using Antivirus.Core.Models;

namespace Antivirus.UI.Forms;

/// <summary>
/// Form for managing quarantined files
/// Karantinaya alınan dosyaları yönetmek için form
/// </summary>
public class QuarantineForm : XtraForm
{
    private readonly IQuarantineService _quarantineService;
    
    private GridControl _gridControl = null!;
    private GridView _gridView = null!;
    private BindingSource _bindingSource = null!;
    private BindingList<QuarantineViewModel> _quarantinedFiles = new();

    private SimpleButton _btnRestore = null!;
    private SimpleButton _btnDelete = null!;
    private SimpleButton _btnClose = null!;
    private LabelControl _lblInfo = null!;

    public QuarantineForm(IQuarantineService quarantineService)
    {
        _quarantineService = quarantineService;
        InitializeComponents();
        LoadQuarantinedFiles();
    }

    private void InitializeComponents()
    {
        this.Text = "Karantina Yönetimi";
        this.Size = new Size(900, 600);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        InitializeGrid();
        InitializeButtons();
        LayoutComponents();
    }

    private void InitializeGrid()
    {
        _gridControl = new GridControl { Dock = DockStyle.Fill };
        _gridView = new GridView(_gridControl);
        _gridControl.MainView = _gridView;

        // Configure columns
        var colFileName = new GridColumn
        {
            FieldName = "OriginalFileName",
            Caption = "Dosya Adı",
            Width = 200,
            OptionsColumn = { AllowEdit = false }
        };

        var colOriginalPath = new GridColumn
        {
            FieldName = "OriginalPath",
            Caption = "Orijinal Konum",
            Width = 300,
            OptionsColumn = { AllowEdit = false }
        };

        var colThreatName = new GridColumn
        {
            FieldName = "ThreatName",
            Caption = "Tehdit",
            Width = 150,
            OptionsColumn = { AllowEdit = false }
        };

        var colQuarantineDate = new GridColumn
        {
            FieldName = "QuarantineDateFormatted",
            Caption = "Karantina Tarihi",
            Width = 150,
            OptionsColumn = { AllowEdit = false }
        };

        var colSize = new GridColumn
        {
            FieldName = "FileSizeFormatted",
            Caption = "Boyut",
            Width = 80,
            OptionsColumn = { AllowEdit = false }
        };

        _gridView.Columns.AddRange(new[] { colFileName, colOriginalPath, colThreatName, colQuarantineDate, colSize });
        
        _gridView.OptionsView.ShowGroupPanel = false;
        _gridView.OptionsSelection.MultiSelect = true;
        _gridView.RowHeight = 28;

        _bindingSource = new BindingSource { DataSource = _quarantinedFiles };
        _gridControl.DataSource = _bindingSource;
    }

    private void InitializeButtons()
    {
        _btnRestore = new SimpleButton
        {
            Text = "Geri Yükle",
            Size = new Size(120, 35)
        };
        _btnRestore.Click += BtnRestore_Click;

        _btnDelete = new SimpleButton
        {
            Text = "Kalıcı Sil",
            Size = new Size(120, 35)
        };
        _btnDelete.Click += BtnDelete_Click;

        _btnClose = new SimpleButton
        {
            Text = "Kapat",
            Size = new Size(120, 35)
        };
        _btnClose.Click += (s, e) => this.Close();

        _lblInfo = new LabelControl
        {
            Text = "⚠️ Karantinadan geri yüklenen dosyalar potansiyel olarak zararlı olabilir!",
            AutoSizeMode = LabelAutoSizeMode.None,
            Size = new Size(500, 25),
            Appearance = { ForeColor = Color.DarkOrange }
        };
    }

    private void LayoutComponents()
    {
        var buttonPanel = new PanelControl
        {
            Dock = DockStyle.Bottom,
            Height = 80,
            Padding = new Padding(10)
        };

        var flowLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            Padding = new Padding(5)
        };

        flowLayout.Controls.Add(_btnRestore);
        flowLayout.Controls.Add(_btnDelete);
        flowLayout.Controls.Add(_btnClose);

        _lblInfo.Location = new Point(10, 25);
        buttonPanel.Controls.Add(_lblInfo);
        buttonPanel.Controls.Add(flowLayout);

        this.Controls.Add(_gridControl);
        this.Controls.Add(buttonPanel);
    }

    private void LoadQuarantinedFiles()
    {
        _quarantinedFiles.Clear();
        
        foreach (var file in _quarantineService.GetQuarantinedFiles())
        {
            _quarantinedFiles.Add(new QuarantineViewModel(file));
        }

        _bindingSource.ResetBindings(false);
    }

    private void BtnRestore_Click(object? sender, EventArgs e)
    {
        var selectedRows = _gridView.GetSelectedRows();
        if (selectedRows.Length == 0)
        {
            XtraMessageBox.Show("Lütfen geri yüklenecek dosyayı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = XtraMessageBox.Show(
            "Seçilen dosyaları orijinal konumlarına geri yüklemek istediğinize emin misiniz?\n\n" +
            "⚠️ Geri yüklenen dosyalar potansiyel olarak zararlı olabilir!",
            "Geri Yükleme Onayı",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result != DialogResult.Yes)
            return;

        var restoredCount = 0;
        for (int i = selectedRows.Length - 1; i >= 0; i--)
        {
            var rowHandle = selectedRows[i];
            if (_gridView.GetRow(rowHandle) is QuarantineViewModel vm)
            {
                if (_quarantineService.RestoreFromQuarantine(vm.QuarantineId))
                {
                    restoredCount++;
                }
            }
        }

        LoadQuarantinedFiles();
        XtraMessageBox.Show($"{restoredCount} dosya başarıyla geri yüklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        var selectedRows = _gridView.GetSelectedRows();
        if (selectedRows.Length == 0)
        {
            XtraMessageBox.Show("Lütfen silinecek dosyayı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = XtraMessageBox.Show(
            "Seçilen dosyaları kalıcı olarak silmek istediğinize emin misiniz?\n\n" +
            "Bu işlem geri alınamaz!",
            "Silme Onayı",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result != DialogResult.Yes)
            return;

        var deletedCount = 0;
        for (int i = selectedRows.Length - 1; i >= 0; i--)
        {
            var rowHandle = selectedRows[i];
            if (_gridView.GetRow(rowHandle) is QuarantineViewModel vm)
            {
                if (_quarantineService.DeletePermanently(vm.QuarantineId))
                {
                    deletedCount++;
                }
            }
        }

        LoadQuarantinedFiles();
        XtraMessageBox.Show($"{deletedCount} dosya kalıcı olarak silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

/// <summary>
/// ViewModel for displaying quarantined files in the grid
/// </summary>
public class QuarantineViewModel
{
    public string QuarantineId { get; set; }
    public string OriginalFileName { get; set; }
    public string OriginalPath { get; set; }
    public string ThreatName { get; set; }
    public string QuarantineDateFormatted { get; set; }
    public string FileSizeFormatted { get; set; }

    public QuarantineViewModel(QuarantinedFileInfo info)
    {
        QuarantineId = info.QuarantineId;
        OriginalFileName = info.OriginalFileName;
        OriginalPath = info.OriginalPath;
        ThreatName = info.ThreatInfo?.ThreatName ?? "Bilinmeyen";
        QuarantineDateFormatted = info.QuarantineDate.ToString("dd.MM.yyyy HH:mm");
        FileSizeFormatted = FormatFileSize(info.FileSize);
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}
