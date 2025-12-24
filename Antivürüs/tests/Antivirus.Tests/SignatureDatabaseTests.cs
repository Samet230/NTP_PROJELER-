using Antivirus.Core.Models;
using Antivirus.Core.Services;
using Xunit;

namespace Antivirus.Tests;

public class SignatureDatabaseTests : IDisposable
{
    private readonly SignatureDatabase _database;
    private readonly string _testSignatureFile;
    private readonly string _testDirectory;

    public SignatureDatabaseTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "AntivirusSigTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_testDirectory);
        _testSignatureFile = Path.Combine(_testDirectory, "signatures.json");
        _database = new SignatureDatabase(_testSignatureFile);
    }

    [Fact]
    public void ContainsHash_ShouldReturnTrue_ForEicarHash()
    {
        // EICAR check (MD5)
        Assert.True(_database.ContainsHash("44d88612fea8a8f36de82e1278abb02f"));
    }

    [Fact]
    public void AddSignature_ShouldAddAndPersist()
    {
        // Arrange
        var newHash = "newinvalidhashtest123";
        var threatWithHash = new ThreatInfo("NewThreat", ThreatType.Trojan, ThreatSeverity.High, "Description");

        // Act
        _database.AddSignature(newHash, threatWithHash);

        // Assert
        Assert.True(_database.ContainsHash(newHash));
        var retrieved = _database.GetThreatInfo(newHash);
        Assert.NotNull(retrieved);
        Assert.Equal("NewThreat", retrieved.ThreatName);
    }

    [Fact]
    public async Task SaveAndLoad_ShouldPersistData()
    {
        // Arrange
        var newHash = "persistenthash123";
        var threatWithHash = new ThreatInfo("PersistentThreat", ThreatType.Spyware, ThreatSeverity.Medium, "");
        _database.AddSignature(newHash, threatWithHash);

        // Act
        await _database.SaveSignaturesAsync();

        // New instance to load from file
        var newDbInstance = new SignatureDatabase(_testSignatureFile);
        await newDbInstance.LoadSignaturesAsync();

        // Assert
        Assert.True(newDbInstance.ContainsHash(newHash));
        Assert.Equal("PersistentThreat", newDbInstance.GetThreatInfo(newHash)?.ThreatName);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }
}
