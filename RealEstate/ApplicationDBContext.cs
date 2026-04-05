using Microsoft.EntityFrameworkCore;
using RealEstate.Models;
using RealEstate.Services;

namespace RealEstate;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ViewingAppointment> ViewingAppointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Roles table
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.Property(e => e.RoleType)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.RoleType)
                .IsUnique();
        });

        // Configure Users table
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.PasswordHash)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Foreign Key relationship
            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.HasIndex(e => e.RoleId);
            entity.HasIndex(e => e.IsActive);
        });

        // Configure ViewingAppointment table
        modelBuilder.Entity<ViewingAppointment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.PropertyId)
                .IsRequired();

            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CustomerPhone)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.CustomerPhotoUrl)
                .HasMaxLength(500);

            entity.Property(e => e.WhenUtc)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(AppointmentStatus.Scheduled);

            entity.HasIndex(e => e.PropertyId);
            entity.HasIndex(e => e.WhenUtc);
        });
    }
}
