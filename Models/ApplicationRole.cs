using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Project_Task_Management.Models
{
    public class ApplicationRole : IdentityRole<string>
    {
        public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public required string Description { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}