namespace CrmLightDemoApp.Onion.Domain.Entities
{
    public class Role : IEntity
    {
        public virtual int Id { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual string Name { get; set; }

        // FK
        public List<UserRoleLink> RefUserRoleLink { get; } = new();
    }
}
