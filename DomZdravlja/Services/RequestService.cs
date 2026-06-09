using DomZdravlja.Models;

namespace DomZdravlja.Services;

public class RequestService
{
    private readonly JsonFileRepository<MedicationRequest> _repository;
    private readonly MedicineService _medicineService;

    public RequestService(
        JsonFileRepository<MedicationRequest> repository,
        MedicineService medicineService)
    {
        _repository = repository;
        _medicineService = medicineService;
    }

    public Task<List<MedicationRequest>> GetAllAsync() => _repository.GetAllAsync();

    public async Task<List<MedicationRequest>> GetByUserIdAsync(int userId) =>
        (await _repository.GetAllAsync()).Where(r => r.UserId == userId).OrderByDescending(r => r.CreatedAt).ToList();

    public async Task<List<MedicationRequest>> GetPendingAsync() =>
        (await _repository.GetAllAsync()).Where(r => r.Status == RequestStatus.NaCekanju).OrderBy(r => r.CreatedAt).ToList();

    public async Task CreateAsync(MedicationRequest request)
    {
        var requests = await _repository.GetAllAsync();
        request.Id = requests.Count == 0 ? 1 : requests.Max(r => r.Id) + 1;
        request.Status = RequestStatus.NaCekanju;
        request.CreatedAt = DateTime.Now;
        requests.Add(request);
        await _repository.SaveAllAsync(requests);
    }

    public async Task<bool> ApproveAsync(int requestId, int moderatorId, string note = "")
    {
        return await UpdateStatusAsync(requestId, RequestStatus.Odobren, moderatorId, note);
    }

    public async Task<bool> RejectAsync(int requestId, int moderatorId, string note)
    {
        return await UpdateStatusAsync(requestId, RequestStatus.Odbijen, moderatorId, note);
    }

    public async Task<bool> DeliverAsync(int requestId, int moderatorId)
    {
        var requests = await _repository.GetAllAsync();
        var request = requests.FirstOrDefault(r => r.Id == requestId);
        if (request is null || request.Status != RequestStatus.Odobren)
            return false;

        if (!await _medicineService.ReduceStockAsync(request.MedicineId, request.Quantity))
            return false;

        request.Status = RequestStatus.Izdato;
        request.ModeratorId = moderatorId;
        request.ProcessedAt = DateTime.Now;
        await _repository.SaveAllAsync(requests);
        return true;
    }

    private async Task<bool> UpdateStatusAsync(int requestId, RequestStatus status, int moderatorId, string note)
    {
        var requests = await _repository.GetAllAsync();
        var request = requests.FirstOrDefault(r => r.Id == requestId);
        if (request is null || request.Status != RequestStatus.NaCekanju)
            return false;

        request.Status = status;
        request.ModeratorId = moderatorId;
        request.ProcessedAt = DateTime.Now;
        request.Note = note;
        await _repository.SaveAllAsync(requests);
        return true;
    }
}
