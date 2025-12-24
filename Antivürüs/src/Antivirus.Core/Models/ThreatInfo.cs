namespace Antivirus.Core.Models;

/// <summary>
/// Contains detailed information about a detected threat
/// Tespit edilen tehdit hakkında detaylı bilgi içerir
/// </summary>
public class ThreatInfo
{
    /// <summary>
    /// Name of the threat (e.g., "EICAR-Test-File")
    /// </summary>
    public string ThreatName { get; set; } = string.Empty;

    /// <summary>
    /// Category of the threat
    /// </summary>
    public ThreatType ThreatType { get; set; } = ThreatType.Unknown;

    /// <summary>
    /// Severity level of the threat
    /// </summary>
    public ThreatSeverity Severity { get; set; } = ThreatSeverity.Low;

    /// <summary>
    /// Detailed description of the threat
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Date when this signature was added to the database
    /// </summary>
    public DateTime DateAdded { get; set; } = DateTime.Now;

    public ThreatInfo() { }

    public ThreatInfo(string threatName, ThreatType threatType, ThreatSeverity severity, string description)
    {
        ThreatName = threatName;
        ThreatType = threatType;
        Severity = severity;
        Description = description;
        DateAdded = DateTime.Now;
    }
}
