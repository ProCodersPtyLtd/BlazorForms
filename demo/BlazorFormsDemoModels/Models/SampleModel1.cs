using System.Dynamic;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;

namespace BlazorFormsDemoModels.Models
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

    public class FieldSetModel1 : FlowModelBase
    {
        public virtual string Name { get; set; }
        public virtual Money Amount { get; set; }
        public virtual string Company { get; set; }
        public virtual string Abn { get; set; }
        public virtual List<CustAddrCount> Addresses { get; set; }

    }

    public class CustAddrCountModel : FlowModelBase
    {
        public virtual List<CustAddrCount> Data { get; set; }
    }

    public class CustAddrCount : FlowModelBase
    {
        public virtual Int32 CustomerId { get; set; }
        public virtual String CompanyName { get; set; }
        public virtual String EmailAddress { get; set; }
        public virtual String FirstName { get; set; }
        public virtual String LastName { get; set; }
        public virtual String MiddleName { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual bool NameStyle { get; set; }
        public virtual String PasswordHash { get; set; }
        public virtual String PasswordSalt { get; set; }
        public virtual String Phone { get; set; }
        public virtual Guid Rowguid { get; set; }
        public virtual String SalesPerson { get; set; }
        public virtual String Suffix { get; set; }
        public virtual String Title { get; set; }
        public virtual Int32 AddrCount { get; set; }
    }
}
