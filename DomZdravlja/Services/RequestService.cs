using DomZdravlja.Data;
using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Services;

public class RequestService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public RequestService(IDbContextFactory<AppDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<List<MedicationRequest>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Requests.AsNoTracking().OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task<List<MedicationRequest>> GetByUserIdAsync(int userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Requests.AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<MedicationRequest>> GetPendingAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Requests.AsNoTracking()
            .Where(r => r.Status == RequestStatus.NaCekanju)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task CreateAsync(MedicationRequest request)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        request.Status = RequestStatus.NaCekanju;
        request.CreatedAt = DateTime.Now;
        context.Requests.Add(request);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ApproveAsync(int requestId, int moderatorId, string note = "") =>
        await UpdateStatusAsync(requestId, RequestStatus.Odobren, moderatorId, note);

    public async Task<bool> RejectAsync(int requestId, int moderatorId, string note) =>
        await UpdateStatusAsync(requestId, RequestStatus.Odbijen, moderatorId, note);

    public async Task<string?> DeliverAsync(int requestId, int moderatorId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var request = await context.Requests.FindAsync(requestId);
        if (request is null || request.Status != RequestStatus.Odobren)
            return "Zahtjev nije pronađen ili nije odobren.";

        var medicine = await context.Medicines.FindAsync(request.MedicineId);
        if (medicine is null)
            return "Lijek nije pronađen.";

        if (medicine.ExpiryDate.HasValue && medicine.ExpiryDate.Value.Date < DateTime.Today)
            return "Lijek je istekao i ne može se izdati.";

        if (medicine.Quantity < request.Quantity)
            return "Nema dovoljno lijeka na zalihi.";

        medicine.Quantity -= request.Quantity;
        request.Status = RequestStatus.Izdato;
        request.ModeratorId = moderatorId;
        request.ProcessedAt = DateTime.Now;
        await context.SaveChangesAsync();
        return null;
    }

    private async Task<bool> UpdateStatusAsync(int requestId, RequestStatus status, int moderatorId, string note)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var request = await context.Requests.FindAsync(requestId);
        if (request is null || request.Status != RequestStatus.NaCekanju)
            return false;

        request.Status = status;
        request.ModeratorId = moderatorId;
        request.ProcessedAt = DateTime.Now;
        request.Note = note;
        await context.SaveChangesAsync();
        return true;
    }
}
