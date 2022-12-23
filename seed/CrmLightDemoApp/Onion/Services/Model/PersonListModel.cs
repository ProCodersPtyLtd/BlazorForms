﻿using BlazorForms.Flows.Definitions;
using CrmLightDemoApp.Onion.Domain;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class PersonListModel : FlowModelBase
    {
        public virtual List<Person>? Data { get; set; }
    }
}