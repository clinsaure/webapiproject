using Microsoft.Extensions.Logging;
using WebApiProject.DataService.IConfiguration;
using WebApiProject.DataService.IRepositories;
using WebApiProject.DataService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiProject.DataService.Data;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;


    public IUsersRepository Users { get; private set; }
    public IRefreshTokensRepository RefreshTokens { get; private set; }

    public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger("db_logs");

        Users = new UsersRepository(_context, _logger);
        RefreshTokens = new RefreshTokensRepository(_context, _logger);
    }

    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
