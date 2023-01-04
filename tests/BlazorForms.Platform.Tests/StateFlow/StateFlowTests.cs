using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine.StateFlow;
using BlazorForms.Tests.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlazorForms.Platform.Tests.StateFlow
{
    public class StateFlowTests
    {
        [Fact]
        public void StateFlowObjectPopulatedTest()
        {
            var flow = new SampleStateFlow1();
            Assert.Equal("Unassigned", flow.Unassigned.Value);
            Assert.Equal("Assigned", flow.Assigned.Value);
            Assert.Equal("Reviewed", flow.Reviewed.Value);
            Assert.Equal("NA", flow.NA.Value);
            Assert.Equal("Open", flow.Open.Value);
            Assert.Equal("StatusClosed", flow.StatusClosed.Value);

            Assert.Equal("ReviewedWithLabel", flow.ReviewedWithLabel.Value);
            Assert.Equal("Reviewed With Label", flow.ReviewedWithLabel.Caption);
        }

        [Fact]
        public void StateFlowParseTest()
        {
            var flow = new TestStateFlow1();
            flow.Parse();
            Assert.Equal(4, flow.States.Count);
            Assert.Equal(5, flow.Transitions.Count);
            var t = flow.Transitions[0];
            Assert.Equal("Unassigned", t.FromState);
            Assert.Equal("Assigned", t.ToState);
        }

        [Fact]
        public async Task StateFlowExecuteTest()
        {
            var creator = new FlowRunProviderCreator();
            creator.GetFlowRunProvider();
            var logger = creator.ServiceProvider.GetRequiredService<ILogger<StateFlowRunEngine>>();
            var engine = new StateFlowRunEngine(logger, creator.ServiceProvider, creator.FlowRunStorage, creator.FlowParser);
            var ps = new FlowRunParameters { FlowType = typeof(TestStateFlow1), NoStorageMode = true };

            var context = await engine.ExecuteFlow(ps);
            Assert.Equal("Unassigned", context.GetState());
            Assert.True(DateTime.Now - context.ExecutionResult.CreatedDate < new TimeSpan(0, 0, 1));

            context = await engine.ContinueFlowNoStorage(context, "Assign");
            Assert.Equal("Assigned", context.GetState());

            context = await engine.ContinueFlowNoStorage(context, "Recover");
            Assert.Equal("BeingReviewed", context.GetState());

            context = await engine.ContinueFlowNoStorage(context, "Close");
            Assert.Equal("Closed", context.GetState());
            Assert.Equal(TaskExecutionFlowStateEnum.Finished, context.ExecutionResult.FlowState);
            Assert.True(DateTime.Now - context.ExecutionResult.FinishedDate < new TimeSpan(0, 0, 1));
        }
    }

    public class SampleStateFlow1 : StateFlowBase<SampleStateFlowModel1>
    {
        // StateFlowBase constructor will populate state.Value = prop.Name using reflection
        public state Unassigned;
        public state Assigned;
        public state Reviewed;
        public state ReviewedWithLabel = new state("Reviewed With Label");

        public status NA;
        public status Open;
        public status StatusClosed;

        public override void Define()
        {
            this
                .State(Unassigned)
                    .Transition(AssignTrigger, Assigned)
                .State(Assigned)
                .State(Reviewed)
                ;
        }

        private TransitionTrigger AssignTrigger()
        {
            return new ButtonTransitionTrigger("Assign");
        }
    }

    public class SampleStateFlowModel1 : FlowModelBase
    {

    }
}
