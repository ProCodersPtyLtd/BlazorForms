using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.ItemStore
{
    public class StoreMappingModel
    {
        // Damain model
        public Dictionary<string, ModelObject> DomainObjects { get; set; } 
        // Database model - tables and queries
        public Dictionary<string, ModelObject> StoreObjects { get; set; } 

        public ModelMapping[] Mappings { get; set; }
    }
}
