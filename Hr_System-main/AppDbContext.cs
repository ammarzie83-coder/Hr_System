using Hr_System.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace Hr_System.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<UserAccount> UserAccounts { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<EmployeeAttachment> EmployeeAttachments { get; set; } = null!;
        public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.DisplayName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Role).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.NationalId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.JobTitle).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Mobile).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(120);
                entity.Property(e => e.PhotoContentType).HasMaxLength(100);
                entity.Property(e => e.PhotoData).HasColumnType("varbinary(max)");
            });

            modelBuilder.Entity<EmployeeAttachment>(entity =>
            {
                entity.Property(a => a.FileName).IsRequired().HasMaxLength(200);
                entity.Property(a => a.ContentType).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Data).IsRequired().HasColumnType("varbinary(max)");
                entity.HasOne(a => a.Employee).WithMany(e => e.Attachments).HasForeignKey(a => a.EmployeeId);
            });

            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.Property(l => l.LeaveType).IsRequired().HasMaxLength(80);
                entity.Property(l => l.Reason).IsRequired().HasMaxLength(300);
                entity.Property(l => l.Status).IsRequired().HasMaxLength(40);
                entity.HasOne(l => l.Employee).WithMany(e => e.LeaveRequests).HasForeignKey(l => l.EmployeeId);
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(a => a.TableName).IsRequired().HasMaxLength(128);
                entity.Property(a => a.Action).IsRequired().HasMaxLength(20);
                entity.Property(a => a.KeyValues).IsRequired();
                entity.Property(a => a.OldValues).HasColumnType("nvarchar(max)");
                entity.Property(a => a.NewValues).HasColumnType("nvarchar(max)");
                entity.Property(a => a.ChangedBy).IsRequired().HasMaxLength(100);
                entity.Property(a => a.ChangedAt).IsRequired();
            });
        }

        public override int SaveChanges()
        {
            AddAuditLogs();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddAuditLogs();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddAuditLogs()
        {
            ChangeTracker.DetectChanges();

            var auditEntries = new List<AuditLog>();
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is not AuditLog && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)))
            {
                var audit = new AuditLog
                {
                    TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    KeyValues = JsonSerializer.Serialize(entry.Properties
                        .Where(p => p.Metadata.IsPrimaryKey())
                        .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue)),
                    ChangedBy = userName,
                    ChangedAt = DateTime.UtcNow
                };

                if (entry.State == EntityState.Added)
                {
                    audit.NewValues = JsonSerializer.Serialize(entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));
                }
                else if (entry.State == EntityState.Modified)
                {
                    audit.OldValues = JsonSerializer.Serialize(entry.Properties
                        .Where(p => p.IsModified)
                        .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue));
                    audit.NewValues = JsonSerializer.Serialize(entry.Properties
                        .Where(p => p.IsModified)
                        .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));
                }
                else if (entry.State == EntityState.Deleted)
                {
                    audit.OldValues = JsonSerializer.Serialize(entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue));
                }

                auditEntries.Add(audit);
            }

            if (auditEntries.Any())
            {
                AuditLogs.AddRange(auditEntries);
            }
        }
    }
}