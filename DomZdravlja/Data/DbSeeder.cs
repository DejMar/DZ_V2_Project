using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Data;

public class DbSeeder
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public DbSeeder(IDbContextFactory<AppDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task SeedAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        if (!await context.Ambulances.AnyAsync())
        {
            await context.Ambulances.AddRangeAsync(
                new Ambulance { Name = "Opća ambulanta" },
                new Ambulance { Name = "Pedijatrijska ambulanta" },
                new Ambulance { Name = "Stomatološka ambulanta" }
            );
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync())
        {
            await context.Users.AddRangeAsync(
                new User { Username = "admin", Password = "admin123", Role = UserRole.Administrator, FullName = "Admin Korisnik", IsActive = true },
                new User { Username = "moderator", Password = "mod123", Role = UserRole.Moderator, FullName = "Moderator Korisnik", IsActive = true },
                new User { Username = "korisnik1", Password = "user123", Role = UserRole.Korisnik, AmbulanceId = 1, FullName = "Dr. Ana Marković", IsActive = true },
                new User { Username = "korisnik2", Password = "user123", Role = UserRole.Korisnik, AmbulanceId = 2, FullName = "Dr. Marko Jović", IsActive = true },
                new User { Username = "korisnik3", Password = "user123", Role = UserRole.Korisnik, AmbulanceId = 3, FullName = "Dr. Ivana Petrović", IsActive = true }
            );
            await context.SaveChangesAsync();
        }

        if (!await context.Medicines.AnyAsync())
        {
            await context.Medicines.AddRangeAsync(
                new Medicine { Name = "Paracetamol 500mg", Description = "Analgetik i antipiretik", Quantity = 500, Unit = "tableta", MinimumStock = 100, ExpiryDate = DateTime.Today.AddMonths(18) },
                new Medicine { Name = "Ibuprofen 400mg", Description = "Protivupalni lijek", Quantity = 300, Unit = "tableta", MinimumStock = 80, ExpiryDate = DateTime.Today.AddMonths(12) },
                new Medicine { Name = "Amoxicillin 500mg", Description = "Antibiotik", Quantity = 150, Unit = "kapsula", MinimumStock = 50, ExpiryDate = DateTime.Today.AddDays(20) },
                new Medicine { Name = "Hlorheksidin", Description = "Antiseptik", Quantity = 40, Unit = "flaša", MinimumStock = 20, ExpiryDate = DateTime.Today.AddMonths(-1) },
                new Medicine { Name = "Fizološki rastvor", Description = "Infuzija 0.9% NaCl", Quantity = 200, Unit = "vrecica", MinimumStock = 50, ExpiryDate = DateTime.Today.AddMonths(24) },
                new Medicine { Name = "Aspirin 100mg", Description = "Antiagregans, analgetik", Quantity = 250, Unit = "tableta", MinimumStock = 60, ExpiryDate = DateTime.Today.AddMonths(14) },
                new Medicine { Name = "Omeprazol 20mg", Description = "Inhibitor protonske pumpe", Quantity = 120, Unit = "kapsula", MinimumStock = 40, ExpiryDate = DateTime.Today.AddMonths(20) },
                new Medicine { Name = "Loperamid 2mg", Description = "Protiv dijareje", Quantity = 80, Unit = "tableta", MinimumStock = 30, ExpiryDate = DateTime.Today.AddMonths(10) },
                new Medicine { Name = "Cetirizin 10mg", Description = "Antihistaminik", Quantity = 100, Unit = "tableta", MinimumStock = 35, ExpiryDate = DateTime.Today.AddMonths(16) },
                new Medicine { Name = "Metformin 500mg", Description = "Antidijabetik", Quantity = 200, Unit = "tableta", MinimumStock = 50, ExpiryDate = DateTime.Today.AddMonths(22) },
                new Medicine { Name = "Salbutamol inhalator", Description = "Bronhodilatator", Quantity = 25, Unit = "inhalator", MinimumStock = 10, ExpiryDate = DateTime.Today.AddMonths(6) },
                new Medicine { Name = "Diazepam 5mg", Description = "Anksiolitik", Quantity = 45, Unit = "tableta", MinimumStock = 15, ExpiryDate = DateTime.Today.AddMonths(12) },
                new Medicine { Name = "Hidrokortizon krema 1%", Description = "Topikalni kortikosteroid", Quantity = 30, Unit = "tuba", MinimumStock = 10, ExpiryDate = DateTime.Today.AddMonths(8) },
                new Medicine { Name = "Rukavice lateks", Description = "Za jednokratnu upotrebu, vel. M", Quantity = 500, Unit = "par", MinimumStock = 100, ExpiryDate = DateTime.Today.AddMonths(30) },
                new Medicine { Name = "Vitamin D3 1000 IU", Description = "Dodatak vitamina D", Quantity = 180, Unit = "kapsula", MinimumStock = 40, ExpiryDate = DateTime.Today.AddMonths(18) }
            );
            await context.SaveChangesAsync();
        }
    }
}
