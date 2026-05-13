using Microsoft.EntityFrameworkCore;
using RealEstate.Models;
using RealEstate.Models.Seller;
using System.Linq;

namespace RealEstate
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        // --- ALL TABLES ---
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<PropertySeller> Sellers { get; set; }
        public DbSet<PropertyInquiry> Inquiries { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ViewingAppointment> ViewingAppointments { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Commission> Commissions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Otp> Otps { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<CommissionDeal> CommissionDeals { get; set; }
        public DbSet<CommissionBatch> CommissionBatches { get; set; }
        public DbSet<CommissionRule> CommissionRules { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }
        public DbSet<TrackingSession> TrackingSessions { get; set; }
        public DbSet<InspectionItem> InspectionItems { get; set; }
        public DbSet<SaleTransaction> SaleTransactions { get; set; }

        // Customer Support & Ticketing
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; }

        // Marketing Campaign & Automation
        public DbSet<MarketingCampaign> MarketingCampaigns { get; set; }
        public DbSet<CampaignRecipient> CampaignRecipients { get; set; }
        public DbSet<CampaignActivity> CampaignActivities { get; set; }
        public DbSet<CustomerSegment> CustomerSegments { get; set; }
        public DbSet<SegmentMembership> SegmentMemberships { get; set; }

        // Sales Pipeline & Opportunity Tracking
        public DbSet<SalesOpportunity> SalesOpportunities { get; set; }
        public DbSet<OpportunityActivity> OpportunityActivities { get; set; }
        public DbSet<OpportunityNote> OpportunityNotes { get; set; }
        public DbSet<SalesPipeline> SalesPipelines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Global Decimal Precision Fix
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            modelBuilder.Entity<ApplicationUser>(b => { b.ToTable("Users"); });

            // 2. Appointment Fix (Resolves Cascade Cycle)
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasOne(a => a.Schedule).WithMany().HasForeignKey(a => a.ScheduleId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.Agent).WithMany().HasForeignKey(a => a.AgentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.Property).WithMany().HasForeignKey(a => a.PropertyId).OnDelete(DeleteBehavior.Cascade);
            });

            // 3. Transaction & Property Fix (Shadow State Cleanup)
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(t => t.Employee).WithMany().HasForeignKey(t => t.EmployeeId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(t => t.Property).WithMany().HasForeignKey(t => t.PropertyId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(t => t.Seller).WithMany().HasForeignKey(t => t.SellerId).OnDelete(DeleteBehavior.Restrict);
            });

            // 4. Invoice Fix (Shadow State Cleanup)
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasOne(d => d.Transaction).WithMany().HasForeignKey(d => d.TransactionId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.Employee).WithMany().HasForeignKey(d => d.EmployeeId).OnDelete(DeleteBehavior.Restrict);
            });

            // 5. Viewing Appointment Fix
            modelBuilder.Entity<ViewingAppointment>(entity =>
            {
                entity.HasOne(v => v.Appointment).WithMany().HasForeignKey(v => v.AppointmentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(v => v.Property).WithMany().HasForeignKey(v => v.PropertyId).OnDelete(DeleteBehavior.Cascade);
            });

            // 6. Tracking Sessions Fix (The one that caused the last crash)
            modelBuilder.Entity<TrackingSession>(entity =>
            {
                entity.HasOne(t => t.ViewingAppointment).WithMany().HasForeignKey(t => t.ViewingAppointmentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(t => t.Property).WithMany().HasForeignKey(t => t.PropertyId).OnDelete(DeleteBehavior.Restrict);
            });

            // 7. General Relationship Fixes
            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasOne(d => d.Seller).WithMany(p => p.Properties).HasForeignKey(d => d.SellerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.Employee).WithMany(p => p.Properties).HasForeignKey(d => d.EmployeeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Agent>(entity =>
            {
                entity.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.Agent).WithMany().HasForeignKey(n => n.AgentId).OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data configurations
            var managerId = 1;
            var brokerId = 2;
            var sellerId = 3;
            var accountingId = 4;
            var superAdminId = 5;

            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser { UserId = superAdminId, FullName = "Super Administrator", Username = "superadmin", Email = "superadmin@estateflow.com", PasswordHash = "GoFeFN+z3PYs0mOONJNnWm4xjS10y4y5a6g8zOVeJOE=", Role = "SuperAdmin", Status = "Active", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ApplicationUser { UserId = managerId, FullName = "System Manager", Username = "manager", Email = "manager@estateflow.com", PasswordHash = "d6g+mYT/D9IwRlBAn9o0xIWY8AK1wLQ+P18CTc2M1p0=", Role = "Manager", Status = "Active", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ApplicationUser { UserId = brokerId, FullName = "System Broker", Username = "broker", Email = "broker@estateflow.com", PasswordHash = "Ygh57lrWRjQGOjFH/r0aL8h6UcM6O1s6SXGLpts4uMg=", Role = "Broker", Status = "Active", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ApplicationUser { UserId = sellerId, FullName = "System Seller", Username = "seller", Email = "seller@estateflow.com", PasswordHash = "5Dq11+6d4/mXus0hHGvIWoXAjpuy5XyMT/OaYbpxLrY=", Role = "Seller", Status = "Active", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ApplicationUser { UserId = accountingId, FullName = "System Accounting", Username = "accounting", Email = "accounting@estateflow.com", PasswordHash = "6t/z2aTP6DXwyy/qjSvx1mWgkq1TaOrPMb3ghkNVQ3k=", Role = "Accounting", Status = "Active", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<CommissionRule>().HasData(
                new CommissionRule { RuleId = 1, ManagerId = managerId, CommissionPercent = 3.0m, CompanySharePercent = 2.0m, IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<AuditLog>().HasData(
                new AuditLog { LogId = 1, UserId = managerId, UserRole = "System", Action = "System Initialization", EntityType = "System", Description = "Comprehensive database schema initialized", CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc), IPAddress = "127.0.0.1", UserAgent = "System" }
            );
        }
    }
}