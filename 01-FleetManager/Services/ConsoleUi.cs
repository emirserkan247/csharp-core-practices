using FleetManager.Models;

namespace FleetManager.Services;

/// <summary>Renkli çıktı ve güvenli kullanıcı girişi yardımcıları.</summary>
public static class ConsoleUi
{
    public static void Success(string message) => WriteColored(message, ConsoleColor.Green);
    public static void Error(string message) => WriteColored("HATA: " + message, ConsoleColor.Red);
    public static void Warning(string message) => WriteColored(message, ConsoleColor.Yellow);
    public static void Info(string message) => WriteColored(message, ConsoleColor.Cyan);

    public static void WriteColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    /// <summary>Aracı bakım durumuna göre renklendirerek yazdırır.</summary>
    public static void PrintVehicle(Vehicle vehicle)
    {
        MaintenanceStatus status = vehicle.CheckMaintenanceStatus();
        (string label, ConsoleColor color) = status switch
        {
            MaintenanceStatus.Due => ("BAKIM GELDİ", ConsoleColor.Red),
            MaintenanceStatus.Approaching => ("BAKIM YAKLAŞIYOR", ConsoleColor.Yellow),
            _ => ("İYİ", ConsoleColor.Green)
        };

        Console.WriteLine(vehicle);
        Console.Write("     Durum: ");
        WriteColored($"{label} | Tahmini bakım maliyeti: {vehicle.CalculateMaintenanceCost():N2} TL", color);
    }

    /// <summary>Boş olmayan bir metin okur.</summary>
    public static string ReadText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(input))
                return input.Replace('|', '/'); // Dosya ayırıcısıyla çakışmayı önle
            Warning("Bu alan boş bırakılamaz.");
        }
    }

    /// <summary>Tam sayı okur; hatalı girişte tekrar sorar.</summary>
    public static int ReadInt(string prompt, int min = 0, int max = int.MaxValue)
    {
        while (true)
        {
            try
            {
                Console.Write(prompt);
                int value = int.Parse(Console.ReadLine() ?? string.Empty);
                if (value < min || value > max)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Değer {min} ile {max} arasında olmalıdır.");
                return value;
            }
            catch (FormatException)
            {
                Error("Lütfen geçerli bir sayı giriniz (harf kullanmayın).");
            }
            catch (OverflowException)
            {
                Error("Girilen sayı çok büyük.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Error(ex.Message.Split('\n')[0]);
            }
        }
    }

    /// <summary>Negatif olmayan ondalık sayı okur; hatalı girişte tekrar sorar.</summary>
    public static double ReadNonNegativeDouble(string prompt)
    {
        while (true)
        {
            try
            {
                Console.Write(prompt);
                double value = double.Parse(Console.ReadLine() ?? string.Empty);
                if (value < 0 || double.IsNaN(value) || double.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(value), "Değer negatif olamaz.");
                return value;
            }
            catch (FormatException)
            {
                Error("Lütfen geçerli bir sayı giriniz (harf kullanmayın).");
            }
            catch (OverflowException)
            {
                Error("Girilen sayı çok büyük.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Error(ex.Message.Split('\n')[0]);
            }
        }
    }
}
