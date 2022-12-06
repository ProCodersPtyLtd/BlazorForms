using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.ItemStore
{
    public interface IItemStoreDataProvider
    {
        DynamicRecordset GetItem(StoreSchema schema, string tableName, long id);
        void SaveItem(StoreSchema schema, string tableName, DynamicRecordset targetRecordset, long id);
    }

    public class ItemStoreDataProvider : IItemStoreDataProvider
    {
        private readonly IStoreDatabaseDriver _storeDatabaseDriver;

        public ItemStoreDataProvider(IStoreDatabaseDriver storeDatabaseDriver)
        {
            _storeDatabaseDriver = storeDatabaseDriver;
        }

        public DynamicRecordset GetItem(StoreSchema schema, string tableName, long id)
        {
            // ToDo: move that to a proper place, to run once when Schema created or changed
            _storeDatabaseDriver.CreateDatabaseObjects(schema).Wait();
            // convert item to recordset
            var item = _storeDatabaseDriver.GetItemById(schema, tableName, id).Result;
            var result = new DynamicRecordset(item.Values);
            // ToDo: populate result.Model
            return result;
        }

        public void SaveItem(StoreSchema schema, string tableName, DynamicRecordset targetRecordset, long id)
        {
            // ToDo: move that to a proper place, to run once when Schema created or changed
            _storeDatabaseDriver.CreateDatabaseObjects(schema).Wait();
            // convert recordset to item
            var item = new StoreObject { TableName = tableName, Values = targetRecordset.Data.ToDict() };
            var resultId = _storeDatabaseDriver.UpsertItem(schema, item, id).Result;
        }
    }
}
