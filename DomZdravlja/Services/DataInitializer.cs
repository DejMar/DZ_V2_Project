using DomZdravlja.Models;

namespace DomZdravlja.Services;

public class DataInitializer
{
    private readonly JsonFileRepository<User> _users;
    private readonly JsonFileRepository<Medicine> _medicines;
    private readonly JsonFileRepository<Ambulance> _ambulances;
    private readonly JsonFileRepository<MedicationRequest> _requests;
    private readonly JsonFileRepository<StockIntake> _stockIntakes;

    public DataInitializer(
        JsonFileRepository<User> users,
        JsonFileRepository<Medicine> medicines,
        JsonFileRepository<Ambulance> ambulances,
        JsonFileRepository<MedicationRequest> requests,
        JsonFileRepository<StockIntake> stockIntakes)
    {
        _users = users;
        _medicines = medicines;
        _ambulances = ambulances;
        _requests = requests;
        _stockIntakes = stockIntakes;
    }

    public async Task InitializeAsync()
    {
        await MigrateLegacyUsersAsync();

        if ((await _users.GetAllAsync()).Count == 0)
        {
            await _users.SaveAllAsync(
            [
                new User { Id = 1, Username = "admin", Password = "admin123", Role = UserRole.Administrator, FullName = "Admin Korisnik", IsActive = true },
                new User { Id = 2, Username = "moderator", Password = "mod123", Role = UserRole.Moderator, FullName = "Moderator Korisnik", IsActive = true },
                new User { Id = 3, Username = "korisnik1", Password = "user123", Role = UserRole.Korisnik, AmbulanceId = 1, FullName = "Dr. Ana Marković", IsActive = true },
                new User { Id = 4, Username = "korisnik2", Password = "user123", Role = UserRole.Korisnik, AmbulanceId = 2, FullName = "Dr. Marko Jović", IsActive = true },
                new User { Id = 5, Username = "korisnik3", Password = "user123", Role = UserRole.Korisnik, AmbulanceId = 3, FullName = "Dr. Ivana Petrović", IsActive = true }
            ]);
        }

        if ((await _ambulances.GetAllAsync()).Count == 0)
        {
            await _ambulances.SaveAllAsync(
            [
                new Ambulance { Id = 1, Name = "Opća ambulanta" },
                new Ambulance { Id = 2, Name = "Pedijatrijska ambulanta" },
                new Ambulance { Id = 3, Name = "Stomatološka ambulanta" }
            ]);
        }

        if ((await _medicines.GetAllAsync()).Count == 0)
        {
            await _medicines.SaveAllAsync(
            [
                new Medicine { Id = 1, Name = "Paracetamol 500mg", Description = "Analgetik i antipiretik", Quantity = 500, Unit = "tableta", MinimumStock = 100, ExpiryDate = DateTime.Today.AddMonths(18) },
                new Medicine { Id = 2, Name = "Ibuprofen 400mg", Description = "Protivupalni lijek", Quantity = 300, Unit = "tableta", MinimumStock = 80, ExpiryDate = DateTime.Today.AddMonths(12) },
                new Medicine { Id = 3, Name = "Amoxicillin 500mg", Description = "Antibiotik", Quantity = 150, Unit = "kapsula", MinimumStock = 50, ExpiryDate = DateTime.Today.AddDays(20) },
                new Medicine { Id = 4, Name = "Hlorheksidin", Description = "Antiseptik", Quantity = 40, Unit = "flaša", MinimumStock = 20, ExpiryDate = DateTime.Today.AddMonths(-1) },
                new Medicine { Id = 5, Name = "Fizološki rastvor", Description = "Infuzija 0.9% NaCl", Quantity = 200, Unit = "vrecica", MinimumStock = 50, ExpiryDate = DateTime.Today.AddMonths(24) },
                new Medicine { Id = 6, Name = "Aspirin 100mg", Description = "Antiagregans, analgetik", Quantity = 250, Unit = "tableta", MinimumStock = 60, ExpiryDate = DateTime.Today.AddMonths(14) },
                new Medicine { Id = 7, Name = "Omeprazol 20mg", Description = "Inhibitor protonske pumpe", Quantity = 120, Unit = "kapsula", MinimumStock = 40, ExpiryDate = DateTime.Today.AddMonths(20) },
                new Medicine { Id = 8, Name = "Loperamid 2mg", Description = "Protiv dijareje", Quantity = 80, Unit = "tableta", MinimumStock = 30, ExpiryDate = DateTime.Today.AddMonths(10) },
                new Medicine { Id = 9, Name = "Cetirizin 10mg", Description = "Antihistaminik", Quantity = 100, Unit = "tableta", MinimumStock = 35, ExpiryDate = DateTime.Today.AddMonths(16) },
                new Medicine { Id = 10, Name = "Metformin 500mg", Description = "Antidijabetik", Quantity = 200, Unit = "tableta", MinimumStock = 50, ExpiryDate = DateTime.Today.AddMonths(22) },
                new Medicine { Id = 11, Name = "Salbutamol inhalator", Description = "Bronhodilatator", Quantity = 25, Unit = "inhalator", MinimumStock = 10, ExpiryDate = DateTime.Today.AddMonths(6) },
                new Medicine { Id = 12, Name = "Diazepam 5mg", Description = "Anksiolitik", Quantity = 45, Unit = "tableta", MinimumStock = 15, ExpiryDate = DateTime.Today.AddMonths(12) },
                new Medicine { Id = 13, Name = "Hidrokortizon krema 1%", Description = "Topikalni kortikosteroid", Quantity = 30, Unit = "tuba", MinimumStock = 10, ExpiryDate = DateTime.Today.AddMonths(8) },
                new Medicine { Id = 14, Name = "Rukavice lateks", Description = "Za jednokratnu upotrebu, vel. M", Quantity = 500, Unit = "par", MinimumStock = 100, ExpiryDate = DateTime.Today.AddMonths(30) },
                new Medicine { Id = 15, Name = "Vitamin D3 1000 IU", Description = "Dodatak vitamina D", Quantity = 180, Unit = "kapsula", MinimumStock = 40, ExpiryDate = DateTime.Today.AddMonths(18) }
            ]);
        }

        if ((await _requests.GetAllAsync()).Count == 0)
            await _requests.SaveAllAsync([]);

        if ((await _stockIntakes.GetAllAsync()).Count == 0)
            await _stockIntakes.SaveAllAsync([]);
    }

    private async Task MigrateLegacyUsersAsync()
    {
        var users = await _users.GetAllAsync();
        if (users.Count == 0)
            return;

        if (users.Any(u => u.IsActive))
            return;

        foreach (var user in users)
            user.IsActive = true;

        await _users.SaveAllAsync(users);
    }
}
