using ABCPharmacy.Api.DTOs;
using ABCPharmacy.Api.Models;
using ABCPharmacy.Api.Repositories;

namespace ABCPharmacy.Api.Services;

public sealed class SaleService(JsonFileRepository repository)
{
    public IReadOnlyList<SaleRecord> GetAll() =>
        repository.GetSales().OrderByDescending(x => x.SoldAtUtc).ToList();

    public SaleRecord Sell(CreateSaleRequest request)
    {
        if (request.Quantity <= 0)
            throw new ArgumentException("Sale quantity must be greater than zero.");

        var medicines = repository.GetMedicines();
        var medicine = medicines.FirstOrDefault(x => x.Id == request.MedicineId);

        if (medicine is null)
            throw new KeyNotFoundException("Medicine not found.");

        if (medicine.Quantity < request.Quantity)
            throw new InvalidOperationException($"Insufficient stock. Available quantity: {medicine.Quantity}.");

        medicine.Quantity -= request.Quantity;

        var sales = repository.GetSales();
        var sale = new SaleRecord
        {
            Id = sales.Count == 0 ? 1 : sales.Max(x => x.Id) + 1,
            MedicineId = medicine.Id,
            MedicineName = medicine.FullName,
            Quantity = request.Quantity,
            UnitPrice = medicine.Price,
            TotalAmount = decimal.Round(medicine.Price * request.Quantity, 2),
            SoldAtUtc = DateTime.UtcNow,
            SoldBy = string.IsNullOrWhiteSpace(request.SoldBy) ? "admin" : request.SoldBy.Trim()
        };

        sales.Add(sale);

        // Both files are updated under the repository lock.
        repository.SaveMedicines(medicines);
        repository.SaveSales(sales);

        return sale;
    }
}
