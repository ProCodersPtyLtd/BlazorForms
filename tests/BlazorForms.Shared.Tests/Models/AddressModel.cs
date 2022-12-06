using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared.Tests.Models
{
    public class AddressModel
    {
        public virtual int? AddressId { get; set; }
        public virtual string StreetLine1 { get; set; }
        public virtual string StreetLine2 { get; set; }
        public virtual string Suburb { get; set; }
        public virtual string State { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string Country { get; set; }

        // Dummy sub property
        public virtual AddressBox Box { get; set; }
    }

    public class AddressBox
    {
        public virtual string Name { get; set; }
    }
}
