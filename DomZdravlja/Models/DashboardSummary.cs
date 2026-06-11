namespace DomZdravlja.Models;

public class DashboardSummary
{
    public List<DashboardAlert> Alerts { get; set; } = [];
    public DashboardStats Stats { get; set; } = new();
    public List<DashboardMedicineItem> CriticalMedicines { get; set; } = [];
    public List<DashboardRequestItem> PendingRequests { get; set; } = [];
    public List<DashboardRequestItem> RecentUserRequests { get; set; } = [];
    public List<DashboardIntakeItem> RecentIntakes { get; set; } = [];
}

public class DashboardAlert
{
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = "info";
    public string? Link { get; set; }
    public string? LinkText { get; set; }
}

public class DashboardStats
{
    public int PendingRequests { get; set; }
    public int OldPendingRequests { get; set; }
    public int ApprovedAwaitingDelivery { get; set; }
    public int LowStockCount { get; set; }
    public int ExpiredCount { get; set; }
    public int ExpiringSoonCount { get; set; }
    public int UserPendingCount { get; set; }
    public int UserApprovedCount { get; set; }
    public int UserDeliveredCount { get; set; }
    public int UserRejectedCount { get; set; }
}

public class DashboardMedicineItem
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class DashboardRequestItem
{
    public int Id { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public string AmbulanceName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int DaysWaiting { get; set; }
}

public class DashboardIntakeItem
{
    public DateTime ReceivedAt { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string ReceivedByName { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}
