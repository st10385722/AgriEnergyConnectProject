using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services.IRepository;
using Agri_EnergyConnect.Services.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AgriEnergyConnectDbContext>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

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


app.Run();
