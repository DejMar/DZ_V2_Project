namespace DomZdravlja.Models;

public class Medicine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Unit { get; set; } = "kom";
    public int MinimumStock { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public bool IsLowStock => Quantity <= MinimumStock;
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value.Date < DateTime.Today;
    public bool IsExpiringSoon => ExpiryDate.HasValue && !IsExpired && ExpiryDate.Value.Date <= DateTime.Today.AddDays(30);
}
