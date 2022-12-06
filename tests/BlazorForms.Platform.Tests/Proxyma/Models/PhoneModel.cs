using BlazorForms.Platform.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.Tests.Models
{
    [ProxyScope]
    public class PhoneModel
    {
        public virtual int? PhoneId { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string PhoneTypeCode { get; set; }

        public virtual bool IsPrimary { get; set; }
    }
}
