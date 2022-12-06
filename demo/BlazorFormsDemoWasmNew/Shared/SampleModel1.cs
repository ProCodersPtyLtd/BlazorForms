using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms
{
    public class SampleModel1 : IFlowModel
    {
        public virtual string Name { get; set; }
        public virtual Money Amount { get; set; }

        #region IFlowModel implementation
        public ExpandoObject Bag => new ExpandoObject();

        public Dictionary<string, DynamicRecordset> Ext => new Dictionary<string, DynamicRecordset>();
        #endregion
    }

    // Another way to define model
    public class SampleModel2 : FlowModelBase
    {
        public virtual string Name { get; set; }
        public virtual Money Amount { get; set; }
    }

    public class CustAddrCountModel : FlowModelBase
    {
        public virtual List<CustAddrCount> Data { get; set; }
    }

    public class CustAddrCount : FlowModelBase
    {
        public Int32 CustomerId { get; set; }
        public String CompanyName { get; set; }
        public String EmailAddress { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String MiddleName { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool NameStyle { get; set; }
        public String PasswordHash { get; set; }
        public String PasswordSalt { get; set; }
        public String Phone { get; set; }
        public Guid Rowguid { get; set; }
        public String SalesPerson { get; set; }
        public String Suffix { get; set; }
        public String Title { get; set; }
        public virtual Int32 AddrCount { get; set; }
    }
}
