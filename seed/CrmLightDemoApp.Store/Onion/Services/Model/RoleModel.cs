using BlazorForms.Storage;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class RoleModel : IEntity
    {
        public virtual int Id { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual string Name { get; set; }

        // FK
        public List<UserRoleLinkModel> RefUserRoleLink { get; } = new();
    }
}
