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
        private readonly IStateFlowRunEngine _flowEngine;
        private readonly IFlowParser _parser;

        public FlowDiagramViewModel(IStateFlowRunEngine flowEngine, IFlowParser parser)
        {
            _flowEngine = flowEngine;
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
            var flow = await _flowEngine.GetStateDetails(ps);

            var graph = GenerateGraph(flow);
            var doc = new Diagram(graph);
            doc.Run();
            var svg = doc.ToString();
            return svg;
        }

        private Graph GenerateGraph(StateFlowTaskDetails flow)
        {
            var graph = new Graph();

            foreach (var n in flow.States)
            {
                graph.AddNode(new ComponentNode(n.State, technology: "State"));
            }

            foreach (var t in flow.Transitions)
            {
                graph.AddEdge(t.FromState, t.GetTrigger().Text, t.ToState);
            }

            return graph;
        }
    }
}
