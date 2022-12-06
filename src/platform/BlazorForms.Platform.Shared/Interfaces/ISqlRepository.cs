using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Shared.Interfaces
{
    public interface ISqlRepository
    {
    }

    public interface ISqlRepository<T> : ISqlRepository
    {
        T GetContext();
    }

    public interface IReferenceRepository<T, T1>: ISqlRepository<T>
    {        
        IQueryable<T1> Get(T db, int id);
        IQueryable<T1> GetAll(T db);
        Task<int> Insert(T db, T1 data);
        Task Delete(T db, int id);
        Task Update(T db, T1 data);
    }
}
