using Agri_EnergyConnect.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Agri_EnergyConnect.Services
{
    public class ProductImageService:IProductImageService
    {
        private readonly St10385722AgriEnergyConnectDbContext _context;

        public ProductImageService(St10385722AgriEnergyConnectDbContext context)
        {
            _context = context;
        }

        public async Task Insert(ProductImage productImage)
        {
            await _context.ProductImages.AddAsync(productImage);
        }

        public async Task<ProductImage> GetProductImageById(int ImageId)
        {
            return await _context.ProductImages.FirstOrDefaultAsync(p => p.ImageId.Equals(ImageId));
        }

        public void Update(ProductImage productImage)
        {
            _context.Entry(productImage).State = EntityState.Modified;
        }

        public async Task Delete(int productImageId)
        {
            ProductImage productImage = await _context.ProductImages.FindAsync(productImageId);

            if (productImage != null)
            {
                _context.ProductImages.Remove(productImage);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<ProductImage> ProcessImageAsync(IFormFile file, int productId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            // Sanitize file name
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

    public interface IProductImageService{
        Task<ProductImage> ProcessImageAsync(IFormFile file, int productId);
        Task Insert(ProductImage productImage);
        Task<ProductImage> GetProductImageById(int productImageId);
        void Update(ProductImage productImage);
        Task Delete(int productImageId);
        Task SaveAsync();
    }
}