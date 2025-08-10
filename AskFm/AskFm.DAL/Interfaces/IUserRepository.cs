using AskFm.DAL.Models;

namespace AskFm.DAL.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);

}