using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Storage;
using System.Dynamic;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class PersonCompanyLinkTypeModel : IEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Deleted { get; set; }

        // FK
        public List<PersonCompanyLinkModel> RefPersonCompanyLink { get; } = new();

        public bool Changed { get; set; }
    }

    public class PersonCompanyLinkTypeListModel : IFlowModel
    {
        public List<PersonCompanyLinkTypeModel>? Data { get; set; }
        public List<PersonCompanyLinkTypeModel>? Deleted { get; set; } = new List<PersonCompanyLinkTypeModel>();
    }
}
