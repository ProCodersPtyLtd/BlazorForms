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
    public class SilentFlowTests
    {
        private IFlowRunProvider _provider;

        public SilentFlowTests()
        {
            _provider = new FlowRunProviderCreator().GetFlowRunProvider();
        }

        [Fact]
        public async Task SimpleClientKeptContextTest()
        {
            // first execution
            var startCtx = new ClientKeptContext { FlowName = typeof(TestSampleFlow1).FullName };
            var ps = new FlowParamsGeneric { OperationName = "new" };
            var firstCtx = await _provider.ExecuteClientKeptContextFlow(startCtx, null, ps);
            var keptCtx = firstCtx.GetClientContext();
            var model = firstCtx.Model as TestSampleModel1;

            Assert.NotEmpty(keptCtx.RefId);
            Assert.Equal(typeof(TestSampleForm1).FullName, keptCtx.ExecutionResult.FormId);
            Assert.Equal(TaskExecutionResultStateEnum.Success, keptCtx.ExecutionResult.ResultState);
            Assert.True(keptCtx.ExecutionResult.IsFormTask);
            Assert.Equal(2, keptCtx.CurrentTaskLine);
            Assert.Equal("new", keptCtx.Params.OperationName);
            Assert.Equal("DeleteMe", model.Name);

            // submit form
            model.Name = "Form1Changes";
            var secondCtx = await _provider.SubmitClientKeptContextFlowForm(keptCtx, model, ps, null);
            keptCtx = secondCtx.GetClientContext();
            model = secondCtx.Model as TestSampleModel1;

            Assert.NotEmpty(keptCtx.RefId);
            Assert.Equal(typeof(TestSampleForm2).FullName, keptCtx.ExecutionResult.FormId);
            Assert.Equal(TaskExecutionResultStateEnum.Success, keptCtx.ExecutionResult.ResultState);
            Assert.True(keptCtx.ExecutionResult.IsFormTask);
            Assert.Equal(4, keptCtx.CurrentTaskLine);
            Assert.Equal("new", keptCtx.Params.OperationName);
            Assert.Equal("SecondForm", model.Name);

            // submit form2
            var thirdCtx = await _provider.SubmitClientKeptContextFlowForm(keptCtx, model, ps, null);
            keptCtx = thirdCtx.GetClientContext();
            model = thirdCtx.Model as TestSampleModel1;

            Assert.Equal("FlowEnd", model.Name);
            Assert.Equal(TaskExecutionResultStateEnum.Success, keptCtx.ExecutionResult.ResultState);
            Assert.Equal(TaskExecutionFlowStateEnum.Finished, keptCtx.ExecutionResult.FlowState);
            Assert.False(keptCtx.ExecutionResult.IsFormTask);
        }
    }

    public class TestSampleFlow1 : FluentFlowBase<TestSampleModel1>
    {
        public override void Define()
        {
            this
                .Begin(() => FlowStart())
                .Next(() => LoadData())
                .NextForm(typeof(TestSampleForm1))
                .Next(() => RefreshFormData())
                .NextForm(typeof(TestSampleForm2))
                .Next(() => SaveData())
                .End(() => FlowEnd());
        }

        public async Task FlowStart()
        {
        }

        public async Task LoadData()
        {
            Model.Name = "DeleteMe";
            Model.Amount = new Money(100m, "Tanga");
        }

        public async Task RefreshFormData()
        {
            Model.Name = "SecondForm";
        }
        public async Task SaveData()
        {
        }

        public async Task FlowEnd()
        {
            Model.Name = "FlowEnd";
        }
    }

    public class TestSampleModel1 : FlowModelBase
    {
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Money Amount { get; set; }
    }

    public class TestSampleForm1 : FormEditBase<TestSampleModel1>
    {
        protected override void Define(FormEntityTypeBuilder<TestSampleModel1> f)
        {
            f.Property(p => p.Id).IsPrimaryKey();
            f.Property(p => p.Name).IsRequired();
            f.Property(e => e.Amount).Control(typeof(MoneyEdit));

            f.Button("/", ButtonActionTypes.Submit);
            f.Button("/", ButtonActionTypes.Cancel, "Cancel Me");
        }
    }

    public class TestSampleForm2 : FormEditBase<TestSampleModel1>
    {
        protected override void Define(FormEntityTypeBuilder<TestSampleModel1> f)
        {
            f.Property(p => p.Id).IsReadOnly();
            f.Property(p => p.Name).IsRequired().IsReadOnly();
            f.Property(e => e.Amount).Control(typeof(MoneyEdit));

            f.Button("/", ButtonActionTypes.Submit);
            f.Button("/", ButtonActionTypes.Cancel, "Cancel Me");
        }
    }
}
