using DomZdravlja.Models;

namespace DomZdravlja.Services;

public class StockIntakeService
{
    private readonly JsonFileRepository<StockIntake> _repository;
    private readonly MedicineService _medicineService;

    public StockIntakeService(
        JsonFileRepository<StockIntake> repository,
        MedicineService medicineService)
    {
        _repository = repository;
        _medicineService = medicineService;
    }

    public async Task<List<StockIntake>> GetAllAsync()
    {
        var intakes = await _repository.GetAllAsync();
        return intakes.OrderByDescending(i => i.ReceivedAt).ToList();
    }

    public async Task<List<StockIntake>> GetRecentAsync(int count = 10)
    {
        var intakes = await GetAllAsync();
        return intakes.Take(count).ToList();
    }

    public async Task<string?> RecordIntakeAsync(StockIntake intake)
    {
        if (intake.Quantity <= 0)
            return "Količina mora biti veća od nule.";

        var medicine = await _medicineService.GetByIdAsync(intake.MedicineId);
        if (medicine is null)
            return "Lijek nije pronađen.";

        var intakes = await _repository.GetAllAsync();
        intake.Id = intakes.Count == 0 ? 1 : intakes.Max(i => i.Id) + 1;
        intake.ReceivedAt = DateTime.Now;
        intakes.Add(intake);
        await _repository.SaveAllAsync(intakes);

        medicine.Quantity += intake.Quantity;
        if (intake.UpdatedExpiryDate.HasValue)
            medicine.ExpiryDate = intake.UpdatedExpiryDate.Value;

        await _medicineService.UpdateAsync(medicine);
        return null;
    }
}
