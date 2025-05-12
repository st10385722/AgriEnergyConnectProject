using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services;
using Agri_EnergyConnect.Services.IRepository;
using Agri_EnergyConnect.Services.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AgriEnergyConnectDbContext>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
//adding authentication
builder.Services.AddAuthentication("CustomAuthentication")
    .AddCookie("CustomAuthentication", options => {
        options.ExpireTimeSpan = new TimeSpan(0,30,0);
        options.LoginPath = "/UserAccount/Login";
        options.AccessDeniedPath = "/UserAccount/AccessDenied";
    }
);

//authorization using UserRole table
builder.Services.AddAuthorization(options =>{
    options.AddPolicy("Employee", policy =>
        policy.RequireRole("employee"));

    options.AddPolicy("Farmer", policy =>
        policy.RequireRole("farmer"));

    options.AddPolicy("Admin", policy =>
        policy.RequireRole("admin"));    
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//setting up db things for auto generation
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AgriEnergyConnectDbContext>();
    var hasher = new PasswordHasher<object>();

    // Ensure the database is created
    await dbContext.Database.EnsureCreatedAsync();

    // 1. Seed Roles if they don't exist
    if (!await dbContext.UserRoles.AnyAsync())
    {
        var roles = new List<UserRole>
        {
            new UserRole { RoleName = "farmer", RoleDescription = "Farmer role" },
            new UserRole { RoleName = "employee", RoleDescription = "Employee role" },
            new UserRole { RoleName = "admin", RoleDescription = "Administrator role" }
        };
        
        await dbContext.UserRoles.AddRangeAsync(roles);
        await dbContext.SaveChangesAsync();
    }

    // Get roles fresh from database
    var existingRoles = await dbContext.UserRoles.ToListAsync();

    // 2. Create Default Users
    var defaultUsers = new[]
    {
        new { Username = "farmer@test.com", Password = "F@rmer1234",  Role = "farmer" },
        new { Username = "employee@test.com", Password = "Emp!oyee123", Role = "employee" },
        new { Username = "admin@test.com", Password = "Adm!n1234", Role = "admin" }
    };

    foreach (var user in defaultUsers)
{
    if (!await dbContext.Users.AnyAsync(u => u.Username == user.Username))
    {
        var role = existingRoles.FirstOrDefault(r => r.RoleName == user.Role);
        if (role == null) continue;

        var newUser = new User
        {
            UserId = Guid.NewGuid().GetHashCode(), // Generate a unique UserId
            Username = user.Username,
            PasswordHash = hasher.HashPassword(null, user.Password),
            RoleId = role.RoleId,
            CreatedBy = 1, // System user
            CreatedAt = DateTime.UtcNow
        };

        // Use AsNoTracking to avoid tracking conflicts
        var existingUser = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == newUser.UserId);

        if (existingUser == null)
        {
            dbContext.Users.Add(newUser);
            //dbContext.Entry(existingUser).State = EntityState.Detached;
        }
    }
}
await dbContext.SaveChangesAsync();
}
app.Run();
