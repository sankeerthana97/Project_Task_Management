using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Task_Management.Models
{
    public class TaskComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Content { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey("Task")]
        public int TaskId { get; set; }
        public required Task Task { get; set; }

        [Required]
        [ForeignKey("CreatedBy")]
        public required string UserId { get; set; }
        public required ApplicationUser CreatedBy { get; set; }
    }
}
