namespace Antivirus.Core.Models;

/// <summary>
/// Represents the progress of an ongoing scan operation
/// Devam eden tarama işleminin ilerlemesini temsil eder
/// </summary>
public class ScanProgress
{
    /// <summary>
    /// Path of the file currently being scanned
    /// </summary>
    public string CurrentFile { get; set; } = string.Empty;

    /// <summary>
    /// Total number of files to be scanned
    /// </summary>
    public int TotalFiles { get; set; }

    /// <summary>
    /// Number of files that have been scanned
    /// </summary>
    public int ScannedFiles { get; set; }

    /// <summary>
    /// Number of threats found so far
    /// </summary>
    public int ThreatsFound { get; set; }

    /// <summary>
    /// Percentage of scan completion (0-100)
    /// </summary>
    public double PercentComplete => TotalFiles > 0 
        ? Math.Round((double)ScannedFiles / TotalFiles * 100, 2) 
        : 0;

    /// <summary>
    /// Estimated time remaining in seconds
    /// </summary>
    public int EstimatedSecondsRemaining { get; set; }

    /// <summary>
    /// Current scanning speed (files per second)
    /// </summary>
    public double FilesPerSecond { get; set; }

    public ScanProgress() { }

    public ScanProgress(string currentFile, int totalFiles, int scannedFiles, int threatsFound)
    {
        CurrentFile = currentFile;
        TotalFiles = totalFiles;
        ScannedFiles = scannedFiles;
        ThreatsFound = threatsFound;
    }
}
