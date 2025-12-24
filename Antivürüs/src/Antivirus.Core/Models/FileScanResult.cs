namespace Antivirus.Core.Models;

/// <summary>
/// Represents the result of scanning a single file
/// Tek bir dosyanın tarama sonucunu temsil eder
/// </summary>
public class FileScanResult
{
    /// <summary>
    /// Full path to the scanned file
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Name of the file (without path)
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the file is infected
    /// </summary>
    public bool IsInfected { get; set; }

    /// <summary>
    /// Status of the scan for this file
    /// </summary>
    public ScanStatus Status { get; set; } = ScanStatus.Clean;

    /// <summary>
    /// Threat information if the file is infected
    /// </summary>
    public ThreatInfo? ThreatInfo { get; set; }

    /// <summary>
    /// Calculated hash of the file (SHA256)
    /// </summary>
    public string Hash { get; set; } = string.Empty;

    /// <summary>
    /// Size of the file in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Time when the scan was performed
    /// </summary>
    public DateTime ScanTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Error message if scan failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates a clean scan result
    /// </summary>
    public static FileScanResult CreateClean(string filePath, string hash, long fileSize)
    {
        return new FileScanResult
        {
            FilePath = filePath,
            FileName = Path.GetFileName(filePath),
            IsInfected = false,
            Status = ScanStatus.Clean,
            Hash = hash,
            FileSize = fileSize,
            ScanTime = DateTime.Now
        };
    }

    /// <summary>
    /// Creates an infected scan result
    /// </summary>
    public static FileScanResult CreateInfected(string filePath, string hash, long fileSize, ThreatInfo threatInfo)
    {
        return new FileScanResult
        {
            FilePath = filePath,
            FileName = Path.GetFileName(filePath),
            IsInfected = true,
            Status = ScanStatus.Infected,
            Hash = hash,
            FileSize = fileSize,
            ThreatInfo = threatInfo,
            ScanTime = DateTime.Now
        };
    }

    /// <summary>
    /// Creates an error scan result
    /// </summary>
    public static FileScanResult CreateError(string filePath, string errorMessage)
    {
        return new FileScanResult
        {
            FilePath = filePath,
            FileName = Path.GetFileName(filePath),
            IsInfected = false,
            Status = ScanStatus.Error,
            ErrorMessage = errorMessage,
            ScanTime = DateTime.Now
        };
    }
}
