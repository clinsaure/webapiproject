using Microsoft.Extensions.Logging;
using WebApiProject.DataService.Data;
using WebApiProject.DataService.IRepositories;
using WebApiProject.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiProject.DataService.IRepositories;

public interface IRefreshTokensRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken> GetByRefreshToken(string refreshToken);
    Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken);
}

