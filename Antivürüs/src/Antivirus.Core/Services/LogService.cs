using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Antivirus.Core.Models;

namespace Antivirus.Core.Services;

public class LogService
{
    private readonly string _logFilePath;
    private List<ScanLog> _logs = new();

    public LogService(string logFilePath)
    {
        _logFilePath = logFilePath;
    }

    public async Task LoadLogsAsync()
    {
        try
        {
            if (File.Exists(_logFilePath))
            {
                var json = await File.ReadAllTextAsync(_logFilePath);
                _logs = JsonSerializer.Deserialize<List<ScanLog>>(json) ?? new List<ScanLog>();
            }
        }
        catch
        {
            _logs = new List<ScanLog>();
        }
    }

    public async Task LogScanAsync(ScanReport report)
    {
        var log = new ScanLog
        {
            ScanTime = report.EndTime,
            ScanType = report.ScanType.ToString(),
            TotalFiles = report.TotalFilesScanned,
            InfectedCount = report.InfectedFiles.Count,
            Summary = report.InfectedFiles.Count > 0 
                ? $"{report.InfectedFiles.Count} tehdit tespit edildi." 
                : "Tarama temiz tamamlandı."
        };

        foreach (var threat in report.InfectedFiles)
        {
            log.FoundThreats.Add($"{threat.ThreatInfo?.ThreatName} | {threat.FilePath}");
        }

        _logs.Add(log);
        await SaveLogsAsync();
    }

    public List<ScanLog> GetLogs() => _logs;

    private async Task SaveLogsAsync()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_logs, options);
            await File.WriteAllTextAsync(_logFilePath, json);
        }
        catch { /* Fallback */ }
    }
}
