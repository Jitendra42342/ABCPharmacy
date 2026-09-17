using ABCPharmacy.Api.DTOs;
using ABCPharmacy.Api.Models;
using ABCPharmacy.Api.Repositories;

namespace ABCPharmacy.Api.Services;

public sealed class MedicineService(JsonFileRepository repository)
{
    public IReadOnlyList<Medicine> Get(string? search)
    {
        var medicines = repository.GetMedicines();

        if (!string.IsNullOrWhiteSpace(search))
            medicines = medicines
                .Where(x => x.FullName.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

        return medicines.OrderBy(x => x.FullName).ToList();
    }

    public Medicine? GetById(int id) =>
        repository.GetMedicines().FirstOrDefault(x => x.Id == id);

    public Medicine Add(CreateMedicineRequest request)
    {
        Validate(request);

        var medicines = repository.GetMedicines();
        var medicine = new Medicine
        {
            Id = medicines.Count == 0 ? 1 : medicines.Max(x => x.Id) + 1,
            FullName = request.FullName.Trim(),
            Notes = request.Notes?.Trim() ?? string.Empty,
            ExpiryDate = request.ExpiryDate.Date,
            Quantity = request.Quantity,
            Price = decimal.Round(request.Price, 2),
            Brand = request.Brand.Trim()
        };

        medicines.Add(medicine);
        repository.SaveMedicines(medicines);
        return medicine;
    }

    private static void Validate(CreateMedicineRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new ArgumentException("Medicine name is required.");
        if (string.IsNullOrWhiteSpace(request.Brand))
            throw new ArgumentException("Brand is required.");
        if (request.Quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.");
        if (request.Price < 0)
            throw new ArgumentException("Price cannot be negative.");
        if (request.ExpiryDate == default)
            throw new ArgumentException("Expiry date is required.");
    }
}
