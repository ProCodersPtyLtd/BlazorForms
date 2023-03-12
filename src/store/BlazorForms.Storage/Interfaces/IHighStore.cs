using BlazorForms.Storage.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Storage
{
    // IHighStorage ?
    public interface IHighStore
    {
        Task<List<T>> GetAllAsync<T>() where T : class, IEntity;
        Task<T> UpsertAsync<T>(T entity) where T : class, IEntity;
        Task<T> GetByIdAsync<T>(int id) where T : class, IEntity;
        Task DeleteAsync<T>(int id) where T : class, IEntity;
        Task SoftDeleteAsync<T>(T entity) where T : class, IEntity;
        Task SoftDeleteAsync<T>(int id) where T : class, IEntity;
        //Task<List<T>> GetListByIdsAsync<T>(IEnumerable<int> ids);
        ContextQuery<T> GetQuery<T>() where T: class, IEntity;
        ContextQuery<T> GetByIdQuery<T>(int id) where T : class, IEntity;
        Task<T> GetByIdAsync<T>(int id) where T : class, IEntity;
        Task DeleteAsync<T>(int id) where T : class, IEntity;
        Task SoftDeleteAsync<T>(T entity) where T : class, IEntity;
        Task SoftDeleteAsync<T>(int id) where T : class, IEntity;
        //Task<List<T>> GetListByIdsAsync<T>(IEnumerable<int> ids);
    }
}

/*
// join usage:
var query = _db.GetListQuery<UserModel>();
query.Include(u => u.Person); // full object
query.Include(u => u.Person, t => t.FullName); // only one field
Model = await _db.ExecuteAsync(query);
Model.AllPersons = await _db.GetListAsync<PersonModel>();

// search usage:

// sort usage:

 */