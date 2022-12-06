namespace CrmLightDemoApp.Onion.Domain
{
    public class PersonContactDetails : Person
    {
        public virtual ContactDetails Contact { get; set; }
    }
}
