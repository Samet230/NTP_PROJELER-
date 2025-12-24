using System.Security.Cryptography;

namespace Antivirus.Core.Services;

/// <summary>
/// Static helper class for calculating file hashes
/// Dosya hash'lerini hesaplamak için statik yardımcı sınıf
/// </summary>
public static class HashCalculator
{
    /// <summary>
    /// Buffer size for reading files (64KB)
    /// </summary>
    private const int BUFFER_SIZE = 65536;

    /// <summary>
    /// Calculates MD5 hash of a file synchronously
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <returns>MD5 hash as lowercase hex string</returns>
    public static string CalculateMD5(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE);
        var hashBytes = md5.ComputeHash(stream);
        return ConvertToHexString(hashBytes);
    }

    /// <summary>
    /// Calculates MD5 hash of a file asynchronously
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>MD5 hash as lowercase hex string</returns>
    public static async Task<string> CalculateMD5Async(string filePath, CancellationToken cancellationToken = default)
    {
        using var md5 = MD5.Create();
        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, useAsync: true);
        var hashBytes = await md5.ComputeHashAsync(stream, cancellationToken);
        return ConvertToHexString(hashBytes);
    }

    /// <summary>
    /// Calculates SHA256 hash of a file synchronously
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <returns>SHA256 hash as lowercase hex string</returns>
    public static string CalculateSHA256(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE);
        var hashBytes = sha256.ComputeHash(stream);
        return ConvertToHexString(hashBytes);
    }

    /// <summary>
    /// Calculates SHA256 hash of a file asynchronously
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>SHA256 hash as lowercase hex string</returns>
    public static async Task<string> CalculateSHA256Async(string filePath, CancellationToken cancellationToken = default)
    {
        using var sha256 = SHA256.Create();
        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, useAsync: true);
        var hashBytes = await sha256.ComputeHashAsync(stream, cancellationToken);
        return ConvertToHexString(hashBytes);
    }

    /// <summary>
    /// Calculates both MD5 and SHA256 hashes asynchronously
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple with (MD5, SHA256) hashes</returns>
    public static async Task<(string MD5, string SHA256)> CalculateBothHashesAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var md5Task = CalculateMD5Async(filePath, cancellationToken);
        var sha256Task = CalculateSHA256Async(filePath, cancellationToken);
        
        await Task.WhenAll(md5Task, sha256Task);
        
        return (await md5Task, await sha256Task);
    }

    /// <summary>
    /// Calculates MD5 hash from a string (useful for EICAR test)
    /// </summary>
    /// <param name="content">String content</param>
    /// <returns>MD5 hash as lowercase hex string</returns>
    public static string CalculateMD5FromString(string content)
    {
        using var md5 = MD5.Create();
        var bytes = System.Text.Encoding.ASCII.GetBytes(content);
        var hashBytes = md5.ComputeHash(bytes);
        return ConvertToHexString(hashBytes);
    }

    /// <summary>
    /// Calculates SHA256 hash from a string (useful for EICAR test)
    /// </summary>
    /// <param name="content">String content</param>
    /// <returns>SHA256 hash as lowercase hex string</returns>
    public static string CalculateSHA256FromString(string content)
    {
        using var sha256 = SHA256.Create();
        var bytes = System.Text.Encoding.ASCII.GetBytes(content);
        var hashBytes = sha256.ComputeHash(bytes);
        return ConvertToHexString(hashBytes);
    }

    /// <summary>
    /// Converts byte array to lowercase hex string
    /// </summary>
    private static string ConvertToHexString(byte[] bytes)
    {
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
