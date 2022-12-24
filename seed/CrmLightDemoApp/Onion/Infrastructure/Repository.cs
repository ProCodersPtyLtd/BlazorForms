using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Repositories;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    // this is repository emulator that stores all data in memory
    // it stores and retrieves object copies, like a real database
    public class Repository<T> : IRepository<T>
        where T : class, IEntity
    {
        private int _id = 0;
        private readonly List<T> _localCache = new List<T>();

        public async Task<int> CreateAsync(T data)
        {
            _id++;
            data.Id = _id;
            _localCache.Add(data.GetCopy());
            return _id;
        }

        public async Task DeleteAsync(int id)
        {
            _localCache.Remove(_localCache.Single(x => x.Id == id));
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return _localCache.Single(x => x.Id == id).GetCopy();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return _localCache.Select(x => x.GetCopy()).ToList();
        }

        public async Task UpdateAsync(T data)
        {
            await DeleteAsync(data.Id);
            _localCache.Add(data.GetCopy());
        }
    }
}
