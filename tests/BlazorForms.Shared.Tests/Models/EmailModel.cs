using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared.Tests.Models
{
    public class EmailModel
    {
        public virtual int? EmailId { get; set; }

        public virtual string EmailAddress { get; set; }

        public virtual string EmailTypeCode { get; set; }

        public virtual bool IsPrimary { get; set; }
    }
}
