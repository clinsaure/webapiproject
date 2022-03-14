using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApiProject.DataService.Data;
using WebApiProject.DataService.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiProject.DataService.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected AppDbContext _dbContext;

    protected DbSet<T> dbSet;

    protected readonly ILogger _logger;

    public GenericRepository(AppDbContext dbContext, ILogger logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        dbSet = dbContext.Set<T>();
    }

    public virtual async Task<bool> Add(T entity)
    {
        await dbSet.AddAsync(entity);
        return true;
    }

    public virtual async Task<IEnumerable<T>> All()
    {
        return await dbSet.ToListAsync();
    }

    //public virtual Task<IEnumerable<T>> GetAllBy(int id)
    //{
    //    throw new NotImplementedException();
    //}

    public virtual Task<bool> Delete(Guid id, string userId)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<T> GetById(Guid id)
    {
        return await dbSet.FindAsync(id);
    }

    public virtual Task<bool> Upsert(T entity)
    {
        throw new NotImplementedException();
    }
}

