namespace DomZdravlja.Models;

public class FuelFill
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public int DriverId { get; set; }
    public int Mileage { get; set; }
    public decimal Liters { get; set; }
    public decimal? Cost { get; set; }
    public bool IsFullTank { get; set; }
    public DateTime FilledAt { get; set; } = DateTime.Now;
    public string Station { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}
