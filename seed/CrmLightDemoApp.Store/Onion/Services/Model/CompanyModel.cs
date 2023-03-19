using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Storage;
using System.Dynamic;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class CompanyModel : IEntity, IFlowModel
    {
        // Company
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public string? Name { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? TaxNumber { get; set; }
        public DateTime? EstablishedDate { get; set; }
        public string? BioText { get; set; }

        // FK
        public List<PersonCompanyLinkModel> RefPersonCompanyLink { get; } = new();

        // Model
        public List<PersonCompanyLinkModel> PersonCompanyLinks { get; set; } = new ();
        public List<PersonCompanyLinkModel> PersonCompanyLinksDeleted { get; set; } = new ();
        public List<PersonCompanyLinkTypeModel> AllLinkTypes { get; set; }
        public List<PersonModel> AllPersons { get; set; }
    }
}
