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
    Task<bool> UpdateUserStatus(Guid identityId);
    Task<User> GetByIdentityId(Guid identityId);
}

