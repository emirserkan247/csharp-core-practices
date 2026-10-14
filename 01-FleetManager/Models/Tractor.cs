namespace FleetManager.Models;

/// <summary>Traktör: çalışma saati bazlı, ağır hizmet çarpanlı bakım.</summary>
public class Tractor : Vehicle
{
    private const double MaintenanceIntervalHours = 250;
    private const decimal BaseCost = 3_000m;
    private const decimal CostPerHour = 2m;
    private const decimal HeavyDutyMultiplier = 1.8m;

    public Tractor(int id, string brand, string model, int year, double currentHours, double lastMaintenanceHours)
        : base(id, brand, model, year, currentHours, lastMaintenanceHours) { }

    public override string TypeName => "Tractor";

    public override string UsageUnit => "saat";

    public override MaintenanceStatus CheckMaintenanceStatus()
    {
        if (UsageSinceMaintenance >= MaintenanceIntervalHours) return MaintenanceStatus.Due;
        if (UsageSinceMaintenance >= MaintenanceIntervalHours * 0.9) return MaintenanceStatus.Approaching;
        return MaintenanceStatus.Ok;
    }

    // (Sabit ücret + çalışılan saat başı ücret) * ağır hizmet çarpanı
    public override decimal CalculateMaintenanceCost() =>
        (BaseCost + (decimal)UsageSinceMaintenance * CostPerHour) * HeavyDutyMultiplier;
}
