using Newtonsoft.Json;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorForms.Platform
{
    public interface ICustomConfigProvider
    {
        void DecorateForm(FormDetails formDetails);
        void LoadCustomData(FlowEventArgs e);
        void SaveCustomData(FlowEventArgs e);
    }

    public class CustomConfigProvider : ICustomConfigProvider
    {
        private readonly ICustomConfigStore _customConfigStore;
        private readonly ICustomModelDataProvider _customModelDataProvider;
        private readonly IJsonPathNavigator _navigator;

        public CustomConfigProvider(ICustomConfigStore customConfigStore, ICustomModelDataProvider customModelDataProvider, IJsonPathNavigator navigator)
        {
            _customConfigStore = customConfigStore;
            _customModelDataProvider = customModelDataProvider;
            _navigator = navigator;
        }

        public void DecorateForm(FormDetails formDetails)
        {
            // Used to create test json data only
            //var cfgt = new CustomFormConfig() { FullName = "Platform.Crm.Business.Clients.ProjectListEditFlow" };
            //cfgt.Fields.Add(new CustomFormField { Field = formDetails.Fields[1] });
            //cfgt.Dependencies.Add(new CustomConfigDependency { TypeCode = CustomConfigDependencyCodes.ModelExtension, Name = "QueryProjects", ItemStoreCode = "table_project" });
            //var json = JsonConvert.SerializeObject(cfgt);

            var cfg = _customConfigStore.GetConfigItem<CustomFormConfig>(ConfigItemTypeCodes.FormConfig, formDetails.ProcessTaskTypeFullName);

            if(cfg != null)
            {
                var adds = cfg.Fields.Where(f => !f.IsOverride);
                formDetails.Fields.AddRange(adds.Select(a => a.Field));

                var overrides = cfg.Fields.Where(f => f.IsOverride);

                foreach(var ov in overrides)
                {
                    var field = formDetails.Fields.FirstOrDefault(f => f.Name == ov.Field.Name);

                    if(field != null)
                    {
                        // only these property overridings supported
                        field.Order = ov.Field.Order;
                        field.DisplayProperties.Required = ov.Field.DisplayProperties.Required;
                        field.DisplayProperties.Disabled = ov.Field.DisplayProperties.Disabled;
                        field.DisplayProperties.Visible = ov.Field.DisplayProperties.Visible;
                        field.DisplayProperties.Caption = ov.Field.DisplayProperties.Caption;
                    }
                }
                
                formDetails.Fields = formDetails.Fields.OrderBy(f => f.Order).ToList();
            }
        }

        public void LoadCustomData(FlowEventArgs e)
        {
            // Used to create test json data only
            //var cfgt = new CustomModelConfig() { FlowName = "Platform.Crm.Business.Clients.ProjectListEditFlow", TaskName = "PopulateDetailsAsync" };
            //cfgt.Models.Add(new CustomModel { Name = "ExtraProject", PkJsonPath = "$.Project.Id", Query = new StoreQueryReference { TableName = "ExtraProjectTable" } });
            //var json = JsonConvert.SerializeObject(cfgt);

            var cfg = _customConfigStore.GetConfigItem<CustomModelConfig>(ConfigItemTypeCodes.ModelConfig, e.Context.FlowName, e.TaskName);

            if(cfg != null)
            {
                foreach(var model in cfg.Models)
                {
                    var id = Convert.ToInt64(_navigator.GetValue(e.Model, model.PkJsonPath));
                    var recordset = _customModelDataProvider.ExecuteQuery(model.Query, id);
                    e.Model.Ext[model.Name] = recordset;
                }
            }
        }

        public void SaveCustomData(FlowEventArgs e)
        {
            var cfg = _customConfigStore.GetConfigItem<CustomModelConfig>(ConfigItemTypeCodes.ModelConfig, e.Context.FlowName, e.TaskName);

            if (cfg != null)
            {
                foreach (var model in cfg.Models)
                {
                    var id = Convert.ToInt64(_navigator.GetValue(e.Model, model.PkJsonPath));
                    var mapping = model.Mapping;
                    var storeReference = model.Query;
                    _customModelDataProvider.SaveRecordset(e.Model.Ext[model.Name], id, storeReference, mapping);
                }
            }
        }
    }
}
