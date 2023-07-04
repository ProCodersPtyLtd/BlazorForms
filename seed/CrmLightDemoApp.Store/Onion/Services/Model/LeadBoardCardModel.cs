using BlazorForms.Flows.Definitions;
using BlazorForms.Rendering.Model;
using BlazorForms.Storage;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class LeadBoardCardModel : IEntity, IFlowBoardCard
    {
        //BoardCard
        public int Id { get; set; }
        public bool Deleted { get; set; }

        //public virtual int BoardId { get; set; }
        public string State { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }

        public int? LeadSourceTypeId { get; set; }
        public string? ContactDetails { get; set; }
        //public virtual string? Comments { get; set; }
        public int? RelatedCompanyId { get; set; }
        public int? RelatedPersonId { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? SalesPersonId { get; set; }

        public string? FollowUpDetails { get; set; }
        public DateTime? FollowUpDate { get; set; }

        public int? ClientCompanyId { get; set; }

        // Model
        public string? Comments { get; set; }

        // for dropdowns
        public List<PersonModel> AllPersons { get; set; } = new();
        public List<CompanyModel> AllCompanies { get; set; } = new();
        public List<LeadSourceTypeModel> AllLeadSources { get; set; } = new();

        // for ClientCompany
        public bool IsNewCompany { get; set; }
        public CompanyModel Company { get; set; } = new();
        public ClientCompanyModel ClientCompany { get; set; } = new();

        public List<CardHistoryModel>? CardHistory { get; set; } = new();

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
