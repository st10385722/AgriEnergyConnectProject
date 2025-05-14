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

//setting app up to auto create roles and users for the tables
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<St10385722AgriEnergyConnectDbContext>();
    var productImageService = scope.ServiceProvider.GetRequiredService<IProductImageService>();
    await SeedData.InitializeAsync(dbContext, productImageService);
}
app.Run();

//references
//https://www.farmbrite.com/post/essential-farm-records-and-data-you-need-to-be-tracking
//https://bootswatch.com/
