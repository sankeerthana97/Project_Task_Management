using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Task_Management.Data;
using Project_Task_Management.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Project_Task_Management.Services;

namespace Project_Task_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public TaskController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: api/Task
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTasks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);

            if (role.Contains("Manager"))
            {
                return Ok(await _context.Tasks
                    .Include(t => t.Project)
                    .Include(t => t.AssignedTo)
                    .ToListAsync());
            }
            else if (role.Contains("TeamLead"))
            {
                var teamLeadProjects = await _context.Projects
                    .Where(p => p.TeamLeadId == userId)
                    .Select(p => p.Id)
                    .ToListAsync();

                return Ok(await _context.Tasks
                    .Where(t => teamLeadProjects.Contains(t.ProjectId))
                    .Include(t => t.Project)
                    .Include(t => t.AssignedTo)
                    .ToListAsync());
            }
            else
            {
                return Ok(await _context.Tasks
                    .Where(t => t.EmployeeId == userId)
                    .Include(t => t.Project)
                    .Include(t => t.AssignedTo)
                    .ToListAsync());
            }
        }

        // GET: api/Task/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                .ThenInclude(c => c.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        // POST: api/Task
        [HttpPost]
        [Authorize(Roles = "TeamLead")]
        public async System.Threading.Tasks.Task<IActionResult> CreateTask([FromBody] Project_Task_Management.Models.Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => 
                    p.Id == task.ProjectId && 
                    p.TeamLeadId == userId);

            if (project == null)
            {
                return BadRequest("Project not found or unauthorized");
            }

            task.CreatedDate = DateTime.UtcNow;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Send email notification to assigned employee
            await SendTaskAssignmentEmail(task);

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        // PUT: api/Task/5
        [HttpPut("{id}")]
        [Authorize]
        public async System.Threading.Tasks.Task<IActionResult> UpdateTask(int id, [FromBody] Project_Task_Management.Models.Task task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);

            // Only TeamLead can update task status and details
            if (role.Contains("TeamLead") && existingTask.Project.TeamLeadId != userId)
            {
                return Unauthorized();
            }
            else if (role.Contains("Employee") && existingTask.EmployeeId != userId)
            {
                return Unauthorized();
            }

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Status = task.Status;
            existingTask.DueDate = task.DueDate;
            existingTask.Priority = task.Priority;

            if (role.Contains("Employee") && task.Status == Project_Task_Management.Models.TaskStatus.Completed)
            {
                existingTask.CompletedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Task/5/Comment
        [HttpPost("{taskId}/Comment")]
        [Authorize]
        public async Task<IActionResult> AddComment(int taskId, [FromBody] TaskComment comment)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);

            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                return NotFound();
            }

            var role = await _userManager.GetRolesAsync(user);
            if (!role.Contains("Manager") && 
                !role.Contains("TeamLead") && 
                !task.EmployeeId.Equals(userId))
            {
                return Unauthorized();
            }

            comment.UserId = userId;
            comment.TaskId = taskId;
            comment.CreatedDate = DateTime.UtcNow;

            _context.TaskComments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = taskId }, task);
        }

        private async System.Threading.Tasks.Task SendTaskAssignmentEmail(Project_Task_Management.Models.Task task)
        {
            var employee = await _userManager.FindByIdAsync(task.EmployeeId);
            if (employee == null) return;
            if (string.IsNullOrEmpty(employee.Email)) return;

            var subject = "New Task Assignment: " + task.Title;
            var body = "<h3>Task Assignment Notification</h3>" +
                        "<p>Dear " + employee.FirstName + " " + employee.LastName + ",</p>" +
                        "<p>You have been assigned a new task: <strong>" + task.Title + "</strong></p>" +
                        "<p>Task Details:</p>" +
                        "<ul>" +
                            "<li>Project: " + task.Project.Title + "</li>" +
                            "<li>Description: " + task.Description + "</li>" +
                            "<li>Due Date: " + task.DueDate.ToString("dd/MM/yyyy") + "</li>" +
                            "<li>Priority: " + task.Priority + "</li>" +
                        "</ul>" +
                        "<p>Please check your dashboard for more details.</p>" +
                        "<p>Best regards,<br>Project Management Team</p>";

            await _emailService.SendEmailAsync(employee.Email, subject, body);
        }
    }
}
