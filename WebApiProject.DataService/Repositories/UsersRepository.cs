using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApiProject.DataService.Data;
using WebApiProject.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiProject.DataService.IRepositories;

namespace WebApiProject.DataService.Repositories;

public class UsersRepository : GenericRepository<User>, IUsersRepository
{
    public UsersRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public override async Task<IEnumerable<User>> All()
    {
        try
        {
            return await dbSet.Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} All method error", typeof(UsersRepository));
            return new List<User>();
        }
    }

    public async Task<bool> UpdateUserProfile(User user)
    {
        try
        {
            var existingUser = await dbSet.Where(x => x.Status == 1)
                .FirstOrDefaultAsync();

            if (existingUser == null) return false;

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.PhoneNumber = user.PhoneNumber;
            //existingUser.Phone = user.Phone;
            existingUser.Sex = user.Sex;
            existingUser.Address = user.Address;
            existingUser.UpdateDate = DateTime.UtcNow;

            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} UpdateUserProfile method has generated an error", typeof(UsersRepository));
            return false;
        }
    }

    public async Task<User> GetByIdentityId(Guid identityId)
    {
        try
        {
            return await dbSet.Where(x => x.Status == 1 && x.IdentityId == identityId)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetByIdentityId method has generated an error", typeof(UsersRepository));
            return null;
        }
    }
}

