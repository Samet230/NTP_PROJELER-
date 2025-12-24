using Antivirus.Core.Models;

namespace Antivirus.Core.Interfaces;

/// <summary>
/// Interface for the antivirus scan engine
/// Antivirüs tarama motoru arayüzü
/// </summary>
public interface IScanEngine
{
    /// <summary>
    /// Scans a single file for malware
    /// </summary>
    /// <param name="filePath">Path to the file to scan</param>
    /// <returns>Scan result for the file</returns>
    Task<FileScanResult> ScanFileAsync(string filePath);

    /// <summary>
    /// Scans a directory recursively for malware
    /// </summary>
    /// <param name="directoryPath">Path to the directory to scan</param>
    /// <param name="progress">Progress reporter</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>Complete scan report</returns>
    Task<ScanReport> ScanDirectoryAsync(
        string directoryPath, 
        IProgress<ScanProgress>? progress = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a quick scan on common malware locations
    /// </summary>
    /// <param name="progress">Progress reporter</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>Complete scan report</returns>
    Task<ScanReport> QuickScanAsync(
        IProgress<ScanProgress>? progress = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates if a scan is currently in progress
    /// </summary>
    bool IsScanning { get; }

    /// <summary>
    /// Event raised when a threat is detected during scanning
    /// </summary>
    event EventHandler<FileScanResult>? ThreatDetected;

    /// <summary>
    /// Event raised when the scan completes
    /// </summary>
    event EventHandler<ScanReport>? ScanCompleted;
}
