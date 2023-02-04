namespace CrmLightDemoApp.Onion.Domain.Entities
{
    public class UserRoleLink : IEntity
    {
        public virtual int Id { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual int UserId { get; set; }
        public virtual int RoleId { get; set; }
    }
}
