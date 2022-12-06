using BlazorForms.ItemStore;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BlazorForms.Platform
{
    public interface ICustomModelDataProvider
    {
        DynamicRecordset ExecuteQuery(StoreQueryReference query);
        DynamicRecordset ExecuteQuery(StoreQueryReference query, long id);
        void SaveRecordset(DynamicRecordset dynamicRecordset, long id, StoreQueryReference storeReference, ModelObjectMapping mapping);
        DynamicRecordset MapRecordset(DynamicRecordset sourceRecordset, StoreQueryReference storeReference, ModelObjectMapping mapping);
    }

    public class CustomModelDataProvider : ICustomModelDataProvider
    {
        private readonly IItemStoreDataProvider _storeDataProvider;
        private readonly ICustomConfigStore _customConfigStore;

        public CustomModelDataProvider(IItemStoreDataProvider storeDataProvider, ICustomConfigStore customConfigStore)
        {
            _storeDataProvider = storeDataProvider;
            _customConfigStore = customConfigStore;
        }

        public DynamicRecordset ExecuteQuery(StoreQueryReference query)
        {
            return new DynamicRecordset { };
        }
        public DynamicRecordset ExecuteQuery(StoreQueryReference query, long id)
        {
            var storeCfg = _customConfigStore.GetConfigItem<CustomItemStoreConfig>(ConfigItemTypeCodes.ItemStoreConfig, query.SchemaName);

            if (!string.IsNullOrWhiteSpace(query.TableName))
            {
                var result = _storeDataProvider.GetItem(storeCfg.Schema, query.TableName, id);
                return result;
            }
            else
            {
                throw new NotImplementedException("queries not supported yet");
            }

            //var result = new DynamicRecordset { Data = new ExpandoObject { } };
            //result.Data.SetValue("InventoryCode", "777333");
            //return result;
        }

        public void SaveRecordset(DynamicRecordset dynamicRecordset, long id, StoreQueryReference storeReference, ModelObjectMapping mapping)
        {
            DynamicRecordset targetRecordset = dynamicRecordset;
            
            if (mapping != null)
            {
                targetRecordset = MapRecordset(dynamicRecordset, storeReference, mapping);
            }

            var storeCfg = _customConfigStore.GetConfigItem<CustomItemStoreConfig>(ConfigItemTypeCodes.ItemStoreConfig, storeReference.SchemaName);
            _storeDataProvider.SaveItem(storeCfg.Schema, mapping?.LeftModelName ?? storeReference.TableName, targetRecordset, id);
        }

        public DynamicRecordset MapRecordset(DynamicRecordset sourceRecordset, StoreQueryReference storeReference, ModelObjectMapping mapping)
        {
            var result = new DynamicRecordset { Data = new ExpandoObject(), Model = new ModelObject { Name = mapping.LeftModelName } };

            if (sourceRecordset.Empty)
            {
                // do nothing
            }
            if(sourceRecordset.SingleValue)
            {
                foreach(var leftField in mapping.FieldMappings.Keys)
                {
                    var rightField = mapping.FieldMappings[leftField];
                    var value = sourceRecordset.Data.GetValue(rightField);
                    result.Data.SetValue(leftField, value);

                    //populate Model definition based on source recordset data types
                    // ToDo: target ModelObject should be retrieved from target Table definition
                    var modelField = new ModelField { Name = leftField, Type = sourceRecordset.Model.Fields[rightField].Type };
                    result.Model.Fields[leftField] = modelField;
                }
            }
            else
            {
                // map all rows
                throw new NotImplementedException("Rows not mapped yet");
            }

            return result;
        }
    }
}
