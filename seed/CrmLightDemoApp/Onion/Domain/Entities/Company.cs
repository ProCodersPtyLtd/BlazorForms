namespace CrmLightDemoApp.Onion.Domain.Entities
{
    public class Company : IEntity
    {
        public virtual int Id { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual string? Name { get; set; }
        public virtual string? RegistrationNumber { get; set; }
        public virtual string? TaxNumber { get; set; }
        public virtual DateTime? EstablishedDate { get; set; }
        public virtual string? BioText { get; set; }

        // FK
        public List<PersonCompanyLink> RefPersonCompanyLink { get; } = new();
    }
}
