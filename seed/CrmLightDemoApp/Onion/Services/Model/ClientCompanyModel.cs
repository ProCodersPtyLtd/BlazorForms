using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using System.Dynamic;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class ClientCompanyModel : ClientCompanyDetails, IFlowModel
    {
        //public Company Company { get; set; }
        //public Person Manager { get; set; }
        //public Person AlternativeManager { get; set; }

        public string? ManagerFullName { get; set; }
        public string? AlternativeManagerFullName { get; set; }

        public List<PersonModel> AllPersons { get; set; } = new();
        public List<Company> AllCompanies { get; set; } = new();
    }
}
