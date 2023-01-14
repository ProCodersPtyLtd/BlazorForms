using BlazorForms.Rendering.Model;
using CrmLightDemoApp.Onion.Domain;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class LeadBoardCardModel : BoardCard, IFlowBoardCard
    {
        // for dropdowns
        public virtual List<PersonModel> AllPersons { get; set; } = new();
        public virtual List<Company> AllCompanies { get; set; } = new();
        public virtual List<LeadSourceType> AllLeadSources { get; set; } = new();

        // for ClientCompany
        public virtual bool IsNewCompany { get; set; }
        public virtual Company Company { get; set; } = new();
        public virtual ClientCompany ClientCompany { get; set; } = new();

        // properties
        public string SalesPersonFullName
        {
            get
            {
                var sp = AllPersons.FirstOrDefault(p => p.Id == SalesPersonId);

                if (sp != null)
                {
                    return sp.FullName;
                }

                return null;
            }
        }
    }
}
