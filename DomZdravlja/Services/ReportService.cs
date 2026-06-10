using DomZdravlja.Models;

namespace DomZdravlja.Services;

public class ReportService
{
    private readonly MedicineService _medicineService;
    private readonly RequestService _requestService;
    private readonly AmbulanceService _ambulanceService;

    public ReportService(
        MedicineService medicineService,
        RequestService requestService,
        AmbulanceService ambulanceService)
    {
        _medicineService = medicineService;
        _requestService = requestService;
        _ambulanceService = ambulanceService;
    }

    public async Task<ReportSummary> GetSummaryAsync()
    {
        var medicines = await _medicineService.GetAllAsync();
        var requests = await _requestService.GetAllAsync();
        var ambulances = await _ambulanceService.GetAllAsync();

        return new ReportSummary
        {
            TotalMedicines = medicines.Count,
            LowStockCount = medicines.Count(m => m.IsLowStock),
            ExpiredCount = medicines.Count(m => m.IsExpired),
            ExpiringSoonCount = medicines.Count(m => m.IsExpiringSoon),
            PendingRequests = requests.Count(r => r.Status == RequestStatus.NaCekanju),
            ApprovedRequests = requests.Count(r => r.Status == RequestStatus.Odobren),
            DeliveredRequests = requests.Count(r => r.Status == RequestStatus.Izdato),
            RejectedRequests = requests.Count(r => r.Status == RequestStatus.Odbijen),
            MedicineStock = medicines.Select(m => new MedicineStockReport
            {
                Name = m.Name,
                Quantity = m.Quantity,
                Unit = m.Unit,
                MinimumStock = m.MinimumStock,
                IsLowStock = m.IsLowStock,
                ExpiryDate = m.ExpiryDate,
                IsExpired = m.IsExpired,
                IsExpiringSoon = m.IsExpiringSoon
            }).OrderBy(m => m.Name).ToList(),
            RequestsByAmbulance = ambulances.Select(a => new AmbulanceRequestReport
            {
                AmbulanceName = a.Name,
                TotalRequests = requests.Count(r => r.AmbulanceId == a.Id),
                DeliveredCount = requests.Count(r => r.AmbulanceId == a.Id && r.Status == RequestStatus.Izdato)
            }).ToList()
        };
    }
}
