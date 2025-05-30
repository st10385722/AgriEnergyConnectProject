using System;
using Agri_EnergyConnect.Models;

namespace Agri_EnergyConnect.Services.IRepository;

//interface to abstract the database access from the farmer controller code
public interface IFarmerRepository
{
    Task<IEnumerable<Farmer>> GetAll();
    Task<Farmer> GetById(int farmerId);
    Task Insert(Farmer farmer);
    void Update(Farmer farmer);
    Task<Farmer> GetFarmerByUserId(int userId);
    Task Delete(int farmerId);
    Task SaveAsync();
}
