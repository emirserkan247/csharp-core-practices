namespace FleetManager.Models;

/// <summary>Bir aracın bakım durumunu ifade eder.</summary>
public enum MaintenanceStatus
{
    /// <summary>Bakıma daha var.</summary>
    Ok,

    /// <summary>Bakım zamanı yaklaşıyor.</summary>
    Approaching,

    /// <summary>Bakım zamanı geldi veya geçti.</summary>
    Due
}
