namespace ABCPharmacy.Api.DTOs;

public sealed class CreateSaleRequest
{
    public int MedicineId { get; set; }
    public int Quantity { get; set; }
    public string SoldBy { get; set; } = "admin";
}
