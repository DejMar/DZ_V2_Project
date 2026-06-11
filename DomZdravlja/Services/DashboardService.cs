using DomZdravlja.Models;

namespace DomZdravlja.Services;

public class DashboardService
{
    private const int OldPendingDays = 2;

    private readonly MedicineService _medicineService;
    private readonly RequestService _requestService;
    private readonly AmbulanceService _ambulanceService;
    private readonly StockIntakeService _stockIntakeService;
    private readonly UserService _userService;

    public DashboardService(
        MedicineService medicineService,
        RequestService requestService,
        AmbulanceService ambulanceService,
        StockIntakeService stockIntakeService,
        UserService userService)
    {
        _medicineService = medicineService;
        _requestService = requestService;
        _ambulanceService = ambulanceService;
        _stockIntakeService = stockIntakeService;
        _userService = userService;
    }

    public async Task<DashboardSummary> GetAdminDashboardAsync()
    {
        var medicines = await _medicineService.GetAllAsync();
        var requests = await _requestService.GetAllAsync();
        var pending = requests.Where(r => r.Status == RequestStatus.NaCekanju).OrderBy(r => r.CreatedAt).ToList();
        var medicineNames = medicines.ToDictionary(m => m.Id, m => m.Name);
        var ambulanceNames = (await _ambulanceService.GetAllAsync()).ToDictionary(a => a.Id, a => a.Name);
        var users = await _userService.GetAllAsync();
        var userNames = users.ToDictionary(u => u.Id, u => u.FullName);

        var summary = new DashboardSummary
        {
            Stats = new DashboardStats
            {
                PendingRequests = pending.Count,
                LowStockCount = medicines.Count(m => m.IsLowStock),
                ExpiredCount = medicines.Count(m => m.IsExpired),
                ExpiringSoonCount = medicines.Count(m => m.IsExpiringSoon)
            },
            CriticalMedicines = medicines
                .Where(m => m.IsExpired || m.IsExpiringSoon || m.IsLowStock)
                .OrderBy(m => m.IsExpired ? 0 : m.IsExpiringSoon ? 1 : 2)
                .ThenBy(m => m.Name)
                .Select(m => new DashboardMedicineItem
                {
                    Name = m.Name,
                    Quantity = m.Quantity,
                    Unit = m.Unit,
                    ExpiryDate = m.ExpiryDate,
                    Status = m.IsExpired ? "Istekao" : m.IsExpiringSoon ? "Ističe uskoro" : "Niska zaliha"
                })
                .ToList(),
            PendingRequests = MapRequests(pending, medicineNames, ambulanceNames),
            RecentIntakes = await MapRecentIntakesAsync(medicineNames, userNames)
        };

        if (summary.Stats.ExpiredCount > 0)
            summary.Alerts.Add(new DashboardAlert
            {
                Message = $"{summary.Stats.ExpiredCount} lijek(ova) je isteklo — ne mogu se izdavati.",
                Severity = "danger",
                Link = "/admin/lijekovi",
                LinkText = "Pregled lijekova"
            });

        if (summary.Stats.ExpiringSoonCount > 0)
            summary.Alerts.Add(new DashboardAlert
            {
                Message = $"{summary.Stats.ExpiringSoonCount} lijek(ova) ističe u narednih 30 dana.",
                Severity = "warning",
                Link = "/admin/lijekovi",
                LinkText = "Pregled lijekova"
            });

        if (summary.Stats.LowStockCount > 0)
            summary.Alerts.Add(new DashboardAlert
            {
                Message = $"{summary.Stats.LowStockCount} lijek(ova) ima nisku zalihu.",
                Severity = "warning",
                Link = "/admin/izvjestaji",
                LinkText = "Izvještaji"
            });

        if (summary.Stats.PendingRequests > 0)
            summary.Alerts.Add(new DashboardAlert
            {
                Message = $"{summary.Stats.PendingRequests} zahtjev(a) čeka obradu moderatora.",
                Severity = "info",
                Link = "/moderator/zahtjevi",
                LinkText = "Zahtjevi"
            });

        return summary;
    }

    public async Task<DashboardSummary> GetModeratorDashboardAsync()
    {
        var requests = await _requestService.GetAllAsync();
        var medicines = await _medicineService.GetAllAsync();
        var medicineNames = medicines.ToDictionary(m => m.Id, m => m.Name);
        var ambulanceNames = (await _ambulanceService.GetAllAsync()).ToDictionary(a => a.Id, a => a.Name);

        var pending = requests.Where(r => r.Status == RequestStatus.NaCekanju).OrderBy(r => r.CreatedAt).ToList();
        var oldPending = pending.Where(r => (DateTime.Now - r.CreatedAt).TotalDays >= OldPendingDays).ToList();
        var approved = requests.Where(r => r.Status == RequestStatus.Odobren).OrderBy(r => r.ProcessedAt).ToList();

        var summary = new DashboardSummary
        {
            Stats = new DashboardStats
            {
                PendingRequests = pending.Count,
                OldPendingRequests = oldPending.Count,
                ApprovedAwaitingDelivery = approved.Count
            },
            PendingRequests = MapRequests(pending, medicineNames, ambulanceNames)
        };

        if (summary.Stats.PendingRequests > 0)
            summary.Alerts.Add(new DashboardAlert
            {
                Message = $"{summary.Stats.PendingRequests} zahtjev(a) na čekanju za obradu.",
                Severity = "info",
                Link = "/moderator/zahtjevi",
                LinkText = "Otvori zahtjeve"
            });

        if (summary.Stats.OldPendingRequests > 0)
            summary.Alerts.Add(new DashboardAlert
            {
                Message = $"{summary.Stats.OldPendingRequests} zahtjev(a) čeka duže od {OldPendingDays} dana.",
                Severity = "warning",
                Link = "/moderator/zahtjevi",
                LinkText = "Pregledaj hitno"
            });

        if (summary.Stats.ApprovedAwaitingDelivery > 0)
            summary.Alerts.Add(new DashboardAlert
            {
                Message = $"{summary.Stats.ApprovedAwaitingDelivery} odobrenih zahtjeva čeka izdavanje.",
                Severity = "warning",
                Link = "/moderator/zahtjevi",
                LinkText = "Izdaj lijekove"
            });

        return summary;
    }

    public async Task<DashboardSummary> GetUserDashboardAsync(int userId)
    {
        var requests = await _requestService.GetByUserIdAsync(userId);
        var medicines = await _medicineService.GetAllAsync();
        var medicineNames = medicines.ToDictionary(m => m.Id, m => m.Name);

        var summary = new DashboardSummary
        {
            Stats = new DashboardStats
            {
                UserPendingCount = requests.Count(r => r.Status == RequestStatus.NaCekanju),
                UserApprovedCount = requests.Count(r => r.Status == RequestStatus.Odobren),
                UserDeliveredCount = requests.Count(r => r.Status == RequestStatus.Izdato),
                UserRejectedCount = requests.Count(r => r.Status == RequestStatus.Odbijen)
            },
            RecentUserRequests = requests.Take(5).Select(r => new DashboardRequestItem
            {
                Id = r.Id,
                MedicineName = medicineNames.GetValueOrDefault(r.MedicineId, "Nepoznat"),
                Quantity = r.Quantity,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                DaysWaiting = (int)(DateTime.Now - r.CreatedAt).TotalDays
            }).ToList()
        };

        if (summary.Stats.UserPendingCount > 0)
            summary.Alerts.Add(new DashboardAlert
            {
                Message = $"{summary.Stats.UserPendingCount} zahtjev(a) još čeka odgovor.",
                Severity = "info",
                Link = "/korisnik/moji-zahtjevi",
                LinkText = "Moji zahtjevi"
            });

        if (summary.Stats.UserApprovedCount > 0)
            summary.Alerts.Add(new DashboardAlert
            {
                Message = $"{summary.Stats.UserApprovedCount} zahtjev(a) je odobreno i čeka izdavanje.",
                Severity = "success",
                Link = "/korisnik/moji-zahtjevi",
                LinkText = "Pregled statusa"
            });

        var recentRejected = requests.FirstOrDefault(r => r.Status == RequestStatus.Odbijen);
        if (recentRejected is not null)
            summary.Alerts.Add(new DashboardAlert
            {
                Message = $"Posljednji odbijeni zahtjev: {medicineNames.GetValueOrDefault(recentRejected.MedicineId, "Nepoznat")} ({recentRejected.CreatedAt:dd.MM.yyyy}).",
                Severity = "danger",
                Link = "/korisnik/moji-zahtjevi",
                LinkText = "Detalji"
            });

        return summary;
    }

    private static List<DashboardRequestItem> MapRequests(
        List<MedicationRequest> requests,
        Dictionary<int, string> medicineNames,
        Dictionary<int, string> ambulanceNames) =>
        requests.Select(r => new DashboardRequestItem
        {
            Id = r.Id,
            MedicineName = medicineNames.GetValueOrDefault(r.MedicineId, "Nepoznat"),
            AmbulanceName = ambulanceNames.GetValueOrDefault(r.AmbulanceId, "Nepoznata"),
            Quantity = r.Quantity,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            DaysWaiting = (int)(DateTime.Now - r.CreatedAt).TotalDays
        }).ToList();

    private async Task<List<DashboardIntakeItem>> MapRecentIntakesAsync(
        Dictionary<int, string> medicineNames,
        Dictionary<int, string> userNames)
    {
        var medicines = await _medicineService.GetAllAsync();
        var units = medicines.ToDictionary(m => m.Id, m => m.Unit);
        var intakes = await _stockIntakeService.GetRecentAsync(5);

        return intakes.Select(i => new DashboardIntakeItem
        {
            ReceivedAt = i.ReceivedAt,
            MedicineName = medicineNames.GetValueOrDefault(i.MedicineId, "Nepoznat"),
            Quantity = i.Quantity,
            Unit = units.GetValueOrDefault(i.MedicineId, "kom"),
            ReceivedByName = userNames.GetValueOrDefault(i.ReceivedByUserId, "Nepoznat"),
            Note = i.Note
        }).ToList();
    }
}
