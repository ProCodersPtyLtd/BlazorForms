using System;
using System.Collections.ObjectModel;

namespace BlazorForms.Shared.Tests.Models
{
    public class ClientModel
    {
        public virtual int? ClientId { get; set; }

        public virtual string Title { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string MiddelName { get; set; }

        //public User User { get; set; }


        public virtual DateTime? BirthDate { get; set; }

        public virtual string Gender { get; set; }

        public virtual string PassportNumber { get; set; }

        public virtual string TaxFileNumber { get; set; }

        public virtual string NationalityCountryCode { get; set; }

        public string ResidentCountryCode { get; set; }

        public virtual string PlaceOfBirth { get; set; }

        public virtual List<AddressModel> AddressList { get; set; }
        public virtual Collection<AddressModel> Addresses { get; set; }
        public virtual Collection<EmailModel> Emails { get; set; }
        public virtual Collection<PhoneModel> Phones { get; set; }

        public virtual AddressModel PostAddress { get; set; }
        public virtual AddressModel ResidentialAddress { get; set; }
        public virtual EmailModel PrimaryEmail { get; set; }
        public virtual PhoneModel HomePhone { get; set; }
        public virtual PhoneModel MobilePhone { get; set; }
    }
}
