using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Task_Management.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public TaskStatus Status { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public required string Priority { get; set; } // High/Medium/Low

        [Required]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public required Project Project { get; set; }

        [Required]
        [ForeignKey("AssignedTo")]
        public required string EmployeeId { get; set; }
        public required ApplicationUser AssignedTo { get; set; }

        public DateTime? CompletedDate { get; set; }

        // Navigation properties
        public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Completed
    }
}
