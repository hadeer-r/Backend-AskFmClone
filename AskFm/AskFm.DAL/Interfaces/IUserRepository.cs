using AskFm.DAL.Models;

namespace AskFm.DAL.Interfaces;

public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetByUsernameAsync(string username);

}