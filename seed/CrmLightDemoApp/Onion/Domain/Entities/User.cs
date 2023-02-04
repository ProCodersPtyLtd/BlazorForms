namespace CrmLightDemoApp.Onion.Domain.Entities
{
    // CRM user represents login account of organisation
    public class User : IEntity
    {
        public virtual int Id { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual int TenantAccountId { get; set; }
        public virtual int PersonId { get; set; }
        public virtual string Login { get; set; }

        // FK
        public List<UserRoleLink> RefUserRoleLink { get; } = new();
    }
}
