using System.Diagnostics;
using Antivirus.Core.Interfaces;
using Antivirus.Core.Models;

namespace Antivirus.Core.Services;

/// <summary>
/// Asynchronous file scanning engine implementation
/// Asenkron dosya tarama motoru implementasyonu
/// </summary>
public class ScanEngine : IScanEngine
{
    private readonly ISignatureDatabase _signatureDatabase;
    private readonly IQuarantineService _quarantineService;
    private bool _isScanning;

    public bool IsScanning => _isScanning;

    public event EventHandler<FileScanResult>? ThreatDetected;
    public event EventHandler<ScanReport>? ScanCompleted;

    public ScanEngine(ISignatureDatabase signatureDatabase, IQuarantineService quarantineService)
    {
        _signatureDatabase = signatureDatabase ?? throw new ArgumentNullException(nameof(signatureDatabase));
        _quarantineService = quarantineService ?? throw new ArgumentNullException(nameof(quarantineService));
    }

    /// <summary>
    /// Scans a single file for malware
    /// Tek bir dosyayı zararlı yazılım için tarar
    /// </summary>
    public async Task<FileScanResult> ScanFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return FileScanResult.CreateError(filePath, "Dosya yolu boş olamaz.");

        if (!File.Exists(filePath))
            return FileScanResult.CreateError(filePath, "Dosya bulunamadı.");

        try
        {
            var fileInfo = new FileInfo(filePath);
            
            // Skip very large files (over 100MB) for performance
            const long MAX_FILE_SIZE = 100 * 1024 * 1024;
            if (fileInfo.Length > MAX_FILE_SIZE)
            {
                return FileScanResult.CreateClean(filePath, "SKIPPED", fileInfo.Length);
            }

            // Skip empty files
            if (fileInfo.Length == 0)
            {
                return FileScanResult.CreateClean(filePath, "EMPTY", 0);
            }

            // Calculate hash asynchronously
            var hash = await HashCalculator.CalculateSHA256Async(filePath);
            
            // Also check MD5 for compatibility with some signature databases
            var md5Hash = await HashCalculator.CalculateMD5Async(filePath);

            // DEBUG OUTPUT
            Console.WriteLine($"[DEBUG] File: {filePath}");
            Console.WriteLine($"[DEBUG] MD5: {md5Hash}");
            Console.WriteLine($"[DEBUG] SHA256: {hash}");
            Console.WriteLine($"[DEBUG] MD5 in DB: {_signatureDatabase.ContainsHash(md5Hash)}");
            Console.WriteLine($"[DEBUG] SHA256 in DB: {_signatureDatabase.ContainsHash(hash)}");
            Console.WriteLine($"[DEBUG] DB Count: {_signatureDatabase.SignatureCount}");

            // Check against signature database (both SHA256 and MD5)
            if (_signatureDatabase.ContainsHash(hash))
            {
                var threatInfo = _signatureDatabase.GetThreatInfo(hash);
                var result = FileScanResult.CreateInfected(filePath, hash, fileInfo.Length, threatInfo!);
                ThreatDetected?.Invoke(this, result);
                return result;
            }
            
            if (_signatureDatabase.ContainsHash(md5Hash))
            {
                var threatInfo = _signatureDatabase.GetThreatInfo(md5Hash);
                var result = FileScanResult.CreateInfected(filePath, md5Hash, fileInfo.Length, threatInfo!);
                ThreatDetected?.Invoke(this, result);
                return result;
            }

            return FileScanResult.CreateClean(filePath, hash, fileInfo.Length);
        }
        catch (UnauthorizedAccessException)
        {
            return FileScanResult.CreateError(filePath, "Dosyaya erişim izni yok.");
        }
        catch (IOException ex)
        {
            return FileScanResult.CreateError(filePath, $"IO Hatası: {ex.Message}");
        }
        catch (Exception ex)
        {
            return FileScanResult.CreateError(filePath, $"Beklenmeyen hata: {ex.Message}");
        }
    }

    /// <summary>
    /// Scans a directory recursively for malware
    /// Bir klasörü alt klasörler dahil zararlı yazılım için tarar
    /// </summary>
    public async Task<ScanReport> ScanDirectoryAsync(
        string directoryPath,
        IProgress<ScanProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (_isScanning)
            throw new InvalidOperationException("Bir tarama zaten devam ediyor.");

        _isScanning = true;
        var report = new ScanReport
        {
            StartTime = DateTime.Now,
            ScanType = ScanType.Custom,
            ScannedPath = directoryPath
        };

        try
        {
            if (!Directory.Exists(directoryPath))
            {
                report.EndTime = DateTime.Now;
                return report;
            }

            // Get all files recursively
            var files = GetAllFiles(directoryPath);
            report.TotalFilesScanned = files.Count;

            var stopwatch = Stopwatch.StartNew();
            var scannedCount = 0;
            var threatsFound = 0;

            foreach (var filePath in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await ScanFileAsync(filePath);
                scannedCount++;

                if (result.IsInfected)
                {
                    report.InfectedFiles.Add(result);
                    threatsFound++;
                }
                else if (result.Status == ScanStatus.Error)
                {
                    report.ErrorFiles++;
                }
                else
                {
                    report.CleanFiles++;
                }

                report.TotalBytesScanned += result.FileSize;

                // Report progress
                if (progress != null)
                {
                    var elapsed = stopwatch.Elapsed.TotalSeconds;
                    var filesPerSecond = elapsed > 0 ? scannedCount / elapsed : 0;
                    var remainingFiles = files.Count - scannedCount;
                    var estimatedSecondsRemaining = filesPerSecond > 0 
                        ? (int)(remainingFiles / filesPerSecond) 
                        : 0;

                    progress.Report(new ScanProgress
                    {
                        CurrentFile = filePath,
                        TotalFiles = files.Count,
                        ScannedFiles = scannedCount,
                        ThreatsFound = threatsFound,
                        FilesPerSecond = filesPerSecond,
                        EstimatedSecondsRemaining = estimatedSecondsRemaining
                    });
                }

                // Small delay to prevent UI freezing and allow cancellation
                if (scannedCount % 10 == 0)
                {
                    await Task.Delay(1, cancellationToken);
                }
            }

            report.EndTime = DateTime.Now;
            ScanCompleted?.Invoke(this, report);
            return report;
        }
        catch (OperationCanceledException)
        {
            report.WasCancelled = true;
            report.EndTime = DateTime.Now;
            return report;
        }
        finally
        {
            _isScanning = false;
        }
    }

    /// <summary>
    /// Performs a quick scan on common malware locations
    /// Yaygın zararlı yazılım konumlarında hızlı tarama yapar
    /// </summary>
    public async Task<ScanReport> QuickScanAsync(
        IProgress<ScanProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var report = new ScanReport
        {
            StartTime = DateTime.Now,
            ScanType = ScanType.Quick,
            ScannedPath = "Quick Scan"
        };

        _isScanning = true;

        try
        {
            // Common quick scan locations
            var quickScanPaths = new List<string>
            {
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Path.GetTempPath()
            };

            var allFiles = new List<string>();
            foreach (var path in quickScanPaths)
            {
                if (Directory.Exists(path))
                {
                    allFiles.AddRange(GetAllFiles(path, maxDepth: 2)); // Limit depth for quick scan
                }
            }

            report.TotalFilesScanned = allFiles.Count;
            var scannedCount = 0;
            var threatsFound = 0;
            var stopwatch = Stopwatch.StartNew();

            foreach (var filePath in allFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await ScanFileAsync(filePath);
                scannedCount++;

                if (result.IsInfected)
                {
                    report.InfectedFiles.Add(result);
                    threatsFound++;
                }
                else if (result.Status == ScanStatus.Error)
                {
                    report.ErrorFiles++;
                }
                else
                {
                    report.CleanFiles++;
                }

                report.TotalBytesScanned += result.FileSize;

                if (progress != null)
                {
                    var elapsed = stopwatch.Elapsed.TotalSeconds;
                    var filesPerSecond = elapsed > 0 ? scannedCount / elapsed : 0;

                    progress.Report(new ScanProgress
                    {
                        CurrentFile = filePath,
                        TotalFiles = allFiles.Count,
                        ScannedFiles = scannedCount,
                        ThreatsFound = threatsFound,
                        FilesPerSecond = filesPerSecond
                    });
                }

                if (scannedCount % 10 == 0)
                {
                    await Task.Delay(1, cancellationToken);
                }
            }

            report.EndTime = DateTime.Now;
            ScanCompleted?.Invoke(this, report);
            return report;
        }
        catch (OperationCanceledException)
        {
            report.WasCancelled = true;
            report.EndTime = DateTime.Now;
            return report;
        }
        finally
        {
            _isScanning = false;
        }
    }

    /// <summary>
    /// Gets all files in a directory recursively
    /// </summary>
    private List<string> GetAllFiles(string directoryPath, int maxDepth = int.MaxValue)
    {
        var files = new List<string>();
        
        try
        {
            GetFilesRecursive(directoryPath, files, 0, maxDepth);
        }
        catch
        {
            // Ignore access errors
        }

        return files;
    }

    private void GetFilesRecursive(string directoryPath, List<string> files, int currentDepth, int maxDepth)
    {
        if (currentDepth > maxDepth)
            return;

        try
        {
            // Add files in current directory
            files.AddRange(Directory.GetFiles(directoryPath));

            // Recurse into subdirectories
            foreach (var subDir in Directory.GetDirectories(directoryPath))
            {
                try
                {
                    // Skip system directories
                    var dirInfo = new DirectoryInfo(subDir);
                    if ((dirInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                        continue;

                    GetFilesRecursive(subDir, files, currentDepth + 1, maxDepth);
                }
                catch
                {
                    // Ignore access errors for individual directories
                }
            }
        }
        catch
        {
            // Ignore access errors
        }
    }
}
