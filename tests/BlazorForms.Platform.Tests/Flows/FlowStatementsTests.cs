using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BlazorForms.Tests.Framework.Core;

namespace BlazorForms.Platform.Tests.Flows
{
    public class FlowStatementsTests
    {
        private IFlowRunProvider _provider;

        public FlowStatementsTests()
        {
            _provider = new FlowRunProviderCreator().GetFlowRunProvider();
        }

        [Fact]
        public async Task FlowWaitTest()
        {
            // first execution
            var startCtx = new ClientKeptContext { FlowName = typeof(TestWaitFlow1).FullName };
            var ps = new FlowParamsGeneric { OperationName = "new" };
            var firstCtx = await _provider.ExecuteClientKeptContextFlow(startCtx, null, ps);
            var keptCtx = firstCtx.GetClientContext();
            var model = firstCtx.Model as TestSampleModel1;

            Assert.NotEmpty(keptCtx.RefId);
            Assert.True(keptCtx.ExecutionResult.IsWaitTask);
            Assert.Equal(TaskExecutionResultStateEnum.Success, keptCtx.ExecutionResult.ResultState);
            Assert.Equal(TaskExecutionFlowStateEnum.Stop, keptCtx.ExecutionResult.FlowState);
            Assert.Equal(1, keptCtx.CurrentTaskLine);
            Assert.Equal("Wait", keptCtx.CurrentTask);

            // continue execution
            model.Name = "Form1Changes";
            var secondCtx = await _provider.ExecuteClientKeptContextFlow(keptCtx, model, ps);
            keptCtx = secondCtx.GetClientContext();
            model = secondCtx.Model as TestSampleModel1;

            Assert.NotEmpty(keptCtx.RefId);
            Assert.True(keptCtx.ExecutionResult.IsWaitTask);
            Assert.Equal(TaskExecutionResultStateEnum.Success, keptCtx.ExecutionResult.ResultState);
            Assert.Equal(TaskExecutionFlowStateEnum.Stop, keptCtx.ExecutionResult.FlowState);
            Assert.Equal(1, keptCtx.CurrentTaskLine);

            // allow continue execution
            TestWaitFlow1.ContinueWaiting = false;
            var thirdCtx = await _provider.ExecuteClientKeptContextFlow(keptCtx, model, ps);
            keptCtx = thirdCtx.GetClientContext();
            model = thirdCtx.Model as TestSampleModel1;

            Assert.Equal("FlowEnd", model.Name);
            Assert.Equal(TaskExecutionResultStateEnum.Success, keptCtx.ExecutionResult.ResultState);
            Assert.Equal(TaskExecutionFlowStateEnum.Finished, keptCtx.ExecutionResult.FlowState);
            Assert.False(keptCtx.ExecutionResult.IsFormTask);
            Assert.False(keptCtx.ExecutionResult.IsWaitTask);
        }
    }

    public class TestWaitFlow1 : FluentFlowBase<TestSampleModel1>
    {
        public static bool ContinueWaiting = true;

        public override void Define()
        {
            this
                .Begin(() => FlowStart())
                .Wait(() => TestWaitFlow1.ContinueWaiting)
                .End(() => FlowEnd());
        }

        public async Task FlowStart()
        {
        }

        public async Task FlowEnd()
        {
            Model.Name = "FlowEnd";
        }
    }
}
