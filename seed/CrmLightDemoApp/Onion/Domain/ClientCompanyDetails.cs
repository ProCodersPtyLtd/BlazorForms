using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Domain
{
    public class ClientCompanyDetails : ClientCompany
    {
        public string? CompanyName { get; set; }
        public string? ManagerFirstName { get; set; }
        public string? ManagerLastName { get; set; }
        public string? AlternativeManagerFirstName { get; set; }
        public string? AlternativeManagerLastName { get; set; }

        //public string? ManagerFullName {  get { return $"{ManagerFirstName} {ManagerLastName}"; } }
        //public string? AlternativeManagerFullName {  get { return $"{AlternativeManagerFirstName} {AlternativeManagerLastName}"; } }
    }
}
