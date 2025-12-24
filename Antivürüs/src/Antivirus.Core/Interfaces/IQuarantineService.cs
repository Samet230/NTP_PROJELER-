using Antivirus.Core.Models;

namespace Antivirus.Core.Interfaces;

/// <summary>
/// Result of a quarantine operation
/// Karantina işleminin sonucu
/// </summary>
public class QuarantineResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public QuarantinedFileInfo? FileInfo { get; set; }

    public static QuarantineResult Succeeded(QuarantinedFileInfo fileInfo, string message = "Dosya karantinaya alındı.")
    {
        return new QuarantineResult { Success = true, Message = message, FileInfo = fileInfo };
    }

    public static QuarantineResult Failed(string message)
    {
        return new QuarantineResult { Success = false, Message = message };
    }
}

/// <summary>
/// Interface for managing quarantined files
/// Karantinaya alınan dosyaları yöneten arayüz
/// </summary>
public interface IQuarantineService
{
    /// <summary>
    /// Moves a file to quarantine
    /// </summary>
    /// <param name="filePath">Path to the file to quarantine</param>
    /// <param name="threatInfo">Optional threat information</param>
    /// <returns>Result of the quarantine operation</returns>
    QuarantineResult MoveToQuarantine(string filePath, ThreatInfo? threatInfo = null);

    /// <summary>
    /// Restores a file from quarantine to its original location
    /// </summary>
    /// <param name="quarantineId">ID of the quarantined file</param>
    /// <returns>True if restored successfully</returns>
    bool RestoreFromQuarantine(string quarantineId);

    /// <summary>
    /// Permanently deletes a quarantined file
    /// </summary>
    /// <param name="quarantineId">ID of the quarantined file</param>
    /// <returns>True if deleted successfully</returns>
    bool DeletePermanently(string quarantineId);

    /// <summary>
    /// Gets all quarantined files
    /// </summary>
    /// <returns>List of quarantined file information</returns>
    List<QuarantinedFileInfo> GetQuarantinedFiles();

    /// <summary>
    /// Gets a specific quarantined file by ID
    /// </summary>
    /// <param name="quarantineId">ID of the quarantined file</param>
    /// <returns>Quarantined file info if found</returns>
    QuarantinedFileInfo? GetQuarantinedFile(string quarantineId);

    /// <summary>
    /// Loads quarantine metadata from storage
    /// </summary>
    Task LoadMetadataAsync();

    /// <summary>
    /// Saves quarantine metadata to storage
    /// </summary>
    Task SaveMetadataAsync();

    /// <summary>
    /// Gets the total number of quarantined files
    /// </summary>
    int QuarantinedCount { get; }

    /// <summary>
    /// Gets the path to the quarantine folder
    /// </summary>
    string QuarantinePath { get; }
}
