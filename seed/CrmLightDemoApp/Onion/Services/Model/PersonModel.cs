﻿using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Entities;
using System.Dynamic;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class PersonModel : Person, IFlowModel
    {
        public virtual string? FullName { get; set; }
        public virtual List<PersonCompanyLinkDetails> CompanyLinks { get; set; }
        public virtual Fomo Fomo { get; set; }
    }

    public class Fomo
    {
        public virtual string? Name { get; set; }
    }
}
