namespace Antivirus.Core.Models;

/// <summary>
/// Represents information about a file in quarantine
/// Karantinadaki bir dosya hakkında bilgi içerir
/// </summary>
public class QuarantinedFileInfo
{
    /// <summary>
    /// Unique identifier for this quarantined file
    /// </summary>
    public string QuarantineId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Original file path before quarantine
    /// </summary>
    public string OriginalPath { get; set; } = string.Empty;

    /// <summary>
    /// Original file name
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// Current path in quarantine folder
    /// </summary>
    public string QuarantinePath { get; set; } = string.Empty;

    /// <summary>
    /// Hash of the file
    /// </summary>
    public string FileHash { get; set; } = string.Empty;

    /// <summary>
    /// Size of the file in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Date when the file was quarantined
    /// </summary>
    public DateTime QuarantineDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Information about the detected threat
    /// </summary>
    public ThreatInfo? ThreatInfo { get; set; }

    /// <summary>
    /// Indicates if the file can be restored
    /// </summary>
    public bool CanRestore { get; set; } = true;

    public QuarantinedFileInfo() { }

    public QuarantinedFileInfo(string originalPath, string quarantinePath, string fileHash, 
        long fileSize, ThreatInfo? threatInfo)
    {
        OriginalPath = originalPath;
        OriginalFileName = Path.GetFileName(originalPath);
        QuarantinePath = quarantinePath;
        FileHash = fileHash;
        FileSize = fileSize;
        ThreatInfo = threatInfo;
        QuarantineDate = DateTime.Now;
    }
}
