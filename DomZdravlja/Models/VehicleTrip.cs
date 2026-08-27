namespace DomZdravlja.Models;

public class VehicleTrip
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public int DriverId { get; set; }
    public int StartMileage { get; set; }
    public int? EndMileage { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.Now;
    public DateTime? EndedAt { get; set; }
    public string RouteDescription { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;

    public bool IsOpen => EndMileage is null;
    public int? DistanceKm => EndMileage.HasValue ? EndMileage.Value - StartMileage : null;
}
