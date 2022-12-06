using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using System.Dynamic;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class PersonModel : PersonContactDetails, IFlowModel
    {
        public ExpandoObject Bag => throw new NotImplementedException();

        public Dictionary<string, DynamicRecordset> Ext => throw new NotImplementedException();
    }
}
