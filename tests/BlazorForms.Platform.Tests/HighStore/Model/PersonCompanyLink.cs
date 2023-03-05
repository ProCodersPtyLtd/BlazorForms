using BlazorForms.Flows.Definitions;
using BlazorForms.Storage.Interfaces;

namespace BlazorForms.Platform.Tests.HighStore
{
    public class PersonCompanyLink : IEntity, IFlowModel
    {
        public virtual int Id { get; set; }
        public virtual int PersonId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual int LinkTypeId { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
