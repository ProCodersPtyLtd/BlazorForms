using CrmLightDemoApp.Onion.Domain;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class PersonCompanyLinkDetailsModel : PersonCompanyLinkDetails
    {
        public virtual bool Changed { get; set; }
        //public virtual string Changed { get; set; }
    }
}
