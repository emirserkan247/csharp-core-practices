using FleetManager.Models;

namespace FleetManager.Services;

/// <summary>Araç koleksiyonunu yöneten servis.</summary>
public class FleetService
{
    private readonly List<Vehicle> _vehicles = new();

    public int Count => _vehicles.Count;

    public IReadOnlyList<Vehicle> Vehicles => _vehicles;

    /// <summary>Yeni araç için bir sonraki kimliği üretir.</summary>
    public int GetNextId()
    {
        int maxId = 0;
        foreach (Vehicle vehicle in _vehicles)
        {
            if (vehicle.Id > maxId) maxId = vehicle.Id;
        }
        return maxId + 1;
    }

    public void AddVehicle(Vehicle vehicle) => _vehicles.Add(vehicle);

    public void LoadVehicles(IEnumerable<Vehicle> vehicles)
    {
        _vehicles.Clear();
        _vehicles.AddRange(vehicles);
    }

    public Vehicle? FindById(int id)
    {
        for (int i = 0; i < _vehicles.Count; i++)
        {
            if (_vehicles[i].Id == id) return _vehicles[i];
        }
        return null;
    }

    /// <summary>Tüm filoyu konsola listeler.</summary>
    public void ListAll()
    {
        if (_vehicles.Count == 0)
        {
            ConsoleUi.Warning("Filoda kayıtlı araç yok.");
            return;
        }

        foreach (Vehicle vehicle in _vehicles)
        {
            ConsoleUi.PrintVehicle(vehicle);
        }
    }

    /// <summary>Bakımı gelen (Due) araçları döndürür.</summary>
    public List<Vehicle> GetVehiclesDueForMaintenance()
    {
        var result = new List<Vehicle>();
        foreach (Vehicle vehicle in _vehicles)
        {
            if (vehicle.CheckMaintenanceStatus() == MaintenanceStatus.Due)
                result.Add(vehicle);
        }
        return result;
    }

    /// <summary>Bakımı gelen araçların toplam bakım maliyetini hesaplar.</summary>
    public decimal CalculateTotalMaintenanceCost()
    {
        decimal total = 0;
        foreach (Vehicle vehicle in GetVehiclesDueForMaintenance())
        {
            total += vehicle.CalculateMaintenanceCost();
        }
        return total;
    }
}
