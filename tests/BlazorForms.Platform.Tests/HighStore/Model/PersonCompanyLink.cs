using BlazorForms.Flows.Definitions;
using BlazorForms.Storage;

namespace BlazorForms.Platform.Tests.HighStore
{
    public class PersonCompanyLink : IEntity, IFlowModel
    {
        public virtual int Id { get; set; }
        public virtual int PersonId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual int LinkTypeId { get; set; }
        public virtual bool Deleted { get; set; }

        public List<Person> RefPersonLink { get; } = new();
        public List<Company> RefCompanyLink { get; } = new();
    }
}
