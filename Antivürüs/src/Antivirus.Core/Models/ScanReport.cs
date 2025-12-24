namespace Antivirus.Core.Models;

/// <summary>
/// Represents a complete scan report after scanning completes
/// Tarama tamamlandıktan sonra tam tarama raporunu temsil eder
/// </summary>
public class ScanReport
{
    /// <summary>
    /// Unique identifier for this scan report
    /// </summary>
    public Guid ReportId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Time when the scan started
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Time when the scan ended
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Total duration of the scan
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// Type of scan performed
    /// </summary>
    public ScanType ScanType { get; set; }

    /// <summary>
    /// Root path that was scanned
    /// </summary>
    public string ScannedPath { get; set; } = string.Empty;

    /// <summary>
    /// Total number of files scanned
    /// </summary>
    public int TotalFilesScanned { get; set; }

    /// <summary>
    /// Number of clean files
    /// </summary>
    public int CleanFiles { get; set; }

    /// <summary>
    /// Number of files with errors during scan
    /// </summary>
    public int ErrorFiles { get; set; }

    /// <summary>
    /// List of infected file results
    /// </summary>
    public List<FileScanResult> InfectedFiles { get; set; } = new();

    /// <summary>
    /// Number of infected files found
    /// </summary>
    public int InfectedCount => InfectedFiles.Count;

    /// <summary>
    /// Indicates whether the scan was cancelled
    /// </summary>
    public bool WasCancelled { get; set; }

    /// <summary>
    /// Total bytes scanned
    /// </summary>
    public long TotalBytesScanned { get; set; }

    /// <summary>
    /// Average scanning speed in files per second
    /// </summary>
    public double AverageFilesPerSecond => Duration.TotalSeconds > 0 
        ? TotalFilesScanned / Duration.TotalSeconds 
        : 0;

    /// <summary>
    /// Creates a summary string of the scan report
    /// </summary>
    public string GetSummary()
    {
        return $"Tarama Tamamlandı: {TotalFilesScanned} dosya tarandı, " +
               $"{InfectedCount} tehdit bulundu, " +
               $"Süre: {Duration:hh\\:mm\\:ss}";
    }
}
