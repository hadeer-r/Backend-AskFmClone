using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AskFm.DAL.Repositories;

public class UserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ApplicationUser?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == username);
    }
    
}