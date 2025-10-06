
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

        modelBuilder.Entity<BarberServiceModel>()
            .HasOne(bsm => bsm.Barber)
            .WithMany()
            .HasForeignKey(bsm => bsm.BarberId)
            .IsRequired();

        modelBuilder.Entity<BarberServiceModel>()
            .HasOne(bsm => bsm.Service)
            .WithMany()
            .HasForeignKey(bsm => bsm.ServiceId)
            .IsRequired();

        modelBuilder.Entity<BarberServiceModel>()
            .HasKey(bsm => new { bsm.BarberId, bsm.ServiceId });
        
        // TODO: Add BarberService composite key

        base.OnModelCreating(modelBuilder);
    }
}
