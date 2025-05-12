using System;
using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Agri_EnergyConnect.Services.Repository;

public class FarmerRepository:IFarmerRepository
{
    private readonly AgriEnergyConnectDbContext _context;

    public FarmerRepository(AgriEnergyConnectDbContext context){
        _context = context;
    }
    public async Task<IEnumerable<Farmer>> GetAll(){
        return await _context.Farmers.ToListAsync();
    }

    public async Task<Farmer> GetById(int farmerId){
        return await _context.Farmers.FindAsync(farmerId);
    }

    public async Task<IEnumerable<Farmer>> GetProductsByFarmerId(int farmerId){
        return await _context.Farmers.Where(f => f.FarmerId.Equals(farmerId)).ToListAsync();
    }

    public async Task<Farmer> GetFarmerByUserId(int userId){
        return await _context.Farmers.FirstOrDefaultAsync(f => f.UserId.Equals(userId));
    }

    public async Task Insert(Farmer farmer){
        await _context.Farmers.AddAsync(farmer);
    }

    public void Update(Farmer farmer){
        _context.Entry(farmer).State = EntityState.Modified;
    }

    public async Task Delete(int farmerId){
        Farmer farmer = await _context.Farmers.FindAsync(farmerId);

        if(farmer != null){
            _context.Farmers.Remove(farmer);
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
