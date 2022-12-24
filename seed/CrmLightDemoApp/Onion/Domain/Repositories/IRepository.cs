namespace CrmLightDemoApp.Onion.Domain.Repositories
{
    public interface IRepository<T>
        where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<int> CreateAsync(T data);
        Task UpdateAsync(T data);
        Task DeleteAsync(int id);
    }
}
