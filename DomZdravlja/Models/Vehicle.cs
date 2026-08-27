namespace DomZdravlja.Models;

public class Vehicle
{
    public int Id { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string FuelType { get; set; } = "Dizel";
    public decimal TankCapacityLiters { get; set; }
    public int CurrentMileage { get; set; }
    public int? AssignedDriverId { get; set; }
    public string Note { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public string DisplayName => $"{Brand} {Model} ({PlateNumber})";
}
