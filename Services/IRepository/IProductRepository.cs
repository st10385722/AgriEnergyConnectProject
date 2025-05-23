using System;
using Agri_EnergyConnect.Models;

namespace Agri_EnergyConnect.Services.IRepository;
//interface to abstract the database access from the product controller code

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAll();
    Task<Product> GetById(int productId);
    Task<IEnumerable<Product>> GetProductByFarmerId(int farmerId);
    Task Insert( Product product);
    void Update(Product product);
    Task Delete(int productId);
    Task SaveAsync();
}
