namespace DomZdravlja.Models;

public class ReportSummary
{
    public int TotalMedicines { get; set; }
    public int LowStockCount { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int DeliveredRequests { get; set; }
    public int RejectedRequests { get; set; }
    public List<MedicineStockReport> MedicineStock { get; set; } = [];
    public List<AmbulanceRequestReport> RequestsByAmbulance { get; set; } = [];
}

public class MedicineStockReport
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int MinimumStock { get; set; }
    public bool IsLowStock { get; set; }
}

public class AmbulanceRequestReport
{
    public string AmbulanceName { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public int DeliveredCount { get; set; }
}
