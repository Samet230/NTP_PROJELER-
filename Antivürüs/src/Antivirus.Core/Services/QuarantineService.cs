using System.Text.Json;
using Antivirus.Core.Interfaces;
using Antivirus.Core.Models;

namespace Antivirus.Core.Services;

/// <summary>
/// Service for managing quarantined files
/// Karantinaya alınan dosyaları yöneten servis
/// </summary>
public class QuarantineService : IQuarantineService
{
    private readonly string _quarantinePath;
    private readonly string _metadataFilePath;
    private readonly List<QuarantinedFileInfo> _quarantinedFiles = new();
    private readonly object _lockObject = new();

    public int QuarantinedCount => _quarantinedFiles.Count;
    public string QuarantinePath => _quarantinePath;

    public QuarantineService(string quarantinePath)
    {
        _quarantinePath = quarantinePath;
        _metadataFilePath = Path.Combine(quarantinePath, "quarantine_metadata.json");
        
        // Ensure quarantine directory exists
        if (!Directory.Exists(_quarantinePath))
        {
            Directory.CreateDirectory(_quarantinePath);
        }
    }

    public QuarantineResult MoveToQuarantine(string filePath, ThreatInfo? threatInfo = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return QuarantineResult.Failed("Dosya yolu boş olamaz.");

        if (!File.Exists(filePath))
            return QuarantineResult.Failed($"Dosya bulunamadı: {filePath}");

        try
        {
            // Generate unique quarantine ID
            var quarantineId = Guid.NewGuid().ToString();
            var originalFileName = Path.GetFileName(filePath);
            
            // Create quarantine filename with ID to avoid conflicts
            // Also add .quarantine extension to prevent accidental execution
            var quarantineFileName = $"{quarantineId}_{originalFileName}.quarantine";
            var quarantinedFilePath = Path.Combine(_quarantinePath, quarantineFileName);

            // Get file info before moving
            var fileInfo = new FileInfo(filePath);
            var fileSize = fileInfo.Length;
            var fileHash = HashCalculator.CalculateSHA256(filePath);

            // Move file to quarantine
            File.Move(filePath, quarantinedFilePath);

            // Create quarantine record
            var quarantinedFile = new QuarantinedFileInfo
            {
                QuarantineId = quarantineId,
                OriginalPath = filePath,
                OriginalFileName = originalFileName,
                QuarantinePath = quarantinedFilePath,
                FileHash = fileHash,
                FileSize = fileSize,
                ThreatInfo = threatInfo,
                QuarantineDate = DateTime.Now
            };

            lock (_lockObject)
            {
                _quarantinedFiles.Add(quarantinedFile);
            }

            // Save metadata asynchronously (fire and forget for now)
            _ = SaveMetadataAsync();

            return QuarantineResult.Succeeded(quarantinedFile, 
                $"'{originalFileName}' dosyası başarıyla karantinaya alındı.");
        }
        catch (UnauthorizedAccessException)
        {
            return QuarantineResult.Failed($"Dosyaya erişim izni yok: {filePath}");
        }
        catch (IOException ex)
        {
            return QuarantineResult.Failed($"Dosya taşınırken hata oluştu: {ex.Message}");
        }
        catch (Exception ex)
        {
            return QuarantineResult.Failed($"Beklenmeyen hata: {ex.Message}");
        }
    }

    public bool RestoreFromQuarantine(string quarantineId)
    {
        if (string.IsNullOrWhiteSpace(quarantineId))
            return false;

        QuarantinedFileInfo? fileInfo;
        lock (_lockObject)
        {
            fileInfo = _quarantinedFiles.FirstOrDefault(f => f.QuarantineId == quarantineId);
        }

        if (fileInfo == null)
            return false;

        if (!File.Exists(fileInfo.QuarantinePath))
            return false;

        try
        {
            // Ensure original directory exists
            var originalDir = Path.GetDirectoryName(fileInfo.OriginalPath);
            if (!string.IsNullOrEmpty(originalDir) && !Directory.Exists(originalDir))
            {
                Directory.CreateDirectory(originalDir);
            }

            // Move file back to original location
            File.Move(fileInfo.QuarantinePath, fileInfo.OriginalPath);

            lock (_lockObject)
            {
                _quarantinedFiles.Remove(fileInfo);
            }

            _ = SaveMetadataAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DeletePermanently(string quarantineId)
    {
        if (string.IsNullOrWhiteSpace(quarantineId))
            return false;

        QuarantinedFileInfo? fileInfo;
        lock (_lockObject)
        {
            fileInfo = _quarantinedFiles.FirstOrDefault(f => f.QuarantineId == quarantineId);
        }

        if (fileInfo == null)
            return false;

        try
        {
            if (File.Exists(fileInfo.QuarantinePath))
            {
                File.Delete(fileInfo.QuarantinePath);
            }

            lock (_lockObject)
            {
                _quarantinedFiles.Remove(fileInfo);
            }

            _ = SaveMetadataAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public List<QuarantinedFileInfo> GetQuarantinedFiles()
    {
        lock (_lockObject)
        {
            return _quarantinedFiles.ToList();
        }
    }

    public QuarantinedFileInfo? GetQuarantinedFile(string quarantineId)
    {
        lock (_lockObject)
        {
            return _quarantinedFiles.FirstOrDefault(f => f.QuarantineId == quarantineId);
        }
    }

    public async Task LoadMetadataAsync()
    {
        if (!File.Exists(_metadataFilePath))
            return;

        try
        {
            var json = await File.ReadAllTextAsync(_metadataFilePath);
            var loadedFiles = JsonSerializer.Deserialize<List<QuarantinedFileInfo>>(json);
            
            if (loadedFiles != null)
            {
                lock (_lockObject)
                {
                    _quarantinedFiles.Clear();
                    _quarantinedFiles.AddRange(loadedFiles);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Karantina metadata yüklenirken hata: {ex.Message}");
        }
    }

    public async Task SaveMetadataAsync()
    {
        try
        {
            List<QuarantinedFileInfo> filesToSave;
            lock (_lockObject)
            {
                filesToSave = _quarantinedFiles.ToList();
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(filesToSave, options);
            await File.WriteAllTextAsync(_metadataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Karantina metadata kaydedilirken hata: {ex.Message}");
        }
    }
}
