using BlazorForms.Storage.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Storage.Interfaces
{
    public interface IModelRepository<T>
        where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<int> CreateAsync(T data);
        Task UpdateAsync(T data);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(T data);
        Task SoftDeleteAsync(int id);
        Task<List<T>> GetListByIdsAsync(IEnumerable<int> ids);

        ContextQuery<T> GetContextQuery();
        // for joins with other qieries from the same db context
        ContextQuery<T> GetContextQuery(DbContext context);
        Task<List<T>> RunContextQueryAsync(ContextQuery<T> ctx);
    }
}
