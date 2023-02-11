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
    public class DefinitionFluentFlowTests
    {
        private IFlowRunProvider _provider;
        private IFluentFlowRunEngine _fluentFlowEngine;

        public DefinitionFluentFlowTests()
        {
            var creator = new FlowRunProviderCreator();
            _provider = creator.GetFlowRunProvider();
            _fluentFlowEngine = creator.FluentFlowRunEngine;
        }

        [Fact]
        public async Task SimpleClientKeptContextTest()
        {
            var ps = new FlowRunParameters
            {
                FlowType = typeof(TestDefinitionSampleFlow1),
                NoStorageMode = true
            };

            var data = await _fluentFlowEngine.GetFlowDefinitionDetails(ps);
        }
    }

    public class TestDefinitionSampleFlow1 : FluentFlowBase<TestSampleModel1>
    {
        public override void Define()
        {
            this
                .Begin(() => FlowStart())
                .Next(() => LoadData())
                .NextForm(typeof(TestSampleForm1))
                .If(() => 1 == 1)
                    .Label("label1")
                    .Next(() => { var someStatement = "1"; })
                .Else()
                    .Next(() => { var someOtherStatement = "2"; })
                    //.Goto("label1")
                .EndIf()
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
        }

        public async Task RefreshFormData()
        {
        }
        public async Task SaveData()
        {
        }

        public async Task FlowEnd()
        {
        }
    }
}
