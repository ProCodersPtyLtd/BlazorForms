using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Storage;
using System.Dynamic;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class PersonModel : IEntity, IFlowModel
    {
        //Person
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public bool Deleted { get; set; }

        // FK
        public List<PersonCompanyLinkModel> RefPersonCompanyLink { get; } = new();

        // Model
        public string? FullName { get; set; }
        public List<PersonCompanyLinkModel> CompanyLinks { get; set; }
    }
}
