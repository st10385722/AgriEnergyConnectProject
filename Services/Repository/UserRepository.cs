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

    public User GetUserWithRole(string email){
        return _context.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Email == email);
    }

    public UserRole getUserRole(int roleId){
        return _context.UserRoles.FirstOrDefault(r => r.RoleId == roleId);
    }
    public async Task<IEnumerable<User>> GetAll(){
        return await _context.Users.ToListAsync();
    }

    public async Task<User> GetById(int userId){
        return await _context.Users.FindAsync(userId);
    }

    public bool EmailExists(string email){
        return _context.Users.Any(u => u.Email == email);
    }

    public async Task Insert(User user){
        await _context.Users.AddAsync(user);
    }

    public void Update(User user){
        _context.Entry(user).State = EntityState.Modified;
    }
    public async Task Delete(int userId){
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
