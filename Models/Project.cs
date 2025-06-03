using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Task_Management.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public required string Requirements { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public ProjectStatus Status { get; set; }

        [Required]
        public ProjectPriority Priority { get; set; }

        [Required]
        public required string SkillsRequired { get; set; }

        [Required]
        public int NumberOfPeopleNeeded { get; set; }

        [Required]
        public required string Criticality { get; set; }

        // Navigation properties
        [ForeignKey("TeamLead")]
        public required string TeamLeadId { get; set; }
        public required ApplicationUser TeamLead { get; set; }

        public ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
    }

    public enum ProjectStatus
    {
        NotStarted,
        Active,
        OnHold,
        Completed,
        Cancelled
    }

    public enum ProjectPriority
    {
        High,
        Medium,
        Low
    }
} 