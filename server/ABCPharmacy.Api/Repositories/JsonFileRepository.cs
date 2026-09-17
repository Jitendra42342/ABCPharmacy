using System.Text.Json;
using ABCPharmacy.Api.Models;

namespace ABCPharmacy.Api.Repositories;

public sealed class JsonFileRepository
{
    private readonly string _dataDirectory;
    private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
    private readonly object _sync = new();

    public JsonFileRepository(IWebHostEnvironment environment)
    {
        _dataDirectory = Path.Combine(environment.ContentRootPath, "Data");
        Directory.CreateDirectory(_dataDirectory);
        EnsureFile("medicines.json", SeedMedicines());
        EnsureFile("sales.json", new List<SaleRecord>());
    }

    public List<Medicine> GetMedicines() =>
        Read<List<Medicine>>("medicines.json") ?? [];

    public void SaveMedicines(List<Medicine> medicines) =>
        Write("medicines.json", medicines);

    public List<SaleRecord> GetSales() =>
        Read<List<SaleRecord>>("sales.json") ?? [];

    public void SaveSales(List<SaleRecord> sales) =>
        Write("sales.json", sales);

    public T? Read<T>(string fileName)
    {
        lock (_sync)
        {
            var path = Path.Combine(_dataDirectory, fileName);
            if (!File.Exists(path)) return default;
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path), _options);
        }
    }

    private void Write<T>(string fileName, T value)
    {
        lock (_sync)
        {
            var path = Path.Combine(_dataDirectory, fileName);
            var temp = path + ".tmp";
            File.WriteAllText(temp, JsonSerializer.Serialize(value, _options));
            File.Move(temp, path, true);
        }
    }

    private void EnsureFile<T>(string fileName, T seed)
    {
        var path = Path.Combine(_dataDirectory, fileName);
        if (!File.Exists(path))
            Write(fileName, seed);
    }

    private static List<Medicine> SeedMedicines() =>
    [
        new() { Id = 1, FullName = "Paracetamol 500mg", Notes = "Pain and fever relief", ExpiryDate = DateTime.Today.AddDays(20), Quantity = 25, Price = 35.50m, Brand = "ABC Pharma" },
        new() { Id = 2, FullName = "Azithromycin 250mg", Notes = "Antibiotic - prescription required", ExpiryDate = DateTime.Today.AddMonths(6), Quantity = 7, Price = 120.00m, Brand = "HealthCare" },
        new() { Id = 3, FullName = "Vitamin C 500mg", Notes = "Supplement", ExpiryDate = DateTime.Today.AddYears(1), Quantity = 50, Price = 85.75m, Brand = "WellLife" }
    ];
}
