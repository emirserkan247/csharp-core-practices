using FleetManager.Models;
using FleetManager.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var fleetService = new FleetService();
var fileService = new FileService();

// Program açılışında kayıtlı araçları yükle
fleetService.LoadVehicles(fileService.Load());
ConsoleUi.Info($"{fleetService.Count} araç dosyadan yüklendi.");

bool running = true;
while (running)
{
    try
    {
        PrintMenu();
        int choice = ConsoleUi.ReadInt("Seçiminiz: ", 1, 6);
        Console.WriteLine();

        switch (choice)
        {
            case 1: AddVehicle(); break;
            case 2: fleetService.ListAll(); break;
            case 3: ShowDueVehicles(); break;
            case 4: UpdateUsage(); break;
            case 5: PerformMaintenance(); break;
            case 6:
                if (fileService.Save(fleetService.Vehicles))
                    ConsoleUi.Success("Veriler kaydedildi. Görüşmek üzere!");
                running = false;
                break;
        }
    }
    catch (ArgumentOutOfRangeException ex)
    {
        ConsoleUi.Error(ex.Message.Split('\n')[0]);
    }
    catch (FormatException ex)
    {
        ConsoleUi.Error(ex.Message);
    }
    catch (IOException ex)
    {
        ConsoleUi.Error($"Dosya hatası: {ex.Message}");
    }

    if (running)
    {
        Console.WriteLine("\nDevam etmek için bir tuşa basın...");
        Console.ReadKey(true);
    }
}

void PrintMenu()
{
    Console.Clear();
    ConsoleUi.WriteColored("╔══════════════════════════════════════╗", ConsoleColor.Cyan);
    ConsoleUi.WriteColored("║   ARAÇ FİLOSU VE BAKIM TAKİP SİSTEMİ ║", ConsoleColor.Cyan);
    ConsoleUi.WriteColored("╚══════════════════════════════════════╝", ConsoleColor.Cyan);
    Console.WriteLine(" [1] Araç Ekle");
    Console.WriteLine(" [2] Tüm Filoyu Listele");
    Console.WriteLine(" [3] Bakımı Gelen Araçları Göster");
    Console.WriteLine(" [4] Kilometre/Kullanım Güncelle");
    Console.WriteLine(" [5] Bakım Yap");
    Console.WriteLine(" [6] Kaydet ve Çık");
    Console.WriteLine();
}

void AddVehicle()
{
    Console.WriteLine("Araç türü: [1] Araba  [2] Motosiklet  [3] Traktör");
    int type = ConsoleUi.ReadInt("Tür seçimi: ", 1, 3);

    string brand = ConsoleUi.ReadText("Marka: ");
    string model = ConsoleUi.ReadText("Model: ");
    int year = ConsoleUi.ReadInt("Yıl: ", 1900, DateTime.Now.Year + 1);
    string unit = type == 3 ? "çalışma saati" : "kilometre";
    double usage = ConsoleUi.ReadNonNegativeDouble($"Güncel {unit}: ");
    double lastMaintenance = ConsoleUi.ReadNonNegativeDouble($"Son bakım yapıldığındaki {unit}: ");

    if (lastMaintenance > usage)
        throw new ArgumentOutOfRangeException(nameof(lastMaintenance), "Son bakım değeri güncel değerden büyük olamaz.");

    int id = fleetService.GetNextId();
    Vehicle vehicle = type switch
    {
        1 => new Car(id, brand, model, year, usage, lastMaintenance),
        2 => new Motorcycle(id, brand, model, year, usage, lastMaintenance),
        _ => new Tractor(id, brand, model, year, usage, lastMaintenance)
    };

    fleetService.AddVehicle(vehicle);
    ConsoleUi.Success($"Araç eklendi (Id: {id}).");
}

void ShowDueVehicles()
{
    List<Vehicle> dueVehicles = fleetService.GetVehiclesDueForMaintenance();
    if (dueVehicles.Count == 0)
    {
        ConsoleUi.Success("Bakımı gelen araç yok.");
        return;
    }

    for (int i = 0; i < dueVehicles.Count; i++)
    {
        Console.Write($"{i + 1}) ");
        ConsoleUi.PrintVehicle(dueVehicles[i]);
    }
    ConsoleUi.Warning($"\nToplam bakım maliyeti: {fleetService.CalculateTotalMaintenanceCost():N2} TL");
}

void UpdateUsage()
{
    Vehicle? vehicle = SelectVehicle();
    if (vehicle == null) return;

    double newValue = ConsoleUi.ReadNonNegativeDouble($"Yeni değer ({vehicle.UsageUnit}): ");
    vehicle.CurrentMileage = newValue; // Geçersizse ArgumentOutOfRangeException fırlatır
    ConsoleUi.Success("Güncellendi.");
}

void PerformMaintenance()
{
    Vehicle? vehicle = SelectVehicle();
    if (vehicle == null) return;

    vehicle.PerformMaintenance();
    ConsoleUi.Success($"{vehicle.Brand} {vehicle.Model} için bakım kaydedildi.");
}

Vehicle? SelectVehicle()
{
    if (fleetService.Count == 0)
    {
        ConsoleUi.Warning("Filoda kayıtlı araç yok.");
        return null;
    }

    fleetService.ListAll();
    int id = ConsoleUi.ReadInt("\nAraç Id: ", 1);
    Vehicle? vehicle = fleetService.FindById(id);
    if (vehicle == null) ConsoleUi.Error("Bu Id ile araç bulunamadı.");
    return vehicle;
}
