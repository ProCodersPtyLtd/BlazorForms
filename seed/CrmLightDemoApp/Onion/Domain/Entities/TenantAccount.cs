namespace CrmLightDemoApp.Onion.Domain.Entities
{
    // CRM tenant represents a company account that uses the system
    // all user accounts should be connected to this organisation account
    public class TenantAccount : IEntity
    {
        public virtual int Id { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual int CompanyId { get; set; }

        // FK
        public List<User> RefUser { get; } = new();
    }
}
