using System.Globalization;
using FleetManager.Models;

namespace FleetManager.Services;

/// <summary>
/// Araçları satır bazlı, '|' ayraçlı metin formatında okuyup yazar.
/// Format: Tip|Id|Marka|Model|Yıl|GüncelDeğer|SonBakımDeğeri
/// İlk alan (tip) polimorfik nesnelerin doğru türde yeniden oluşturulmasını sağlar.
/// </summary>
public class FileService
{
    private const char Separator = '|';
    private readonly string _filePath;

    public FileService(string filePath = "fleet_data.json")
    {
        _filePath = filePath;
    }

    /// <summary>Araçları dosyaya yazar. Hata olursa IOException yakalanır ve false döner.</summary>
    public bool Save(IEnumerable<Vehicle> vehicles)
    {
        try
        {
            using var writer = new StreamWriter(_filePath, append: false);
            foreach (Vehicle v in vehicles)
            {
                writer.WriteLine(string.Join(Separator,
                    v.TypeName, v.Id, v.Brand, v.Model, v.Year,
                    v.CurrentMileage.ToString(CultureInfo.InvariantCulture),
                    v.LastMaintenanceMileage.ToString(CultureInfo.InvariantCulture)));
            }
            return true;
        }
        catch (IOException ex)
        {
            ConsoleUi.Error($"Dosya yazılırken sorun oluştu: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            ConsoleUi.Error($"Dosyaya yazma izni yok: {ex.Message}");
        }
        return false;
    }

    /// <summary>Araçları dosyadan okur. Bozuk satırlar atlanır; dosya yoksa boş liste döner.</summary>
    public List<Vehicle> Load()
    {
        var vehicles = new List<Vehicle>();
        if (!File.Exists(_filePath)) return vehicles;

        try
        {
            using var reader = new StreamReader(_filePath);
            int lineNumber = 0;
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    vehicles.Add(ParseLine(line));
                }
                catch (Exception ex) when (ex is FormatException or ArgumentException)
                {
                    ConsoleUi.Warning($"Satır {lineNumber} atlandı (bozuk veri): {ex.Message}");
                }
            }
        }
        catch (IOException ex)
        {
            ConsoleUi.Error($"Dosya okunurken sorun oluştu: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            ConsoleUi.Error($"Dosyaya okuma izni yok: {ex.Message}");
        }
        return vehicles;
    }

    // Tek bir satırı doğru araç türüne dönüştürür
    private static Vehicle ParseLine(string line)
    {
        string[] parts = line.Split(Separator);
        if (parts.Length != 7)
            throw new FormatException("Beklenen alan sayısı 7.");

        int id = int.Parse(parts[1], CultureInfo.InvariantCulture);
        string brand = parts[2];
        string model = parts[3];
        int year = int.Parse(parts[4], CultureInfo.InvariantCulture);
        double current = double.Parse(parts[5], CultureInfo.InvariantCulture);
        double last = double.Parse(parts[6], CultureInfo.InvariantCulture);

        return parts[0] switch
        {
            "Car" => new Car(id, brand, model, year, current, last),
            "Motorcycle" => new Motorcycle(id, brand, model, year, current, last),
            "Tractor" => new Tractor(id, brand, model, year, current, last),
            _ => throw new FormatException($"Bilinmeyen araç tipi: {parts[0]}")
        };
    }
}
