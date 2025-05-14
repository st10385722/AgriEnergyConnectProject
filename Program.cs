using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services;
using Agri_EnergyConnect.Services.IRepository;
using Agri_EnergyConnect.Services.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//not secure, but 
//added for ease of use instead of using env variables
builder.Services.AddDbContext<St10385722AgriEnergyConnectDbContext>(options =>{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//dependency inection for my repository services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFarmerRepository, FarmerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();

//adds httpcontext it the project, which allows the current user id
//to be store in a http cookie
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
//adding authentication, with a cookie expiry of 1 hour, which deletes it and prompts you
// to login again
builder.Services.AddAuthentication("CustomAuthentication")
    .AddCookie("CustomAuthentication", options => {
        options.ExpireTimeSpan = new TimeSpan(1,0,0);
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

//setting app up to auto create roles and users for the tables`
using (var scope = app.Services.CreateScope())
{
    //gets db context and password hasher
    var dbContext = scope.ServiceProvider.GetRequiredService<St10385722AgriEnergyConnectDbContext>();
    var hasher = new PasswordHasher<object>();

    // Ensure the database is created
    await dbContext.Database.EnsureCreatedAsync();

    // 1. Seed Roles if they don't exist
    if (!await dbContext.UserRoles.AnyAsync())
    {
        var roles = new List<UserRole>
        {
            new UserRole { RoleId = 1, RoleName = "admin", RoleDescription = "Administrator role" },
            new UserRole { RoleId = 2, RoleName = "employee", RoleDescription = "Employee role" },
            new UserRole { RoleId = 3,RoleName = "farmer", RoleDescription = "Farmer role" }
        };
        
        await dbContext.UserRoles.AddRangeAsync(roles);
        await dbContext.SaveChangesAsync();
    }

    // Get roles fresh from database
    var existingRoles = await dbContext.UserRoles.ToListAsync();

    // 2. Create Default Users
    var defaultUsers = new[]
    {
        new { Username = "admin@test.com", Password = "Adm!n1234", Email = "admin@test.com", Role = "admin" },  
        new { Username = "employee@test.com", Password = "Emp!oyee123", Email = "employee@test.com", Role = "employee" },  
        new { Username = "farmer@test.com", Password = "F@rmer1234", Email = "farmer@test.com", Role = "farmer" }
    };
    //a count to add a new userid incrementally
    int count = 1;
    // creates the generic users with foreach
    foreach (var user in defaultUsers)
    {
        if (!await dbContext.Users.AnyAsync(u => u.Username == user.Username))
        {
            var role = existingRoles.FirstOrDefault(r => r.RoleName == user.Role);
            if (role == null) continue;

            var newUser = new User
            {
                //increments user id for role assignment
                UserId = count++,
                Username = user.Username,
                //hashing password
                PasswordHash = hasher.HashPassword(null, user.Password),
                Email = user.Email,
                RoleId = role.RoleId,
                CreatedBy = 0,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Users.Add(newUser);
        }
    }
    await dbContext.SaveChangesAsync();
}
app.Run();

//references
//https://www.farmbrite.com/post/essential-farm-records-and-data-you-need-to-be-tracking
//https://bootswatch.com/
