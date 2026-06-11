namespace DomZdravlja.Models;

public class StockIntake
{
    public int Id { get; set; }
    public int MedicineId { get; set; }
    public int Quantity { get; set; }
    public DateTime ReceivedAt { get; set; } = DateTime.Now;
    public int ReceivedByUserId { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime? UpdatedExpiryDate { get; set; }
}
