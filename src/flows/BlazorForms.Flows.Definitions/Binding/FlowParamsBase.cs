using BlazorForms.Shared;
using System.Collections.Generic;
using System.Linq;

namespace BlazorForms.Flows.Definitions
{
    public abstract class FlowParamsBase: IFlowParams
    {
        public FlowParamsBase()
        {
            DynamicInput = new Dictionary<string, string>();
        }

        public string ItemId { get; set; }

        public bool ItemKeyAboveZero 
        {
            get
            {
                return int.TryParse(ItemId, out var id) && id > 0;
            }
        }

        public int ItemKey
        {
            get
            {
                return int.TryParse(ItemId, out var id) ? id : 0;
            }
        }

        public string ParentItemId { get; set; }
        public string AssignedUser { get; set; }
        public string AssignedTeam { get; set; }
        public FlowReferenceOperation Operation { get; set; }
        public string OperationName { get; set; }
        public string Tag { get; set; }

        public string this[string index]
        {
            get
            {
                return DynamicInput.TryGetValue(index, out var value) ? value : string.Empty;
            }

            set
            {
                DynamicInput[index] = value;
            }
        }

        public Dictionary<string, string> DynamicInput { get; set; }

        /// <summary>
        /// Concatenates the DynamicInput OVERWRITING
        /// </summary>
        /// <param name="other"></param>
        public Dictionary<string, string> ConcatDynamic(FlowParamsBase other)
        {
            if (other?.DynamicInput?.Count == 0)
            {
                return DynamicInput;
            }
            return other.DynamicInput
                .Concat(DynamicInput.Where(di => !other.DynamicInput.ContainsKey(di.Key)))
                .ToDictionary(k => k.Key, v => v.Value);
        }
    }
}
