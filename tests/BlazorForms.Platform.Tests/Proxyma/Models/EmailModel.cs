using BlazorForms.Platform.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.Tests.Models
{
    [ProxyScope]
    public class EmailModel
    {
        public virtual int? EmailId { get; set; }

        public virtual string EmailAddress { get; set; }

        public virtual string EmailTypeCode { get; set; }

        public virtual bool IsPrimary { get; set; }
    }
}
