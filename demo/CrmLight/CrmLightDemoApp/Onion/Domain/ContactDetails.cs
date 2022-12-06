namespace CrmLightDemoApp.Onion.Domain
{
    public class ContactDetails
    {
        public virtual int Id { get; set; }
        public virtual int PersonId { get; set; }
        public virtual string? Phone { get; set; }
        public virtual string? Email { get; set; }
        public virtual DateTime LastUpdatedOn { get; set; }
    }
}
