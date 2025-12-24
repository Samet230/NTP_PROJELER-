using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Antivirus.Core.Models;
using Antivirus.Core.Services;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using DevExpress.Data;

namespace Antivirus.UI.Forms;

public class HistoryForm : XtraForm
{
    private readonly LogService _logService;
    private GridControl _gridControl = null!;
    private GridView _gridView = null!;

    public HistoryForm(LogService logService)
    {
        _logService = logService;
        InitializeComponents();
        LoadData();
    }

    private void InitializeComponents()
    {
        this.Text = "Tarama Geçmişi";
        this.Size = new Size(800, 500);
        this.StartPosition = FormStartPosition.CenterParent;
        this.Icon = SystemIcons.Information;

        _gridControl = new GridControl { Dock = DockStyle.Fill };
        _gridView = new GridView(_gridControl);
        _gridControl.MainView = _gridView;

        var colDate = new GridColumn { FieldName = "ScanTime", Caption = "Tarih", UnboundType = UnboundColumnType.DateTime, Visible = true };
        colDate.DisplayFormat.FormatType = FormatType.DateTime;
        colDate.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";

        var colType = new GridColumn { FieldName = "ScanType", Caption = "Tarama Türü", Visible = true, Width = 80 };
        var colTotal = new GridColumn { FieldName = "TotalFiles", Caption = "Taranan", Visible = true, Width = 60 };
        var colInfected = new GridColumn { FieldName = "InfectedCount", Caption = "Tehdit", Visible = true, Width = 60 };
        var colSummary = new GridColumn { FieldName = "Summary", Caption = "Açıklama", Visible = true, Width = 180 };
        
        // Column to show threats with file paths
        var colThreats = new GridColumn 
        { 
            FieldName = "FoundThreatsDisplay", 
            Caption = "Bulunan Tehditler (Tehdit | Dosya Yolu)", 
            Visible = true, 
            Width = 350
        };
        colThreats.UnboundType = UnboundColumnType.String;

        _gridView.Columns.AddRange(new[] { colDate, colType, colTotal, colInfected, colSummary, colThreats });
        _gridView.OptionsView.ShowGroupPanel = false;
        _gridView.OptionsBehavior.Editable = false;
        _gridView.RowHeight = 50; // Allow multi-line for threats
        
        // Custom unbound column to display threats list as string
        _gridView.CustomUnboundColumnData += (sender, e) =>
        {
            if (e.Column.FieldName == "FoundThreatsDisplay" && e.IsGetData)
            {
                var log = e.Row as ScanLog;
                if (log?.FoundThreats != null && log.FoundThreats.Count > 0)
                {
                    e.Value = string.Join("\n", log.FoundThreats);
                }
                else
                {
                    e.Value = "-";
                }
            }
        };

        this.Controls.Add(_gridControl);
    }

    private async void LoadData()
    {
        await _logService.LoadLogsAsync();
        _gridControl.DataSource = _logService.GetLogs();
    }
}
