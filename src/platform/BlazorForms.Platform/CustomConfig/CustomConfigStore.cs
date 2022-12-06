using Newtonsoft.Json;
using BlazorForms.ItemStore;
using BlazorForms.Shared;
using BlazorForms.Platform.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BlazorForms.Platform
{
    public interface ICustomConfigStore
    {
        CustomConfigItem GetConfigItem(string typeCode, string name, string childName = null);
        T GetConfigItem<T>(string typeCode, string name, string childName = null) where T : class;
    }

    public class CustomConfigStore : ICustomConfigStore
    {
        private readonly IAuthState _authState;
        private ILogStreamer _logStreamer;
        public CustomConfigStore(IAuthState authState, ILogStreamer logStreamer)
        {
            _authState = authState;
            _logStreamer = logStreamer;
        }

        public CustomConfigItem GetConfigItem(string typeCode, string name, string childName = null)
        {
            if(_authState.UserLogin().Result == "owner@nocode.com.au")
            {
                if(typeCode == ConfigItemTypeCodes.FormConfig && name == "Platform.Crm.Business.Clients.ProjectListEditFlow")
                {
                    var formExt1 = EmbeddedResourceHelper.GetApiRequestFile("Platform.CustomConfig.ProjectCustomConfig.json", typeof(CustomConfigItem).GetTypeInfo().Assembly, _logStreamer);
                    return new CustomConfigItem { JsonData = formExt1 };
                }

                if (typeCode == ConfigItemTypeCodes.ModelConfig && name == "Platform.Crm.Business.Clients.ProjectListEditFlow" && 
                    (childName == "PopulateDetailsAsync" || childName == "SaveAsync"))
                {
                    var ext1 = EmbeddedResourceHelper.GetApiRequestFile("Platform.CustomConfig.CustomModelConfig1.json", typeof(CustomConfigItem).GetTypeInfo().Assembly, _logStreamer);
                    return new CustomConfigItem { JsonData = ext1 };
                }

                if (typeCode == ConfigItemTypeCodes.ItemStoreConfig && name == "_pc1")
                {
                    var cfg = new CustomItemStoreConfig { Schema = GetSchema1() };
                    var json = JsonConvert.SerializeObject(cfg);
                    return new CustomConfigItem { JsonData = json };
                }
            }

            return null;
        }

        public T GetConfigItem<T>(string typeCode, string name, string childName = null) where T: class
        {
            var item = GetConfigItem(typeCode, name, childName);
            return item == null ? null: JsonConvert.DeserializeObject<T>(item.JsonData);
        }

        private StoreSchema GetSchema1()
        {
            var schema = new StoreSchema { Name = "_pc1", Definitions = new Dictionary<string, StoreDefinition>() };

            var projectTable = new StoreDefinition { Name = "ExtraProjectTable", Properties = new Dictionary<string, StoreProperty>() };
            schema.Definitions["ExtraProjectTable"] = projectTable;
            projectTable.Properties["project_id"] = new StoreProperty { Name = "project_id", Type = "int", Pk = true, ExternalId = true };
            projectTable.Properties["InventoryCode"] = new StoreProperty { Name = "InventoryCode", Type = "string" };

            return schema;
        }
    }

    public class CustomConfigItem
    {
        public long Id { get; set; }
        public string JsonData { get; set; }
        public string Name { get; set; }
        public string ChildName { get; set; }
        public ConfigItemTypeCode TypeCode { get; set; }
    }

    public class ConfigItemTypeCode
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public static class ConfigItemTypeCodes
    {
        public const string FormConfig = "FormConfig";
        public const string ModelConfig = "ModelConfig";
        public const string ItemStoreConfig = "StoreConfig";
    }
}
