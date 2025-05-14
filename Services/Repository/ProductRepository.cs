using System;
using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Agri_EnergyConnect.Services.Repository;

public class ProductRepository: IProductRepository
{
    //get database instance inside repository
     private readonly St10385722AgriEnergyConnectDbContext _context;

    //constructor
    public ProductRepository(St10385722AgriEnergyConnectDbContext context){
        _context = context;
    }
    //gets all products, regardless of farmer
    public async Task<IEnumerable<Product>> GetAll(){
        return await _context.Products.ToListAsync();
    }

    //gets the product object via its id
    public async Task<Product> GetById(int ProductId){
        return await _context.Products.FindAsync(ProductId);
    }

    //gets a list of products via the farmer id reference id
    public async Task<IEnumerable<Product>> GetProductByFarmerId(int farmerId){
        return await _context.Products.Where(p => p.FarmerId.Equals(farmerId)).ToListAsync();
    }

    //generic insert
    public async Task Insert(Product Product){
        await _context.Products.AddAsync(Product);
    }

    //generic update
    public void Update(Product Product){
        _context.Entry(Product).State = EntityState.Modified;
    }
    //generic delete
    public async Task Delete(int productId){
        Product product = await _context.Products.FindAsync(productId);

        if(product != null){
            _context.Products.Remove(product);
        }
    }

    //saves the database changes from local to databse
    public async Task SaveAsync(){
        await _context.SaveChangesAsync();
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
