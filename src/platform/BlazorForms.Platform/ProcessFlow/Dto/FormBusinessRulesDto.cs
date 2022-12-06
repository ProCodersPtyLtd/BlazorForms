using BlazorForms.Flows.Definitions;
using BlazorForms.Platform;

namespace BlazorForms.Platform.ProcessFlow.Dto
{
    public class FormBusinessRulesDto
    {
        public FieldDisplayDetails[] DisplayProperties { get; set; }
        public IFlowModel Model { get; set; }
    }
}
