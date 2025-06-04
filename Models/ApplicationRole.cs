using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Project_Task_Management.Models
{
    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole() : base()
        {
            ConcurrencyStamp = Guid.NewGuid().ToString();
        }

        public override string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Description { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}