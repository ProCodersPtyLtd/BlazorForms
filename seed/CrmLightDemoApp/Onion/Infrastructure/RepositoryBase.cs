using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain.Entities;
using CrmLightDemoApp.Onion.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    public class RepositoryBase<T> : LocalCacheRepository<T>
        where T : class, IEntity
    {    
    }
}
