namespace FleetManager.Models;

/// <summary>Motosiklet: her 5.000 km'de bir bakım.</summary>
public class Motorcycle : Vehicle
{
    private const double MaintenanceInterval = 5_000;
    private const decimal BaseCost = 1_200m;
    private const decimal OverduePenaltyPerKm = 0.08m;

    public Motorcycle(int id, string brand, string model, int year, double currentMileage, double lastMaintenanceMileage)
        : base(id, brand, model, year, currentMileage, lastMaintenanceMileage) { }

    public override string TypeName => "Motorcycle";

    public override MaintenanceStatus CheckMaintenanceStatus()
    {
        if (UsageSinceMaintenance >= MaintenanceInterval) return MaintenanceStatus.Due;
        if (UsageSinceMaintenance >= MaintenanceInterval * 0.9) return MaintenanceStatus.Approaching;
        return MaintenanceStatus.Ok;
    }

    // Sabit ücret + gecikilen her km için (daha yüksek) ceza
    public override decimal CalculateMaintenanceCost()
    {
        double overdue = Math.Max(0, UsageSinceMaintenance - MaintenanceInterval);
        return BaseCost + (decimal)overdue * OverduePenaltyPerKm;
    }
}
