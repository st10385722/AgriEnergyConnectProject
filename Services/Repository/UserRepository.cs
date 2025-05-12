using System;
using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Agri_EnergyConnect.Services.Repository;

public class UserRepository : IUserRepository
{
    private readonly AgriEnergyConnectDbContext _context;

    public UserRepository(AgriEnergyConnectDbContext context){
        _context = context;
    }

    public User GetUserWithRole(string username){
        return _context.Users
            .Include(u => u.RoleId)
            .FirstOrDefault(u => u.Username == username);
    }

    public UserRole getUserRole(int roleId){
        return _context.UserRoles.FirstOrDefault(r => r.RoleId == roleId);
    }
    public async Task<IEnumerable<User>> GetAll(){
        return await _context.Users.ToListAsync();
    }

    public async Task<User> GetById(string listingId){
        return await _context.Users.FindAsync(listingId);
    }

    public bool UsernameExists(string username){
        return _context.Users.Any(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetProductsByUserId(string userId){
        return await _context.Users.Where(l => l.UserId.Equals(userId)).ToListAsync();
    }

    public async Task Insert(User user){
        await _context.Users.AddAsync(user);
    }

    public void Update(User user){
        _context.Entry(user).State = EntityState.Modified;
    }
    public async Task Delete(string userId){
        User user = await _context.Users.FindAsync(userId);

        if(user != null){
            _context.Users.Remove(user);
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
