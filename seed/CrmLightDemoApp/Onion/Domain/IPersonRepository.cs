﻿namespace CrmLightDemoApp.Onion.Domain
{
    public interface IPersonRepository
    {
        Task<List<Person>> GetAllAsync();
        Task<Person> GetByIdAsync(int id);
        Task<int> CreateAsync(Person data);
        Task UpdateAsync(Person data);
        Task DeleteAsync(int id);
    }
}