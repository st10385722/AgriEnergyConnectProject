using System;
using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Agri_EnergyConnect.Services.Repository;

public class ProductRepository: IProductRepository
{
     private readonly AgriEnergyConnectDbContext _context;

    public ProductRepository(AgriEnergyConnectDbContext context){
        _context = context;
    }
    public async Task<IEnumerable<Product>> GetAll(){
        return await _context.Products.ToListAsync();
    }

    public async Task<Product> GetById(int ProductId){
        return await _context.Products.FindAsync(ProductId);
    }

    public async Task<IEnumerable<Product>> GetProductByFarmerId(int farmerId){
        return await _context.Products.Where(p => p.FarmerId.Equals(farmerId)).ToListAsync();
    }

    public async Task Insert(Product Product){
        await _context.Products.AddAsync(Product);
    }

    public void Update(Product Product){
        _context.Entry(Product).State = EntityState.Modified;
    }
    public async Task Delete(int productId){
        Product product = await _context.Products.FindAsync(productId);

        if(product != null){
            _context.Products.Remove(product);
        }
    }

    public async Task SaveAsync(){
        await _context.SaveChangesAsync();
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
