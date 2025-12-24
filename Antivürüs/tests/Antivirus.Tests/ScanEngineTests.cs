using Antivirus.Core.Interfaces;
using Antivirus.Core.Models;
using Antivirus.Core.Services;
using Moq;
using Xunit;

namespace Antivirus.Tests;

public class ScanEngineTests : IDisposable
{
    private readonly Mock<ISignatureDatabase> _mockSignatureDb;
    private readonly Mock<IQuarantineService> _mockQuarantineService;
    private readonly ScanEngine _scanEngine;
    private readonly string _testDirectory;

    public ScanEngineTests()
    {
        _mockSignatureDb = new Mock<ISignatureDatabase>();
        _mockQuarantineService = new Mock<IQuarantineService>();
        _scanEngine = new ScanEngine(_mockSignatureDb.Object, _mockQuarantineService.Object);

        // Create a temporary test directory
        _testDirectory = Path.Combine(Path.GetTempPath(), "AntivirusTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public async Task ScanFileAsync_ShouldDetectInfectedFile()
    {
        // Arrange
        var infectedFilePath = Path.Combine(_testDirectory, "infected.txt");
        await File.WriteAllTextAsync(infectedFilePath, "MALWARE_CONTENT");
        var hash = HashCalculator.CalculateSHA256(infectedFilePath);

        var threatInfo = new ThreatInfo("Test.Virus", ThreatType.Virus, ThreatSeverity.High, "Test virus");
        
        _mockSignatureDb.Setup(db => db.ContainsHash(It.IsAny<string>())).Returns(true);
        _mockSignatureDb.Setup(db => db.GetThreatInfo(It.IsAny<string>())).Returns(threatInfo);

        // Act
        var result = await _scanEngine.ScanFileAsync(infectedFilePath);

        // Assert
        Assert.True(result.IsInfected);
        Assert.Equal(ScanStatus.Infected, result.Status);
        Assert.Equal(threatInfo, result.ThreatInfo);
    }

    [Fact]
    public async Task ScanFileAsync_ShouldPassCleanFile()
    {
        // Arrange
        var cleanFilePath = Path.Combine(_testDirectory, "clean.txt");
        await File.WriteAllTextAsync(cleanFilePath, "CLEAN_CONTENT");

        _mockSignatureDb.Setup(db => db.ContainsHash(It.IsAny<string>())).Returns(false);

        // Act
        var result = await _scanEngine.ScanFileAsync(cleanFilePath);

        // Assert
        Assert.False(result.IsInfected);
        Assert.Equal(ScanStatus.Clean, result.Status);
        Assert.Null(result.ThreatInfo);
    }

    [Fact]
    public async Task ScanDirectoryAsync_ShouldFindMixOfFiles()
    {
        // Arrange
        var cleanFile = Path.Combine(_testDirectory, "clean.txt");
        var infectedFile = Path.Combine(_testDirectory, "infected.txt");
        
        await File.WriteAllTextAsync(cleanFile, "CLEAN");
        await File.WriteAllTextAsync(infectedFile, "INFECTED");

        var infectedHash = await HashCalculator.CalculateSHA256Async(infectedFile);
        
        _mockSignatureDb.Setup(db => db.ContainsHash(It.Is<string>(h => h == infectedHash))).Returns(true);
        _mockSignatureDb.Setup(db => db.GetThreatInfo(It.Is<string>(h => h == infectedHash)))
            .Returns(new ThreatInfo("Test.Virus", ThreatType.Virus, ThreatSeverity.Medium, ""));

        // Act
        var report = await _scanEngine.ScanDirectoryAsync(_testDirectory);

        // Assert
        Assert.Equal(2, report.TotalFilesScanned);
        Assert.Equal(1, report.CleanFiles);
        Assert.Equal(1, report.InfectedCount);
        Assert.Contains(report.InfectedFiles, f => f.FilePath == infectedFile);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }
}
