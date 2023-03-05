using BlazorForms.Flows.Definitions;
using BlazorForms.Storage.Interfaces;

namespace BlazorForms.Platform.Tests.HighStore
{
    public class Role : IEntity, IFlowModel
    {
        public virtual int Id { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual string Name { get; set; }

        // FK
        public List<UserRoleLink> RefUserRoleLink { get; } = new();
    }
}
