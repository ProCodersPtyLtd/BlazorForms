using BlazorForms.ItemStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Tests.Framework.Core
{
    public class MockStoreDatabaseDriver : IStoreDatabaseDriver
    {
        public Task CreateDatabaseObjects(StoreSchema storeSchema)
        {
            throw new NotImplementedException();
        }

        public Task DeleteItem(StoreSchema storeSchema, StoreObject item)
        {
            throw new NotImplementedException();
        }

        public Task<StoreRecordset> ExecuteQuery(StoreSchema storeSchema, StoreQuery storeQuery, params QueryParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public Task<StoreObject> GetItemById(StoreSchema storeSchema, string tableName, long id)
        {
            throw new NotImplementedException();
        }

        public Task<long> UpsertItem(StoreSchema storeSchema, StoreObject item, long pkid = 0)
        {
            throw new NotImplementedException();
        }

        public Task ValidateDatabaseObjects(StoreSchema storeSchema)
        {
            throw new NotImplementedException();
        }

        public void ValidateQuery(StoreSchema storeSchema, StoreQuery storeQuery)
        {
            throw new NotImplementedException();
        }

        public Task ValidateSchema(StoreSchema storeSchema)
        {
            throw new NotImplementedException();
        }
    }
}
