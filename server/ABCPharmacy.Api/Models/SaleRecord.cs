namespace ABCPharmacy.Api.Models;

public sealed class SaleRecord
{
    public int Id { get; set; }
    public int MedicineId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime SoldAtUtc { get; set; }
    public string SoldBy { get; set; } = string.Empty;
}
