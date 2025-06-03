using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Task_Management.Models
{
    public class EmployeeProfile
    {
        [Key]
        [ForeignKey("User")]
        public required string UserId { get; set; }
        public required ApplicationUser User { get; set; }

        [Required]
        public required string Skills { get; set; }

        [Required]
        public int TotalYearsOfExperience { get; set; }

        [Required]
        public required string CurrentRole { get; set; }

        [Required]
        public required string Responsibilities { get; set; }

        [Required]
        public WorkloadStatus WorkloadStatus { get; set; }

        public int NumberOfActiveProjects { get; set; }

        public int NumberOfTasks { get; set; }

        public int EstimatedHours { get; set; }

        public DateTime? NextProjectDeadline { get; set; }

        // Navigation properties
        public ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
    }

    public enum WorkloadStatus
    {
        Available,    // 0-1 projects
        Moderate,     // 2 projects
        High          // 3+ projects
    }
} 