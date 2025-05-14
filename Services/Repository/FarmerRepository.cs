using System;
using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Agri_EnergyConnect.Services.Repository;

public class FarmerRepository:IFarmerRepository
{
    //creating a single instance of the db contex for the farmer repository
    private readonly St10385722AgriEnergyConnectDbContext _context;

    //constructor
    public FarmerRepository(St10385722AgriEnergyConnectDbContext context){
        _context = context;
    }
    //gets all farmers, used for employee view
    public async Task<IEnumerable<Farmer>> GetAll(){
        return await _context.Farmers.ToListAsync();
    }

    //gets the farmer via their id
    public async Task<Farmer> GetById(int farmerId){
        return await _context.Farmers.FindAsync(farmerId);
    }

    //gets products by the farmer id, used by employee farmer index
    public async Task<IEnumerable<Farmer>> GetProductsByFarmerId(int farmerId){
        return await _context.Farmers.Where(f => f.FarmerId.Equals(farmerId)).ToListAsync();
    }

    //gets the farmer via their user id, useful for farmer page
    public async Task<Farmer> GetFarmerByUserId(int userId){
        return await _context.Farmers.FirstOrDefaultAsync(f => f.UserId.Equals(userId));
    }

    //generic insert method for farmer
    public async Task Insert(Farmer farmer){
        await _context.Farmers.AddAsync(farmer);
    }

    //update method for farmer
    public void Update(Farmer farmer){
        _context.Entry(farmer).State = EntityState.Modified;
    }

    //delete method for farmer
    public async Task Delete(int farmerId){
        Farmer farmer = await _context.Farmers.FindAsync(farmerId);

        if(farmer != null){
            _context.Farmers.Remove(farmer);
        }
    }

    //save context
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
