using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorForms.Platform.Integration.Tests.TestApi;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform;
using BlazorForms.Rendering.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Rendering;

namespace BlazorForms.Platform.Integration.Tests
{
    [TestClass]
    public class RestFlowRunProviderTests : WebApiTestBase
    {
        private readonly RestFlowRunProvider _provider;

        public RestFlowRunProviderTests()
        {
            _provider = new RestFlowRunProvider(_client, _knownTypesBinder);
        }

        [TestMethod]
        public async Task FlowContextParametersWrapperTest()
        {
            var ps = new FlowParamsGeneric { ItemId = "33", ParentItemId = "227", AssignedUser = "user3", Operation = FlowReferenceOperation.Details };
            ps["d1"] = "dynamicParameter1";
            ps["d2"] = "dynamicParameter2";
            ps["v2"] = "500";
            var ctx = await _provider.ExecuteFlow("BlazorForms.Platform.Crm.Business.Artel.ArtelProjectDashboardFlow", null, ps, "blabla");

            var retParams = ctx.Params;
            Assert.IsNotNull(retParams);
            Assert.AreEqual("33", retParams.ItemId);
            Assert.AreEqual("dynamicParameter1", retParams["d1"]);
            Assert.AreEqual("dynamicParameter2", retParams["d2"]);
            Assert.AreEqual("500", retParams["v2"]);
        }

        [TestMethod]
        public async Task ExecuteFlowTest()
        {
            var ctx = await _provider.ExecuteFlow("BlazorForms.Platform.Crm.Business.Artel.ArtelProjectDashboardFlow", null, new FlowParamsGeneric { ItemId = "7" }, "blabla");
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.RefId);
            var id = ctx.RefId;

            ctx = await _provider.ExecuteFlow("BlazorForms.Platform.Crm.Business.Artel.ArtelProjectDashboardFlow", ctx.RefId, new FlowParamsGeneric { ItemId = "7" });
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.RefId);
            Assert.AreEqual(id, ctx.RefId);
        }

        [TestMethod]
        public async Task ExecuteFlowNoStorageTest()
        {
            var ctx = await _provider.ExecuteFlow("BlazorForms.Platform.Crm.Business.Artel.ArtelProjectDashboardFlow", null, new FlowParamsGeneric { ItemId = "7" }, "blabla", true);
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.RefId);
        }

        [TestMethod]
        public async Task GetLastModelTest()
        {
            var ctx = await _provider.ExecuteFlow("BlazorForms.Platform.Crm.Business.Artel.ArtelProjectDashboardFlow", null, new FlowParamsGeneric { ItemId = "7" }, "blabla");
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.RefId);
            var id = ctx.RefId;

            var model = await _provider.GetLastModel(id);
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public async Task GetFormDetailsTest()
        {
            var form = await _provider.GetFormDetails("BlazorForms.Platform.Crm.Business.Artel.FormArtelProjectDashboard");
            Assert.IsNotNull(form);
        }

        [TestMethod]
        public async Task ExecuteFormLoadRulesTest()
        {
            var ctx = await _provider.ExecuteFlow("BlazorForms.Platform.Crm.Business.Artel.ArtelProjectDashboardFlow", null, new FlowParamsGeneric { ItemId = "7" });
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.RefId);

            //var result = await _provider.ExecuteFormLoadRules(ctx.Model, ctx.ExecutionResult.FormId);
            var view = await _provider.GetCurrentFlowRunUserView(ctx.RefId, ctx);
            var form = await _provider.GetFormDetails(view.UserViewName);
            var allFields = form.Fields;
            var ruleRequest = FormViewModel.GetRuleRequest(form.ProcessTaskTypeFullName, null, null, 0, allFields, null);
            var result = await _provider.ExecuteFormLoadRules(ruleRequest, ctx.Model);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model as IFlowModel);
        }

        [TestMethod]
        public async Task GetActiveFlowsTest()
        {
            var flowType = "BlazorForms.Platform.Crm.Business.Artel.ArtelProjectDashboardFlow";
            var data = _provider.GetActiveFlowsIds(flowType);
            
            await foreach (var flows in data)
            {
                Assert.IsNotNull(flows);
            }
        }

        [TestMethod]
        public async Task GetListFlowUserViewTest()
        {
            var flowType = "BlazorForms.Platform.Crm.Business.Artel.ArtelProjectListFlow";
            var view = await _provider.GetListFlowUserView(flowType, new FlowParamsGeneric(), new QueryOptions { PageIndex = 0, PageSize = 5 });
            Assert.IsNotNull(view.GetModel());
            Assert.IsNotNull(view.RawDataList);
        }

        [TestMethod]
        public async Task GetListItemFlowUserViewTest()
        {
            var view = await _provider.GetListItemFlowUserView("BlazorForms.Platform.Crm.Business.Artel.ArtelProjectDashboardFlow", new FlowParamsGeneric { ItemId = "5" });
            Assert.IsNotNull(view);
            Assert.IsNotNull(view.GetModel());
            Assert.IsNull(view.RawDataList);
        }

        

        //[Fact(Skip = "ToDo: REST SubmitListItemForm is not implemented during RefId refactoring")]
        //public async Task SubmitListItemFormTest()
        //{
        //    var flow = "BlazorForms.Platform.Crm.Business.Artel.ArtelProjectDashboardFlow";
        //    var ctx = await _provider.ExecuteFlow(flow, null, new FlowParamsGeneric { ItemId = "7" });
        //    Assert.IsNotNull(ctx);
        //    Assert.IsNotNull(ctx.RefId);

        //    ctx = await _provider.SubmitListItemForm(ctx.RefId, ctx.Model, "Na-Na");
        //    Assert.IsNotNull(ctx);
        //    Assert.IsNotNull(ctx.ExecutionResult);
        //}

        [TestMethod]
        public async Task GetFlowDefaultReadonlyViewTest()
        {
            var ctx = await _provider.ExecuteFlow("BlazorForms.Platform.Crm.Business.Artel.ArtelProjectDashboardFlow", null, new FlowParamsGeneric { ItemId = "7" });
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.RefId);

            var view = await _provider.GetFlowDefaultReadonlyView(ctx.RefId);
            Assert.IsNotNull(view);
            Assert.IsNotNull(view.GetModel());
            Assert.IsNull(view.RawDataList);
        }

        [TestMethod]
        public async Task RejectFormTest()
        {
            var flow = "BlazorForms.Platform.Crm.Business.Artel.ArtelProjectSettingsFlow";
            var ctx = await _provider.ExecuteFlow(flow, null, new FlowParamsGeneric { ItemId = "7" });
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.RefId);
            Assert.IsTrue(ctx.ExecutionResult.IsFormTask);

            var form = await _provider.GetFormDetails(ctx.ExecutionResult.FormId);
            Assert.IsNotNull(form);

            var result = await _provider.RejectForm(ctx.RefId, ctx.Model, "SaveButton", "Save");
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public async Task SubmitFormTest()
        {
            var flow = "BlazorForms.Platform.Crm.Business.Artel.ArtelProjectSettingsFlow";
            var ctx = await _provider.ExecuteFlow(flow, null, new FlowParamsGeneric { ItemId = "7" });
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.RefId);
            Assert.IsTrue(ctx.ExecutionResult.IsFormTask);

            var form = await _provider.GetFormDetails(ctx.ExecutionResult.FormId);
            Assert.IsNotNull(form);

            var result = await _provider.SubmitForm(ctx.RefId, ctx.Model, "SaveButton", "Save");
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public async Task SaveFormTest()
        {
            var flow = "BlazorForms.Platform.Crm.Business.Artel.ArtelProjectSettingsFlow";
            var ctx = await _provider.ExecuteFlow(flow, null, new FlowParamsGeneric { ItemId = "7" });
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.RefId);
            Assert.IsTrue(ctx.ExecutionResult.IsFormTask);

            var form = await _provider.GetFormDetails(ctx.ExecutionResult.FormId);
            Assert.IsNotNull(form);

            await _provider.SaveForm(ctx.RefId, ctx.Model, "SaveButton", "Save");
        }

        [TestMethod]
        public async Task TriggerRuleTest()
        {
            var flow = "BlazorForms.Platform.Crm.Business.Artel.ArtelProjectSettingsFlow";
            var ctx = await _provider.ExecuteFlow(flow, null, new FlowParamsGeneric { ItemId = "7" });
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.RefId);
            Assert.IsTrue(ctx.ExecutionResult.IsFormTask);

            var form = await _provider.GetFormDetails(ctx.ExecutionResult.FormId);
            Assert.IsNotNull(form);

            var formDisplayProperties = form.Fields.Where(f => f.ControlType != "Table").Select(f => f?.DisplayProperties).Where(f => f != null).Select(d => new FieldDisplayDetails
            {
                Caption = d.Caption,
                Disabled = d.Disabled,
                Highlighted = d.Highlighted,
                Hint = d.Hint,
                Binding = d.Binding.CopyWithKey(),
                Name = d.Name,
                Required = d.Required,
                Visible = d.Visible
            }).ToArray();

            var binding = form.Fields.FirstOrDefault(f => f.Name == "ProjectBaseCurrencyControl")?.Binding;

            var execResult = await _provider.TriggerRule(ctx.ExecutionResult.FormId, ctx.Model, null, formDisplayProperties, null, "APS-CUR-1", binding, 0);
            Assert.IsNotNull(execResult);
            Assert.IsNotNull(execResult.Model);
            Assert.IsNotNull(execResult.Validations);
            Assert.IsNotNull(execResult.FieldsDisplayProperties);
            Assert.IsNull(execResult.AccessModel);
        }
    }
}
