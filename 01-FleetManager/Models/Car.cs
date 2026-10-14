namespace FleetManager.Models;

/// <summary>Otomobil: her 15.000 km'de bir bakım.</summary>
public class Car : Vehicle
{
    private const double MaintenanceInterval = 15_000;
    private const decimal BaseCost = 2_500m;
    private const decimal OverduePenaltyPerKm = 0.05m;

    public Car(int id, string brand, string model, int year, double currentMileage, double lastMaintenanceMileage)
        : base(id, brand, model, year, currentMileage, lastMaintenanceMileage) { }

    public override string TypeName => "Car";

    public override MaintenanceStatus CheckMaintenanceStatus()
    {
        if (UsageSinceMaintenance >= MaintenanceInterval) return MaintenanceStatus.Due;
        if (UsageSinceMaintenance >= MaintenanceInterval * 0.9) return MaintenanceStatus.Approaching;
        return MaintenanceStatus.Ok;
    }

    // Sabit ücret + gecikilen her km için ceza
    public override decimal CalculateMaintenanceCost()
    {
        double overdue = Math.Max(0, UsageSinceMaintenance - MaintenanceInterval);
        return BaseCost + (decimal)overdue * OverduePenaltyPerKm;
    }
}
