using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace BlazorForms.Platform.Crm.Domain.Models
{
    public class AuthUser
    {
        public ClaimsPrincipal User { get; set; }
        public string Email { get; set; }
    }
}
