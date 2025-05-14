using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Agri_EnergyConnect.Models;

//this class seeds the database with the values, provided it has the corrent table
public static class SeedData
{
    public static async Task InitializeAsync(St10385722AgriEnergyConnectDbContext dbContext, IProductImageService productImageService)
    {
        var hasher = new PasswordHasher<object>();

        // Ensure the database is created
        await dbContext.Database.EnsureCreatedAsync();

        // 1. Seed Roles
        if (!await dbContext.UserRoles.AnyAsync())
        {
            var roles = new List<UserRole>
            {
                new UserRole { RoleId = 1, RoleName = "admin", RoleDescription = "Administrator role" },
                new UserRole { RoleId = 2, RoleName = "employee", RoleDescription = "Employee role" },
                new UserRole { RoleId = 3, RoleName = "farmer", RoleDescription = "Farmer role" }
            };

            await dbContext.UserRoles.AddRangeAsync(roles);
            await dbContext.SaveChangesAsync();
        }

        // 2. Seed Users
        if (!await dbContext.Users.AnyAsync())
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "admin@test.com",
                    //calls the hasher object to hash the password
                    PasswordHash = hasher.HashPassword(null, "Adm!n1234"),
                    Email = "admin@test.com",
                    RoleId = 1,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0
                },
                new User
                {
                    UserId = 2,
                    Username = "employee@test.com",
                    PasswordHash = hasher.HashPassword(null, "Emp!oyee1234"),
                    Email = "employee@test.com",
                    RoleId = 2,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1
                },
                new User
                {
                    UserId = 3,
                    Username = "farmer@test.com",
                    PasswordHash = hasher.HashPassword(null, "F@rmer1234"),
                    Email = "farmer@test.com",
                    RoleId = 3,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1
                }
            };

            await dbContext.Users.AddRangeAsync(users);
            await dbContext.SaveChangesAsync();
        }

        // 3. Seed Farmers
        if (!await dbContext.Farmers.AnyAsync())
        {
            var farmers = new List<Farmer>
            {
                new Farmer
                {
                    FarmerId = 1,
                    UserId = 3,
                    FarmName = "Diddly Squat Farms",
                    FarmType = "Crop Farm",
                    HavestingDate = new DateTime(2025, 5, 1),
                    CropType = "Corn",
                    LivestockType = null,
                    NumberOfEmployees = 10
                }
            };

            await dbContext.Farmers.AddRangeAsync(farmers);
            await dbContext.SaveChangesAsync();
        }

        // 4. Seed Products
        if (!await dbContext.Products.AnyAsync())
        {
            var products = new List<Product>
            {
                new Product
                {
                    ProductId = 1,
                    ProductName = "Organic Corn",
                    ProductType = "Crop",
                    ProductDescription = "High-quality organic corn",
                    Quantity = 100,
                    Price = 50.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    FarmerId = 1
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "Fresh Tomatoes",
                    ProductType = "Crop",
                    ProductDescription = "Freshly harvested tomatoes",
                    Quantity = 200,
                    Price = 30.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    FarmerId = 1
                }
            };

            await dbContext.Products.AddRangeAsync(products);
            await dbContext.SaveChangesAsync();
        }

        // 5. Seed Product Images
        if (!await dbContext.ProductImages.AnyAsync())
        {
            var productImages = new List<(int productId, string fileName)>
            {
                (1, "tomatoe.jpeg"),
                (2, "corn.jpeg")
            };

            foreach (var (productId, fileName) in productImages)
            {
                // Simulate file upload
                var filePath = Path.Combine("wwwroot", "content", fileName);
                if (File.Exists(filePath))
                {
                    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    var formFile = new FormFile(fileStream, 0, fileStream.Length, null, fileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/jpeg"
                    };

                    //calling the processImageAsync method from the service in order to convert it to byte array properly
                    var productImage = await productImageService.ProcessImageAsync(formFile, productId);
                    productImage.ImageId = Math.Abs(Guid.NewGuid().GetHashCode());
                    productImage.CreatedAt = DateTime.UtcNow;

                    await productImageService.Insert(productImage);
                    await productImageService.SaveAsync();

                    // Update the product with the new ProductImageId
                    var product = await dbContext.Products.FindAsync(productId);
                    if (product != null)
                    {
                        product.ProductImageId = productImage.ImageId;
                        dbContext.Products.Update(product);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            await productImageService.SaveAsync();
        }
    }
}