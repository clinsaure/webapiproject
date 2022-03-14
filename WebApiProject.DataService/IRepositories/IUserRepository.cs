using WebApiProject.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiProject.DataService.IRepositories;

public interface IUsersRepository : IGenericRepository<User>
{
    Task<bool> UpdateUserProfile(User user);
    Task<User> GetByIdentityId(Guid identityId);
}

