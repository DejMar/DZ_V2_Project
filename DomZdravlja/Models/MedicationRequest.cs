namespace DomZdravlja.Models;

public class MedicationRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AmbulanceId { get; set; }
    public int MedicineId { get; set; }
    public int Quantity { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.NaCekanju;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ProcessedAt { get; set; }
    public int? ModeratorId { get; set; }
    public string Note { get; set; } = string.Empty;
}
