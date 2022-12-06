using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorForms.Platform.Integration.Tests.TestApi;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Integration.Tests.Server.Flows;
using BlazorForms.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Platform;

namespace BlazorForms.Platform.Integration.Tests
{
    [TestClass]
    public class SilentFlowApiTests : WebApiTestBase
    {
        private readonly RestFlowRunProvider _provider;

        public SilentFlowApiTests()
        {
            _provider = new RestFlowRunProvider(_client, _knownTypesBinder);
        }

        [TestMethod]
        public async Task SimpleClientKeptContextTest()
        {
            // first execution
            var startCtx = new ClientKeptContext { FlowName = typeof(TestSampleFlow1).FullName };
            var ps = new FlowParamsGeneric { OperationName = "new" };
            var firstCtx = await _provider.ExecuteClientKeptContextFlow(startCtx, null, ps);
            var keptCtx = firstCtx.GetClientContext();
            var model = firstCtx.Model as TestSampleModel1;

            Assert.IsFalse(string.IsNullOrEmpty(keptCtx.RefId));
            Assert.AreEqual(typeof(TestSampleForm1).FullName, keptCtx.ExecutionResult.FormId);
            Assert.AreEqual(TaskExecutionResultStateEnum.Success, keptCtx.ExecutionResult.ResultState);
            Assert.IsTrue(keptCtx.ExecutionResult.IsFormTask);
            Assert.AreEqual(2, keptCtx.CurrentTaskLine);
            Assert.AreEqual("new", keptCtx.Params.OperationName);
            Assert.AreEqual("DeleteMe", model.Name);

            // submit form
            model.Name = "Form1Changes";
            var secondCtx = await _provider.SubmitClientKeptContextFlowForm(keptCtx, model, ps, null);
            keptCtx = secondCtx.GetClientContext();
            model = secondCtx.Model as TestSampleModel1;

            Assert.IsFalse(string.IsNullOrEmpty(keptCtx.RefId));
            Assert.AreEqual(typeof(TestSampleForm2).FullName, keptCtx.ExecutionResult.FormId);
            Assert.AreEqual(TaskExecutionResultStateEnum.Success, keptCtx.ExecutionResult.ResultState);
            Assert.IsTrue(keptCtx.ExecutionResult.IsFormTask);
            Assert.AreEqual(4, keptCtx.CurrentTaskLine);
            Assert.AreEqual("new", keptCtx.Params.OperationName);
            Assert.AreEqual("SecondForm", model.Name);

            // submit form2
            var thirdCtx = await _provider.SubmitClientKeptContextFlowForm(keptCtx, model, ps, "$.Buttons.Reject");
            keptCtx = thirdCtx.GetClientContext();
            model = thirdCtx.Model as TestSampleModel1;

            Assert.AreEqual("FlowEnd", model.Name);
            Assert.AreEqual(TaskExecutionResultStateEnum.Success, keptCtx.ExecutionResult.ResultState);
            Assert.AreEqual(TaskExecutionFlowStateEnum.Finished, keptCtx.ExecutionResult.FlowState);
            Assert.AreEqual("$.Buttons.Reject", keptCtx.ExecutionResult.FormLastAction);
            Assert.IsFalse(keptCtx.ExecutionResult.IsFormTask);
        }
    }
    
}
