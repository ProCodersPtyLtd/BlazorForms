using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Storage;
using System.Dynamic;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class ClientCompanyModel : IEntity, IFlowModel
    {
        // ClientCompany
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime? StartContractDate { get; set; }
        public int? ClientManagerId { get; set; }
        public int? AlternativeClientManagerId { get; set; }
        public bool Deleted { get; set; }

        // refs
        public PersonModel ClientManager { get; set; }
        public PersonModel AlternativeClientManager { get; set; }
        public CompanyModel Company { get; set; }

        // ClientCompanyDetails
        //public virtual string? CompanyName { get; set; }
        //public virtual string? ManagerFirstName { get; set; }
        //public virtual string? ManagerLastName { get; set; }
        //public virtual string? AlternativeManagerFirstName { get; set; }
        //public virtual string? AlternativeManagerLastName { get; set; }

        public string? ManagerFullName { get; set; }
        public string? AlternativeManagerFullName { get; set; }

        public List<PersonModel> AllPersons { get; set; } = new();
        public List<CompanyModel> AllCompanies { get; set; } = new();
    }
}
