using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<Ambulance> Ambulances => Set<Ambulance>();
    public DbSet<MedicationRequest> Requests => Set<MedicationRequest>();
    public DbSet<StockIntake> StockIntakes => Set<StockIntake>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleTrip> VehicleTrips => Set<VehicleTrip>();
    public DbSet<FuelFill> FuelFills => Set<FuelFill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).HasMaxLength(100).IsRequired();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Password).HasMaxLength(255).IsRequired();
            entity.Property(u => u.FullName).HasMaxLength(200).IsRequired();
            entity.Property(u => u.Role).HasConversion<int>();
            entity.HasOne<Ambulance>()
                .WithMany()
                .HasForeignKey(u => u.AmbulanceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.ToTable("medicines");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Name).HasMaxLength(200).IsRequired();
            entity.Property(m => m.Description).HasMaxLength(500).IsRequired();
            entity.Property(m => m.Unit).HasMaxLength(50).IsRequired();
            entity.Ignore(m => m.IsLowStock);
            entity.Ignore(m => m.IsExpired);
            entity.Ignore(m => m.IsExpiringSoon);
        });

        modelBuilder.Entity<Ambulance>(entity =>
        {
            entity.ToTable("ambulances");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<MedicationRequest>(entity =>
        {
            entity.ToTable("requests");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Status).HasConversion<int>();
            entity.Property(r => r.Note).HasMaxLength(500).IsRequired();
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Ambulance>()
                .WithMany()
                .HasForeignKey(r => r.AmbulanceId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Medicine>()
                .WithMany()
                .HasForeignKey(r => r.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.ModeratorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<StockIntake>(entity =>
        {
            entity.ToTable("stock_intakes");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Note).HasMaxLength(500).IsRequired();
            entity.HasOne<Medicine>()
                .WithMany()
                .HasForeignKey(s => s.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.ReceivedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("vehicles");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.PlateNumber).HasMaxLength(20).IsRequired();
            entity.HasIndex(v => v.PlateNumber).IsUnique();
            entity.Property(v => v.Brand).HasMaxLength(100).IsRequired();
            entity.Property(v => v.Model).HasMaxLength(100).IsRequired();
            entity.Property(v => v.FuelType).HasMaxLength(50).IsRequired();
            entity.Property(v => v.TankCapacityLiters).HasPrecision(8, 2);
            entity.Property(v => v.Note).HasMaxLength(500).IsRequired();
            entity.Ignore(v => v.DisplayName);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(v => v.AssignedDriverId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<VehicleTrip>(entity =>
        {
            entity.ToTable("vehicle_trips");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.RouteDescription).HasMaxLength(500).IsRequired();
            entity.Property(t => t.Note).HasMaxLength(500).IsRequired();
            entity.Ignore(t => t.IsOpen);
            entity.Ignore(t => t.DistanceKm);
            entity.HasOne<Vehicle>()
                .WithMany()
                .HasForeignKey(t => t.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FuelFill>(entity =>
        {
            entity.ToTable("fuel_fills");
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Liters).HasPrecision(8, 2);
            entity.Property(f => f.Cost).HasPrecision(10, 2);
            entity.Property(f => f.Station).HasMaxLength(200).IsRequired();
            entity.Property(f => f.Note).HasMaxLength(500).IsRequired();
            entity.HasOne<Vehicle>()
                .WithMany()
                .HasForeignKey(f => f.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(f => f.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
