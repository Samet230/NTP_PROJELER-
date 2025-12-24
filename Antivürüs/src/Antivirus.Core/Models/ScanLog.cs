using System;
using System.Collections.Generic;

namespace Antivirus.Core.Models;

/// <summary>
/// Model for a historical scan record
/// </summary>
public class ScanLog
{
    public Guid LogId { get; set; } = Guid.NewGuid();
    public DateTime ScanTime { get; set; }
    public string ScanType { get; set; } = string.Empty;
    public int TotalFiles { get; set; }
    public int InfectedCount { get; set; }
    public string Summary { get; set; } = string.Empty;
    
    // Summary details of threats found (optional details)
    public List<string> FoundThreats { get; set; } = new();
}
