using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Project_Task_Management.Services;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Project_Task_Management.Data;
using Project_Task_Management.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Http; // For StringValues and session
using System.Linq; // For FirstOrDefault
using System.Collections.Generic; // For IEnumerable
using System.IdentityModel.Tokens.Jwt; // For JwtSecurityTokenHandler

var builder = WebApplication.CreateBuilder(args);

//  Debug log to confirm JWT Key is loaded
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is missing in configuration.");
Console.WriteLine("JWT Key from config: " + jwtKey);

//  Add controller support
builder.Services.AddControllers();

//  Configure EF Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity with EF stores
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

//  Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//  Swagger for API testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//  Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI();

//  Commented out HTTPS redirection since you're not using HTTPS locally
// app.UseHttpsRedirection();

// Redirect root URL "/" to /pages/login.html
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/pages/login.html");
        return;
    }
    await next();
});

app.UseRouting();

app.UseSession();

app.UseCors("AllowAll");
app.UseStaticFiles();

// Add JWT token middleware to handle token storage and validation
app.Use(async (context, next) =>
{
    // Try to get token from cookie first
    var token = context.Request.Cookies["jwt"];
    if (string.IsNullOrEmpty(token))
    {
        // If no cookie, try to get from Authorization header
        var authHeader = context.Request.Headers["Authorization"];
        if (!string.IsNullOrEmpty(authHeader.ToString()) && authHeader.ToString().StartsWith("Bearer "))
        {
            token = authHeader.ToString().Substring("Bearer ".Length).Trim();
        }
    }

    if (!string.IsNullOrEmpty(token))
    {
        try
        {
            // Get the token validation parameters from the JWT configuration
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };

            var handler = new JwtSecurityTokenHandler();
            var tokenClaims = handler.ReadJwtToken(token).Claims;
            
            // Create a new ClaimsIdentity with the token claims
            var identity = new ClaimsIdentity(tokenClaims, "jwt");
            
            // Add additional claims if needed
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, tokenClaims.FirstOrDefault(c => c.Type == "sub")?.Value ?? ""));
            
            // Set the user principal
            context.User = new ClaimsPrincipal(identity);
            
            // Store token in session
            context.Session.SetString("JwtToken", token);
            
            // Set the token in the context for API requests
            context.Items["JwtToken"] = token;
        }
        catch
        {
            // If token is invalid, continue without authentication
            context.User = null;
        }
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Custom middleware to handle redirects after login
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        var user = context.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Manager")
            {
                context.Response.Redirect("/dashboard/manager");
            }
            else if (role == "TeamLead")
            {
                context.Response.Redirect("/dashboard/teamlead");
            }
            else if (role == "Employee")
            {
                context.Response.Redirect("/dashboard/employee");
            }
            else
            {
                context.Response.Redirect("/pages/login.html");
            }
            return;
        }
        context.Response.Redirect("/pages/login.html");
        return;
    }
    await next();
});

// Apply migrations and seed roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        context.Database.Migrate();

        // Seed default roles
        string[] roles = { "Manager", "TeamLead", "Employee" };
        foreach (var roleName in roles)
        {
            if (!roleManager.RoleExistsAsync(roleName).Result)
            {
                roleManager.CreateAsync(new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} role"
                }).Wait();
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();
