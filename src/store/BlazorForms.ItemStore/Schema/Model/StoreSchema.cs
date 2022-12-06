using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazorForms.ItemStore
{
    // trying to define classes that serialized to jsonSchema
    public class StoreSchema
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
        public string[] Tags { get; set; }

        public Dictionary<string, StoreDefinition> Definitions { get; set; }
    }

    // The first property in these classes is a Key in Dictionary

    public class StoreDefinition
    {
        [Key]
        public string Name { get; set; }

        public string Type { get; set; }
        public string Comment { get; set; }

        public Dictionary<string, StoreProperty> Properties { get; set; }
    }

    public class StoreProperty
    {
        [Key]
        public string Name { get; set; }

        // types: string, number
        public string Type { get; set; }
        public string Comment { get; set; }

        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public bool Pk { get; set; }
        public bool AutoIncrement { get { return !ExternalId; } }

        public bool ExternalId { get; set; }
    }
}
