namespace Antivirus.Core.Models;

/// <summary>
/// Represents the type of threat detected
/// Tehdit türünü temsil eder
/// </summary>
public enum ThreatType
{
    Unknown = 0,
    Virus = 1,
    Trojan = 2,
    Worm = 3,
    Ransomware = 4,
    Spyware = 5,
    Adware = 6,
    TestFile = 7  // EICAR gibi test dosyaları için
}

/// <summary>
/// Represents the severity level of a threat
/// Tehdidin ciddiyet seviyesini temsil eder
/// </summary>
public enum ThreatSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

/// <summary>
/// Represents the type of scan operation
/// Tarama işlemi türünü temsil eder
/// </summary>
public enum ScanType
{
    Quick = 1,      // Hızlı tarama - sadece kritik lokasyonlar
    Full = 2,       // Derin tarama - tüm disk
    Custom = 3      // Özel tarama - kullanıcı seçimi
}

/// <summary>
/// Represents the status of a scanned file
/// Taranan dosyanın durumunu temsil eder
/// </summary>
public enum ScanStatus
{
    Clean = 0,      // Temiz
    Infected = 1,   // Zararlı bulundu
    Suspicious = 2, // Şüpheli
    Error = 3       // Tarama hatası
}
