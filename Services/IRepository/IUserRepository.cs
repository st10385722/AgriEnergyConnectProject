using System;
using Agri_EnergyConnect.Models;

namespace Agri_EnergyConnect.Services.IRepository;

public interface IUserRepository
{
    User GetUserWithRole(string email);
    UserRole getUserRole(int roleId);
    bool EmailExists(string email);
    Task<IEnumerable<User>> GetAll();
    Task<User> GetById(int userId);
    Task Insert(User user);
    void Update(User user);
    Task Delete(int userId);
    Task SaveAsync();
}
