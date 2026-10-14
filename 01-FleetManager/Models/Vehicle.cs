namespace FleetManager.Models;

/// <summary>Tüm araç türlerinin ortak soyut ana sınıfı.</summary>
public abstract class Vehicle
{
    private int _year;
    private double _currentMileage;
    private double _lastMaintenanceMileage;

    protected Vehicle(int id, string brand, string model, int year, double currentMileage, double lastMaintenanceMileage)
    {
        Id = id;
        Brand = brand;
        Model = model;
        Year = year;
        _currentMileage = ValidateNonNegative(currentMileage, nameof(currentMileage));
        _lastMaintenanceMileage = ValidateNonNegative(lastMaintenanceMileage, nameof(lastMaintenanceMileage));
    }

    public int Id { get; }
    public string Brand { get; }
    public string Model { get; }

    /// <summary>Model yılı. Geçersiz değerlerde ArgumentOutOfRangeException fırlatır.</summary>
    public int Year
    {
        get => _year;
        private set
        {
            if (value < 1900 || value > DateTime.Now.Year + 1)
                throw new ArgumentOutOfRangeException(nameof(Year), $"Yıl 1900 ile {DateTime.Now.Year + 1} arasında olmalıdır.");
            _year = value;
        }
    }

    /// <summary>Güncel kilometre (traktörde çalışma saati).</summary>
    public double CurrentMileage
    {
        get => _currentMileage;
        set
        {
            ValidateNonNegative(value, nameof(CurrentMileage));
            if (value < _lastMaintenanceMileage)
                throw new ArgumentOutOfRangeException(nameof(CurrentMileage), "Güncel değer son bakım değerinden küçük olamaz.");
            _currentMileage = value;
        }
    }

    public double LastMaintenanceMileage => _lastMaintenanceMileage;

    /// <summary>Son bakımdan bu yana kullanılan miktar.</summary>
    public double UsageSinceMaintenance => _currentMileage - _lastMaintenanceMileage;

    /// <summary>Kullanım biriminin adı (km, saat).</summary>
    public virtual string UsageUnit => "km";

    /// <summary>Dosyaya yazarken kullanılan tip etiketi.</summary>
    public abstract string TypeName { get; }

    /// <summary>Türe özgü bakım maliyetini hesaplar.</summary>
    public abstract decimal CalculateMaintenanceCost();

    /// <summary>Türe özgü bakım durumunu belirler.</summary>
    public abstract MaintenanceStatus CheckMaintenanceStatus();

    /// <summary>Bakım yapıldı: son bakım değerini güncel değere eşitler.</summary>
    public void PerformMaintenance() => _lastMaintenanceMileage = _currentMileage;

    public override string ToString() =>
        $"#{Id,-3} {TypeName,-10} {Brand} {Model} ({Year}) | {_currentMileage:N0} {UsageUnit} | Son bakım: {_lastMaintenanceMileage:N0} {UsageUnit}";

    protected static double ValidateNonNegative(double value, string paramName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, "Değer negatif olamaz.");
        return value;
    }
}
