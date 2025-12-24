using System.Text.Json;
using Antivirus.Core.Interfaces;
using Antivirus.Core.Models;

namespace Antivirus.Core.Services;

/// <summary>
/// JSON-based signature database implementation
/// JSON tabanlı imza veritabanı implementasyonu
/// </summary>
public class SignatureDatabase : ISignatureDatabase
{
    private readonly Dictionary<string, ThreatInfo> _signatures = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _signatureFilePath;
    private DateTime _lastUpdated = DateTime.Now;

    // EICAR Test File Standard Hashes
    // EICAR test string: X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*
    private const string EICAR_MD5 = "44d88612fea8a8f36de82e1278abb02f";
    private const string EICAR_SHA256 = "275a021bbfb6489e54d471899f7db9d1663fc695ec2fe2a2c4538aabf651fd0f";

    public int SignatureCount => _signatures.Count;
    public DateTime LastUpdated => _lastUpdated;

    public SignatureDatabase(string signatureFilePath)
    {
        _signatureFilePath = signatureFilePath;
        InitializeDefaultSignatures();
    }

    /// <summary>
    /// Initializes the database with default signatures including EICAR test file
    /// </summary>
    private void InitializeDefaultSignatures()
    {
        var eicarThreatInfo = new ThreatInfo(
            "EICAR-Test-File",
            ThreatType.TestFile,
            ThreatSeverity.Low,
            "EICAR Antivirüs Test Dosyası - Bu gerçek bir tehdit değildir, sadece antivirüs yazılımlarını test etmek için kullanılır."
        );

        // Add both MD5 and SHA256 hashes for EICAR test file
        _signatures[EICAR_MD5] = eicarThreatInfo;
        _signatures[EICAR_SHA256] = eicarThreatInfo;

        // Add some sample malware signatures for demonstration
        var sampleVirus = new ThreatInfo(
            "Win32.Sample.Virus",
            ThreatType.Virus,
            ThreatSeverity.High,
            "Örnek virüs imzası - Simülasyon amaçlı"
        );
        _signatures["e99a18c428cb38d5f260853678922e03"] = sampleVirus;

        var sampleTrojan = new ThreatInfo(
            "Trojan.Generic.Backdoor",
            ThreatType.Trojan,
            ThreatSeverity.Critical,
            "Örnek trojan imzası - Simülasyon amaçlı"
        );
        _signatures["d41d8cd98f00b204e9800998ecf8427e"] = sampleTrojan;

        // Custom test virus - Calculate hash from known content
        // Simple single-line content to avoid encoding issues
        var customTestVirus = new ThreatInfo(
            "CustomTest.Malware.Simulation",
            ThreatType.Virus,
            ThreatSeverity.High,
            "Özel test virüsü - Antivirüs simülasyonu test dosyası"
        );
        
        // Simple test content (single line, WITH trailing CRLF added by file writer)
        var testContent = "ANTIVIRUS-TEST-VIRUS-FILE-2024\r\n";
        var testMd5 = CalculateMD5FromContent(testContent);
        var testSha256 = CalculateSHA256FromContent(testContent);
        Console.WriteLine($"[INIT] Test virus MD5: {testMd5}");
        Console.WriteLine($"[INIT] Test virus SHA256: {testSha256}");
        _signatures[testMd5] = customTestVirus;
        _signatures[testSha256] = customTestVirus;
    }

    private static string CalculateMD5FromContent(string content)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var hashBytes = md5.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    private static string CalculateSHA256FromContent(string content)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public bool ContainsHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return false;

        return _signatures.ContainsKey(hash);
    }

    public ThreatInfo? GetThreatInfo(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return null;

        return _signatures.TryGetValue(hash, out var threatInfo) ? threatInfo : null;
    }

    public void AddSignature(string hash, ThreatInfo threatInfo)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Hash değeri boş olamaz.", nameof(hash));
        
        if (threatInfo == null)
            throw new ArgumentNullException(nameof(threatInfo));

        _signatures[hash] = threatInfo;
        _lastUpdated = DateTime.Now;
    }

    public bool RemoveSignature(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return false;

        var removed = _signatures.Remove(hash);
        if (removed)
            _lastUpdated = DateTime.Now;
        
        return removed;
    }

    public async Task LoadSignaturesAsync()
    {
        if (!File.Exists(_signatureFilePath))
        {
            // If file doesn't exist, save current signatures (with defaults)
            await SaveSignaturesAsync();
            return;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_signatureFilePath);
            var loadedData = JsonSerializer.Deserialize<SignatureDatabaseDto>(json);
            
            if (loadedData?.Signatures != null)
            {
                foreach (var kvp in loadedData.Signatures)
                {
                    _signatures[kvp.Key] = kvp.Value;
                }
                _lastUpdated = loadedData.LastUpdated;
            }
        }
        catch (JsonException ex)
        {
            // Log error and continue with default signatures
            Console.WriteLine($"İmza veritabanı yüklenirken hata: {ex.Message}");
        }
    }

    public async Task SaveSignaturesAsync()
    {
        try
        {
            var directory = Path.GetDirectoryName(_signatureFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var dto = new SignatureDatabaseDto
            {
                Signatures = _signatures,
                LastUpdated = _lastUpdated
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(dto, options);
            await File.WriteAllTextAsync(_signatureFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"İmza veritabanı kaydedilirken hata: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// DTO for serializing signature database
    /// </summary>
    private class SignatureDatabaseDto
    {
        public Dictionary<string, ThreatInfo> Signatures { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }
}
