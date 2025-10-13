
using Microsoft.EntityFrameworkCore;

using Fadebook.Models;

namespace Fadebook.DB;

public class FadebookDbContext : DbContext
{
    public DbSet<CustomerModel> customerTable { get; set; }
    public DbSet<BarberModel> barberTable { get; set; }
    public DbSet<ServiceModel> serviceTable { get; set; }
    public DbSet<BarberServiceModel> barberServiceTable { get; set; }
    public DbSet<AppointmentModel> appointmentTable { get; set; }
    public DbSet<UserModel> userTable { get; set; }
    public DbSet<BarberWorkHoursModel> barberWorkHoursTable { get; set; }

    public FadebookDbContext(DbContextOptions<FadebookDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerModel>()
            .HasIndex(cm => cm.Username)
            .IsUnique();

        modelBuilder.Entity<BarberModel>()
            .HasIndex(bm => bm.Username)
            .IsUnique();

        modelBuilder.Entity<AppointmentModel>()
            .HasOne(am => am.Customer)
            .WithMany()
            .HasForeignKey(am => am.CustomerId)
            .IsRequired();

        modelBuilder.Entity<AppointmentModel>()
            .HasOne(am => am.Service)
            .WithMany()
            .HasForeignKey(am => am.ServiceId)
            .IsRequired();

        modelBuilder.Entity<AppointmentModel>()
            .HasOne(am => am.Barber)
            .WithMany()
            .HasForeignKey(am => am.BarberId)
            .IsRequired();

        // Configure explicit many-to-many relationship between Barber and Service
        modelBuilder.Entity<BarberServiceModel>()
            .HasKey(bsm => bsm.Id);

        modelBuilder.Entity<BarberServiceModel>()
            .HasIndex(bsm => new { bsm.BarberId, bsm.ServiceId })
            .IsUnique();

        modelBuilder.Entity<BarberServiceModel>()
            .HasOne(bsm => bsm.Barber)
            .WithMany(b => b.BarberServices)
            .HasForeignKey(bsm => bsm.BarberId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<BarberServiceModel>()
            .HasOne(bsm => bsm.Service)
            .WithMany(s => s.BarberServices)
            .HasForeignKey(bsm => bsm.ServiceId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // User model configuration
        modelBuilder.Entity<UserModel>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<UserModel>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // BarberWorkHours configuration
        modelBuilder.Entity<BarberWorkHoursModel>()
            .HasOne(bwh => bwh.Barber)
            .WithMany()
            .HasForeignKey(bwh => bwh.BarberId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Ensure unique work hour slots per barber per day (prevent duplicate entries)
        modelBuilder.Entity<BarberWorkHoursModel>()
            .HasIndex(bwh => new { bwh.BarberId, bwh.DayOfWeek, bwh.StartTime, bwh.EndTime })
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
