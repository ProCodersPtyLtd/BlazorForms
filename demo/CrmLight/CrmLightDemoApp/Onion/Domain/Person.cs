namespace CrmLightDemoApp.Onion.Domain
{
    public class Person
    {
        public virtual int Id { get; set; }
        public virtual string? FirstName { get; set; }
        public virtual string? LastName { get; set; }
        public virtual DateTime? BirthDate { get; set; }
    }
}
