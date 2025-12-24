using Antivirus.Core.Models;
using Antivirus.Core.Services;
using Xunit;

namespace Antivirus.Tests;

public class QuarantineServiceTests : IDisposable
{
    private readonly QuarantineService _quarantineService;
    private readonly string _quarantineDir;
    private readonly string _sourceDir;

    public QuarantineServiceTests()
    {
        var testId = Guid.NewGuid();
        _quarantineDir = Path.Combine(Path.GetTempPath(), $"QTests_{testId}", "Quarantine");
        _sourceDir = Path.Combine(Path.GetTempPath(), $"QTests_{testId}", "Source");
        
        Directory.CreateDirectory(_quarantineDir);
        Directory.CreateDirectory(_sourceDir);

        _quarantineService = new QuarantineService(_quarantineDir);
    }

    [Fact]
    public void MoveToQuarantine_ShouldMoveFile()
    {
        // Arrange
        var filePath = Path.Combine(_sourceDir, "virus.txt");
        File.WriteAllText(filePath, "MALICIOUS");

        // Act
        var result = _quarantineService.MoveToQuarantine(filePath, new ThreatInfo("Virus", ThreatType.Virus, ThreatSeverity.High, ""));

        // Assert
        Assert.True(result.Success);
        Assert.False(File.Exists(filePath)); // Removed from source
        Assert.True(File.Exists(result.FileInfo.QuarantinePath)); // Exists in quarantine
        Assert.Equal(1, _quarantineService.QuarantinedCount);
    }

    [Fact]
    public void RestoreFromQuarantine_ShouldRestoreFile()
    {
        // Arrange
        var filePath = Path.Combine(_sourceDir, "restore_test.txt");
        File.WriteAllText(filePath, "TO_RESTORE");
        var qResult = _quarantineService.MoveToQuarantine(filePath);
        var qId = qResult.FileInfo.QuarantineId;

        // Act
        var restored = _quarantineService.RestoreFromQuarantine(qId);

        // Assert
        Assert.True(restored);
        Assert.True(File.Exists(filePath)); // Back in source
        Assert.False(File.Exists(qResult.FileInfo.QuarantinePath)); // Gone from quarantine
        Assert.Equal(0, _quarantineService.QuarantinedCount);
    }

    public void Dispose()
    {
        try
        {
            var rootDir = Path.GetDirectoryName(_quarantineDir); // The parent folder
            if (Directory.Exists(rootDir))
                Directory.Delete(rootDir, true);
        }
        catch { }
    }
}
