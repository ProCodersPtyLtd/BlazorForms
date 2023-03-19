using BlazorForms.Flows.Definitions;
using BlazorForms.Storage;

namespace BlazorForms.Platform.Tests.HighStore
{
    public class PersonCompanyLinkType : IEntity, IFlowModel
    {
        public virtual int Id { get; set; }
        public virtual string? Name { get; set; }
        public virtual bool Deleted { get; set; }

        // FK
        public List<PersonCompanyLink> RefPersonCompanyLink { get; } = new();
    }
}
