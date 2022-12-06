using BlazorForms.ItemStore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.ItemStore
{
    public interface IStoreDatabaseDriver
    {
        // Schema
        Task ValidateSchema(StoreSchema storeSchema);
        Task CreateDatabaseObjects(StoreSchema storeSchema);
        Task ValidateDatabaseObjects(StoreSchema storeSchema);

        // Query
        void ValidateQuery(StoreSchema storeSchema, StoreQuery storeQuery);
        // too SQL dependent!
        //string GenerateQuerySql(StoreQuery storeQuery);
        Task<StoreRecordset> ExecuteQuery(StoreSchema storeSchema, StoreQuery storeQuery, params QueryParameter[] parameters);
        Task<StoreObject> GetItemById(StoreSchema storeSchema, string tableName, long id);
        Task<long> UpsertItem(StoreSchema storeSchema, StoreObject item, long pkid = 0);
        Task DeleteItem(StoreSchema storeSchema, StoreObject item);
    }
}
