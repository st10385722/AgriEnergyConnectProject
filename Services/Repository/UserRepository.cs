using System;
using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Agri_EnergyConnect.Services.Repository;
//repository that access the database, and is abstracted through the IUserRepository interface that it extends
public class UserRepository : IUserRepository
{
    //db context declaration
    private readonly St10385722AgriEnergyConnectDbContext _context;

    //constructor
    public UserRepository(St10385722AgriEnergyConnectDbContext context){
        _context = context;
    }

    //finds the user via their email, and also includes their role
    //used in login for assigning claims
    public User GetUserWithRole(string email){
        return _context.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Email == email);
    }

    //get user role object via the role id in the UserRole table
    //used for checking if a user is in a certain role
    public UserRole getUserRole(int roleId){
        return _context.UserRoles.FirstOrDefault(r => r.RoleId == roleId);
    }
    //gets all users
    public async Task<IEnumerable<User>> GetAll(){
        return await _context.Users.ToListAsync();
    }

    //gets a user via their userId
    public async Task<User> GetById(int userId){
        return await _context.Users.FindAsync(userId);
    }

    //checks whether an email already exists in the database
    //indiciating if that email has an account or not.
    //if it does, it return 1, and the account cannot be made
    public bool EmailExists(string email){
        return _context.Users.Any(u => u.Email == email);
    }

    //generic insert
    public async Task Insert(User user){
        await _context.Users.AddAsync(user);
    }

    //generic update
    public void Update(User user){
        _context.Entry(user).State = EntityState.Modified;
    }
    //generic delete
    public async Task Delete(int userId){
        User user = await _context.Users.FindAsync(userId);

        if(user != null){
            _context.Users.Remove(user);
        }
    }

    //save context changes to database
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
