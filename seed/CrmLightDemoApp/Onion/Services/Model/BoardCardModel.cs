using BlazorForms.Rendering.Model;
using CrmLightDemoApp.Onion.Domain;

namespace CrmLightDemoApp.Onion.Services.Model
{
	public class BoardCardModel : BoardCard, IFlowBoardCard
	{
        // for dropdowns
        public virtual List<PersonModel> AllPersons { get; set; } = new();
        public virtual List<LeadSourceType> AllLeadSources { get; set; } = new();
    }
}
