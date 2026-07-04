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
    }
}
