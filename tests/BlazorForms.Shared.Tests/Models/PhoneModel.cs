using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared.Tests.Models
{
    public class PhoneModel
    {
        public virtual int? PhoneId { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string PhoneTypeCode { get; set; }

        public virtual bool IsPrimary { get; set; }
    }
}
