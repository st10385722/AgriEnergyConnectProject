using Agri_EnergyConnect.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Agri_EnergyConnect.Services
{
    //this class is used for handling the logistics behind uploading an
    //image to the database when you create a new product
    //as with all repositories, it uses an interface layer to
    //abstract database operations in the controller

    //the interface that controller utilize when adding a product image
    //to the database
    public interface IProductImageService{
        Task<ProductImage> ProcessImageAsync(IFormFile file, int productId);
        Task Insert(ProductImage productImage);
        Task<ProductImage> GetProductImageById(int productImageId);
        void Update(ProductImage productImage);
        Task Delete(int productImageId);
        Task SaveAsync();
    }

    //the product image service class that houses all of the
    //database operations, abstracted via the IProductService repository
    public class ProductImageService:IProductImageService
    {
        //database declaration
        private readonly St10385722AgriEnergyConnectDbContext _context;

        //constructor
        public ProductImageService(St10385722AgriEnergyConnectDbContext context)
        {
            _context = context;
        }

        //inserting a product image object into the database
        public async Task Insert(ProductImage productImage)
        {
            await _context.ProductImages.AddAsync(productImage);
        }

        //gets a project image via its id
        public async Task<ProductImage> GetProductImageById(int ImageId)
        {
            return await _context.ProductImages.FirstOrDefaultAsync(p => p.ImageId.Equals(ImageId));
        }

        //updates a product image
        //used when changing the file when creating a new product
        public void Update(ProductImage productImage)
        {
            _context.Entry(productImage).State = EntityState.Modified;
        }

        //deleting a product image object
        //used when you delete a product
        public async Task Delete(int productImageId)
        {
            ProductImage productImage = await _context.ProductImages.FindAsync(productImageId);

            if (productImage != null)
            {
                _context.ProductImages.Remove(productImage);
            }
        }

        //saves changes to the database
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        //this method is used to convert the image from a IFormFile format into the
        //byte stream array format for adding to the database
        public async Task<ProductImage> ProcessImageAsync(IFormFile file, int productId)
        {

            // Sanitize file name using regex to replace common file name errors to underscore
            var sanitizedFileName = Regex.Replace(file.FileName, @"[^a-zA-Z0-9_\-\.]", "_");

            // Convert file to byte array
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileData = memoryStream.ToArray();
            }

            // Create ProductImage object
            return new ProductImage
            {
                ProductId = productId,
                FileName = sanitizedFileName,
                ContentType = file.ContentType,
                ImageData = fileData,
                CreatedAt = DateTime.UtcNow
            };
        }

        //this code is used for releasing resources used by the repository
        //such as the database context
        //it reduces resource leaks
        //and ensures the method is executed once before being disposed
        private bool disposed = false;

        protected virtual void Dispose(bool disposing){
            if(!this.disposed){
                if(disposing){
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose(){
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}