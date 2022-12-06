using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.Crm.Domain.Models.Messages
{
    public class EmailAddress
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }
    public class EmailMessage
    {
        public EmailAddress From { get; set; }
        public List<EmailAddress> To { get; set; }
        public List<EmailAddress> Cc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Dictionary<string, byte[]> Attachments { get; set; } = new Dictionary<string, byte[]>();
    }
}
