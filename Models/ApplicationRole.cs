using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Project_Task_Management.Models
{
    public class ApplicationRole : IdentityRole
    {
        [Required]
        public required string Description { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
} 