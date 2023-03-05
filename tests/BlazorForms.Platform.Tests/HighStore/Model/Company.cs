using BlazorForms.Flows.Definitions;
using BlazorForms.Storage.Interfaces;

namespace BlazorForms.Platform.Tests.HighStore
{
    public class Company : IEntity, IFlowModel
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

        // for dropdowns
        public virtual List<PersonCompanyLinkType> AllLinkTypes { get; set; }
        public virtual List<Person> AllPersons { get; set; }
    }
}
