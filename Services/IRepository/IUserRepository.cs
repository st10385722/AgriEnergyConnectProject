using System;
using Agri_EnergyConnect.Models;

namespace Agri_EnergyConnect.Services.IRepository;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User> GetById(string userId);
    Task<IEnumerable<User>> GetProductsByUserId(string userId);
    Task Insert(User user);
    void Update(User user);
    Task Delete(string userId);
    Task SaveAsync();
}
