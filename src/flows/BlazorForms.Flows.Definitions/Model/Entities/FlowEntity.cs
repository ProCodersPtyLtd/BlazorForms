using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace BlazorForms.Flows.Definitions
{
    public enum FlowStatus
    {
        Created = 0,
        Waiting = 1,
        Started = 2,
        Failed  = 3,
        Finished = 4,
        Deleted = 5
    }

    public static class FlowStatusExtensions
    {
        public static bool IsFlowStatusActive(this FlowStatus flowStatus)
        {
            var check = new[] { FlowStatus.Finished, FlowStatus.Deleted };
            return check.Contains(flowStatus);
        }
    }


    public enum FlowEntityTypes
    {
        [Description("Flow")]
        Flow = 1,
        [Description("FlowHistory")]
        FlowHistory = 2,
    }

    public class FlowEntityBase
    {         
        /// <summary>
        /// Etag is an environment separation key, used for shared Cosmos deployments
        /// </summary>
        public string EnvTag { get; set; }
        public string id { get; set; }
        public string Version { get; set; }
        /// <summary>
        /// RefId is a reference id that can be used to filter results on the query level
        /// RefId can be an id of object or a correlation id linking to multiple objects bound to this flow
        /// </summary>
        public string RefId { get; set; }
        /// <summary>
        /// TetantId used in multi-tenant systems
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// Key to order records
        /// </summary>
        public DateTime Created { get; set; }
        public string FlowName { get; set; }
        public string DocumentType { get; set; }
        public IEnumerable<string> FlowTags { get; set; }
        public FlowStatus FlowStatus { get; set; }
    }

    public class FlowEntity : FlowEntityBase
    {
        public FlowEntity()
        {
            DocumentType = FlowEntityTypes.Flow.GetDescription();
            Version = "1.0";
        }

        // use Context.Model
        // public object LastModel { get; set; }
        public FlowContext Context { get; set; }
    }

    // Just example for possible future extension 
    //public class FlowEntityHistory : FlowEntityBase
    //{
    //    public FlowEntityHistory()
    //    {
    //        DocumentType = FlowEntityTypes.FlowHistory.GetDescription();
    //        Version = "1.0";
    //    }

    //    /// <summary>
    //    /// Reference to FlowEntity for which this history is stored
    //    /// </summary>
    //    public string FlowEntityRefId { get; set; }
    //    public virtual Collection<FlowContext> Records { get; set; }
    //}
}
