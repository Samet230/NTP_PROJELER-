using Antivirus.Core.Models;

namespace Antivirus.Core.Interfaces;

/// <summary>
/// Interface for the signature database that stores known malware hashes
/// Bilinen zararlı yazılım hash'lerini saklayan imza veritabanı arayüzü
/// </summary>
public interface ISignatureDatabase
{
    /// <summary>
    /// Checks if the given hash exists in the signature database
    /// </summary>
    /// <param name="hash">SHA256 or MD5 hash to check</param>
    /// <returns>True if hash is found (file is malware)</returns>
    bool ContainsHash(string hash);

    /// <summary>
    /// Gets threat information for a given hash
    /// </summary>
    /// <param name="hash">Hash to lookup</param>
    /// <returns>ThreatInfo if found, null otherwise</returns>
    ThreatInfo? GetThreatInfo(string hash);

    /// <summary>
    /// Adds a new signature to the database
    /// </summary>
    /// <param name="hash">Hash of the malware</param>
    /// <param name="threatInfo">Information about the threat</param>
    void AddSignature(string hash, ThreatInfo threatInfo);

    /// <summary>
    /// Removes a signature from the database
    /// </summary>
    /// <param name="hash">Hash to remove</param>
    /// <returns>True if removed successfully</returns>
    bool RemoveSignature(string hash);

    /// <summary>
    /// Loads signatures from persistent storage
    /// </summary>
    Task LoadSignaturesAsync();

    /// <summary>
    /// Saves signatures to persistent storage
    /// </summary>
    Task SaveSignaturesAsync();

    /// <summary>
    /// Gets the total number of signatures in the database
    /// </summary>
    int SignatureCount { get; }

    /// <summary>
    /// Gets the date of the last database update
    /// </summary>
    DateTime LastUpdated { get; }
}
