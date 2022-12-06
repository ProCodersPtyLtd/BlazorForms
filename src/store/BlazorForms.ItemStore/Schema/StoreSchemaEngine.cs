using Newtonsoft.Json;
using BlazorForms.ItemStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.ItemStore
{
    public class StoreSchemaEngine : IStoreSchemaEngine
    {
        public StoreSchema ReadSchema(string json)
        {
            var data = JsonConvert.DeserializeObject<StoreSchema>(json);

            foreach(var tk in data.Definitions.Keys)
            {
                var table = data.Definitions[tk];
                table.Name = tk;

                foreach(var pk in table.Properties.Keys)
                {
                    table.Properties[pk].Name = pk;
                }
            }

            return data;
        }

        public StoreQuery ReadQuery(string json)
        {
            var data = JsonConvert.DeserializeObject<StoreQuery>(json);
            var query = data.Query;

            // populate key props in Dictionaries 

            if (query.Fields != null)
            {
                foreach (var fk in query.Fields.Keys)
                {
                    var f = query.Fields[fk];
                    f.FieldAlias = fk;
                }
            }

            if (query.Tables != null)
            {
                foreach (var tk in query.Tables.Keys)
                {
                    var table = query.Tables[tk];
                    table.ObjectAlias = tk;
                }
            }

            if (query.SubQueries != null)
            {
                foreach (var sk in query.SubQueries.Keys)
                {
                    var q = query.SubQueries[sk];
                    q.QueryAlias = sk;
                }
            }

            return data;
        }
    }
}
