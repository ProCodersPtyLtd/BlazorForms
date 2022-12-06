using BlazorForms.Platform.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.Tests.Models
{
    [ProxyScope]
    public class AddressModel
    {
        public virtual int? AddressId { get; set; }
        public virtual string StreetLine1 { get; set; }
        public virtual string StreetLine2 { get; set; }
        public virtual string Suburb { get; set; }
        public virtual string State { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string Country { get; set; }
    }
}
