﻿using BlazorForms.Flows.Definitions;
using BlazorForms.Storage;

namespace BlazorForms.Platform.Tests.HighStore
{
    public class Person : IEntity, IFlowModel
    {
        public virtual int Id { get; set; }
        public virtual string? FirstName { get; set; }
        public virtual string? LastName { get; set; }
        public virtual DateTime? BirthDate { get; set; }
        public virtual string? Phone { get; set; }
        public virtual string? Email { get; set; }
        public virtual DateTime? LastUpdatedOn { get; set; }
        public virtual bool Deleted { get; set; }

        // FK
        public List<PersonCompanyLink> RefPersonCompanyLink { get; } = new();

        // Computed
        public virtual string? FullName { get; set; }
    }
}
