using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Task_Management.Models
{
    public class ProjectAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public required Project Project { get; set; }

        [Required]
        [ForeignKey("Employee")]
        public required string EmployeeId { get; set; }
        public required ApplicationUser Employee { get; set; }

        [Required]
        public ProjectRole Role { get; set; }

        [Required]
        public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;

        public DateTime? RemovalDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
    }

    public enum ProjectRole
    {
        Developer,
        Tester,
        Designer,
        BusinessAnalyst,
        ProjectLead
    }
} 