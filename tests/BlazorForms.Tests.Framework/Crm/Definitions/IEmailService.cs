using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Platform.Crm.Domain.Models.Messages;

namespace BlazorForms.Platform.Crm.Definitions.Services
{
    public interface IEmailService
    {
        Task<bool> SendMessage(EmailMessage message);
        Task<bool> IsValidAddress(string email);
    }
}
