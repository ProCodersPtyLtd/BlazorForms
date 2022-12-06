using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.Crm.Domain.Models.Timesheets
{
    public class UserDetails
    {
        public virtual int EntityId { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual int? CompanyEntityId { get; set; }
        public virtual string Projects { get; set; }
        public virtual string Access { get; set; }
        public virtual string AssignedRole { get; set; }

        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}
