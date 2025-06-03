using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project_Task_Management.Models;

namespace Project_Task_Management.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
        public DbSet<ProjectAssignment> ProjectAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Project entity
            builder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Requirements).IsRequired();
                entity.Property(e => e.SkillsRequired).IsRequired();
                entity.Property(e => e.Criticality).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ModifiedAt);
            });

            // Configure EmployeeProfile entity
            builder.Entity<EmployeeProfile>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Skills).IsRequired();
                entity.Property(e => e.CurrentRole).IsRequired();
                entity.Property(e => e.Responsibilities).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ModifiedAt);

                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<EmployeeProfile>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ProjectAssignment entity
            builder.Entity<ProjectAssignment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProjectId).IsRequired();
                entity.Property(e => e.EmployeeId).IsRequired();
                entity.Property(e => e.AssignmentDate).IsRequired();
                entity.Property(e => e.RemovalDate);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ModifiedAt);

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.ProjectAssignments)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed roles
            var roles = new List<ApplicationRole>
            {
                new ApplicationRole
                {
                    Id = "1",
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    Description = "Project Manager role with full access",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new ApplicationRole
                {
                    Id = "2",
                    Name = "TeamLead",
                    NormalizedName = "TEAMLEAD",
                    Description = "Team Lead role with team management access",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new ApplicationRole
                {
                    Id = "3",
                    Name = "Employee",
                    NormalizedName = "EMPLOYEE",
                    Description = "Employee role with basic access",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            };

            builder.Entity<ApplicationRole>().HasData(roles);

            // Add indexes for better performance
            builder.Entity<Project>()
                .HasIndex(p => p.Title);

            builder.Entity<EmployeeProfile>()
                .HasIndex(e => e.UserId);

            builder.Entity<ProjectAssignment>()
                .HasIndex(pa => new { pa.ProjectId, pa.EmployeeId });
        }
    }
} 