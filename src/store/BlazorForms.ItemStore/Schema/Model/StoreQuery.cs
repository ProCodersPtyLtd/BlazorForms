using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazorForms.ItemStore
{
    public class StoreQuery
    {
        public string Name { get; set; }
        public string SchemaName { get; set; }
        public string[] Tags { get; set; }
        public StoreQueryDefinition Query { get; set; }
    }

    // The first property in these classes is a Key in Dictionary

    public class StoreQueryParameter
    {
        [Key]
        public string Name { get; set; }

        // int, string, date, etc.
        public string Type { get; set; }
    }

    public class StoreQueryDefinition
    {
        [Key]
        public string QueryAlias { get; set; }

        public string Comment { get; set; }

        // Select
        public Dictionary<string, StoreQueryField> Fields { get; set; }

        // From
        public Dictionary<string, StoreTableReference> Tables { get; set; }
        public Dictionary<string, StoreQueryDefinition> SubQueries { get; set; }
        public StoreObjectJoin[] Joins { get; set; }

        // Group By
        public string[] GroupByFields { get; set; }

        // Having
        public StoreQueryCondition Having { get; set; }

        // Where
        public StoreQueryCondition Where { get; set; }
    }

    public class StoreQueryField
    {
        [Key]
        // Unique Field Alias in Query = FieldName by Default
        public string FieldAlias { get; set; }

        public StoreFieldReference Field { get; set; }

        // <empty>, min, max, count, concat(f1, f2, ...) etc.
        public string Operation { get; set; }
        public string RawExpression { get; set; }

    }
    
    public class StoreTableReference
    {
        [Key]
        // Unique Object Alias to use in Query
        public string ObjectAlias { get; set; }

        // Name of Table - Definition in Schema (StoreDefinition::Name)
        public string TableName { get; set; }
    }

    // No Key field in these classes

    public class StoreObjectJoin
    {
        public string LeftObjectAlias { get; set; }
        public string LeftFieldAlias { get; set; }
        public string[] LeftComplexFieldAliases { get; set; }

        public string RightObjectAlias { get; set; }
        public string RightFieldAlias { get; set; }
        public string[] RightComplexFieldAliases { get; set; }

        // <empty> or inner, left, right, full 
        public string JoinType { get; set; }

    }

    public class StoreQueryCondition
    {
        // public string RawExpression { get; set; }

        public QueryExpression Expression { get; set; }
        // ToDo: implement expression tree with logical operators and parentheses, or something powerfull and lightweight
    }

    public class StoreFieldReference
    {
        public string ObjectAlias { get; set; }
        public string FieldName { get; set; }
    }

    public class QueryExpression
    {
        public object ScalarParam { get; set; }
        public StoreQueryField FieldParam { get; set; }
        public QueryExpression Left { get; set; }
        public string Operator { get; set; }
        public QueryExpression Right { get; set; }
    }

}
