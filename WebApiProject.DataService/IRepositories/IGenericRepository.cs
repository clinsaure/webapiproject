using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiProject.DataService.IRepositories;

public interface IGenericRepository<T> where T : class
{
    // Get all entities
    Task<IEnumerable<T>> All();

    // Get by id
    Task<T> GetById(Guid id);

    // Add entity
    Task<bool> Add(T entity);

    // delete entity
    Task<bool> Delete(Guid id, string userId);

    // Update entity or add if it does not exit
    Task<bool> Upsert(T entity);
}

