using BlazorForms.Storage.Interfaces;
using BlazorForms.Storage.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Storage.InMemory
{
    public class InMemoryModelRepository<T> : IModelRepository<T>
        where T : class
    {
        public Task<int> CreateAsync(T data)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public ContextQuery<T> GetContextQuery()
        {
            throw new NotImplementedException();
        }

        public ContextQuery<T> GetContextQuery(DbContext context)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetListByIdsAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> RunContextQueryAsync(ContextQuery<T> ctx)
        {
            throw new NotImplementedException();
        }

        public Task SoftDeleteAsync(T data)
        {
            throw new NotImplementedException();
        }

        public Task SoftDeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T data)
        {
            throw new NotImplementedException();
        }
    }
}
