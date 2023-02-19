using Microsoft.Msagl.Drawing;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine.StateFlow;
using SvgLayerSample.Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Flows
{
    public class FlowDiagramViewModel
    {
        private readonly IStateFlowRunEngine _stateFlowEngine;
        private readonly IFluentFlowRunEngine _fluentFlowEngine;
        private readonly IFlowParser _parser;

        public FlowDiagramViewModel(IFluentFlowRunEngine fluentFlowEngine, IStateFlowRunEngine flowEngine, IFlowParser parser)
        {
            _stateFlowEngine = flowEngine;
            _fluentFlowEngine = fluentFlowEngine;
            _parser = parser;
        }

        public async Task<string> GetFlowDiagramSvg(string flowId, Type flowType)
        {
            if (flowId == null && flowType == null)
            {
                throw new Exception($"This method accepts at least one not null parameter flowId or flowType");
            }

            var type = flowType ?? _parser.GetTypeByName(flowId);
            var ps = new FlowRunParameters { FlowType = type, NoStorageMode = true };
            FlowDefinitionDetails flow;

            if (_parser.GetTypesInheritedFrom(typeof(IStateFlow)).Any(f => f == flowType))
            {

                flow = await _stateFlowEngine.GetFlowDefinitionDetails(ps);
            }
            else
            {
                flow = await _fluentFlowEngine.GetFlowDefinitionDetails(ps);
            }

            var graph = GenerateGraph(flow);
            var doc = new Diagram(graph);
            doc.Run();
            var svg = doc.ToString();
            return svg;
        }

        private Graph GenerateGraph(FlowDefinitionDetails flow)
        {
            var graph = new Graph();
            var states = flow.States.ToDictionary(x => x.State, x => x.Caption ?? x.State);

            foreach (var n in flow.States)
            {
                graph.AddNode(new ComponentNode(states[n.State], technology: n.Type));
            }

            foreach (var t in flow.Transitions)
            {
                //graph.AddEdge(t.FromState, t.GetTrigger().Text, t.ToState);
                graph.AddEdge(states[t.FromState], null, states[t.ToState]);
            }

            return graph;
        }
    }
}
