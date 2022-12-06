using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows
{
    public interface IFluentFlow : IFlow
    {
        //void Define();
        List<TaskDef> Parse();
        //IFlowModel GetModel();
        //void SetModel(IFlowModel model);
        //void SetParams(FlowParamsGeneric p);
        //IFlowContext CreateContext();
        List<TaskDef> Tasks { get; }
        void SetFlowRefId(string refId);
        void SetFirstPass(bool firstPass);
        string CreateRefId();
        void SetFlowContext(IFlowContext flowContext);
        void UpdateCurrentContext(string statusMessage, string assignedUser, string adminUser, string assignedTeam);
    }

    public interface IFluentFlow<M> : IFluentFlow where M : IFlowModel
    {
        M Model { get; set; }
        FlowParamsGeneric Params { get; set; }
    }
}
