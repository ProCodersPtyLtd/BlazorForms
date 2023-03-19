using BlazorForms.Flows.Definitions;
using BlazorForms.Storage;

namespace BlazorForms.Platform.Tests.HighStore
{
    // CRM user represents login account of organisation
    public class User : IEntity, IFlowModel
    {
        public virtual int Id { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual int TenantAccountId { get; set; }
        public virtual int PersonId { get; set; }
        public virtual string Login { get; set; }

        // FK
        public List<UserRoleLink> RefUserRoleLink { get; } = new();

        public virtual Person Person { get; set; }
    }
}
