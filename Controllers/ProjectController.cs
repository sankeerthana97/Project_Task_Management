using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Task_Management.Data;
using Project_Task_Management.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Project_Task_Management.Models;
using Project_Task_Management.Services;

namespace Project_Task_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public ProjectController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: api/Project
        [HttpGet]
        [Authorize(Roles = "Manager,TeamLead")]
        public async Task<IActionResult> GetProjects()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null) return Unauthorized();

            if (roles.Contains("Manager"))
            {
                return Ok(await _context.Projects
                    .Include(p => p.TeamLead)
                    .Include(p => p.ProjectAssignments)
                    .ThenInclude(pa => pa.Employee)
                    .ToListAsync());
            }
            else if (roles.Contains("TeamLead"))
            {
                return Ok(await _context.Projects
                    .Where(p => p.TeamLeadId == userId)
                    .Include(p => p.TeamLead)
                    .Include(p => p.ProjectAssignments)
                    .ThenInclude(pa => pa.Employee)
                    .ToListAsync());
            }

            return Unauthorized();
        }

        // GET: api/Project/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Manager,TeamLead")]
        public async Task<IActionResult> GetProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.TeamLead)
                .Include(p => p.ProjectAssignments)
                .ThenInclude(pa => pa.Employee)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        // POST: api/Project
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateProject([FromBody] Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            
            project.TeamLeadId = userId;
            project.CreatedAt = DateTime.UtcNow;

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        // PUT: api/Project/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProject = await _context.Projects.FindAsync(id);
            if (existingProject == null)
            {
                return NotFound();
            }

            existingProject.Title = project.Title;
            existingProject.Description = project.Description;
            existingProject.Requirements = project.Requirements;
            existingProject.StartDate = project.StartDate;
            existingProject.EndDate = project.EndDate;
            existingProject.Status = project.Status;
            existingProject.NumberOfPeopleNeeded = project.NumberOfPeopleNeeded;
            existingProject.Criticality = project.Criticality;
            existingProject.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Project/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Project/5/AssignEmployee
        [HttpPost("{projectId}/AssignEmployee")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AssignEmployeeToProject(int projectId, [FromBody] string employeeId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            var employee = await _userManager.FindByIdAsync(employeeId);
            if (employee == null) return NotFound();
            if (string.IsNullOrEmpty(employee.Email)) return BadRequest("Employee email is required");

            var existingAssignment = await _context.ProjectAssignments
                .FirstOrDefaultAsync(pa => 
                    pa.ProjectId == projectId && 
                    pa.EmployeeId == employeeId && 
                    pa.IsActive);

            if (existingAssignment != null)
            {
                return BadRequest("Employee is already assigned to this project");
            }

            var assignment = new ProjectAssignment
            {
                ProjectId = projectId,
                EmployeeId = employeeId,
                AssignmentDate = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Project = project,
                Employee = employee
            };

            _context.ProjectAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            // Send email notification to employee
            await SendAssignmentEmail(employee, project);

            return Ok();
        }

        private async System.Threading.Tasks.Task SendAssignmentEmail(ApplicationUser employee, Project project)
        {
            var subject = "Project Assignment: " + project.Title;
            var body = "<h3>Project Assignment Notification</h3>" +
                        "<p>Dear " + employee.FirstName + " " + employee.LastName + ",</p>" +
                        "<p>You have been assigned to the project: <strong>" + project.Title + "</strong></p>" +
                        "<p>Project Details:</p>" +
                        "<ul>" +
                            "<li>Start Date: " + project.StartDate.ToString("dd/MM/yyyy") + "</li>" +
                            "<li>End Date: " + project.EndDate.ToString("dd/MM/yyyy") + "</li>" +
                            "<li>Required Skills: " + project.SkillsRequired + "</li>" +
                            "<li>Criticality: " + project.Criticality + "</li>" +
                        "</ul>" +
                        "<p>Please check your dashboard for more details.</p>" +
                        "<p>Best regards,<br>Project Management Team</p>";

            if (!string.IsNullOrEmpty(employee.Email))
            {
                await _emailService.SendEmailAsync(employee.Email, subject, body);
            }
        }
    }
}
