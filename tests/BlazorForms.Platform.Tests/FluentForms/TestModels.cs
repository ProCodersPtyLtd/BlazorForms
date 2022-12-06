using Microsoft.EntityFrameworkCore;
using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.Tests.FluentForms
{
    public class TestModelsContext : DbContext
    {
    }

    public class TestOrder
    {
        public TestOrder()
        {
            OrderItems = new HashSet<TestOrderItem>();
        }

        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ExecuteDate { get; set; }
        public virtual int ClientId { get; set; }

        public virtual TestClient Client { get; set; }
        public virtual ICollection<TestOrderItem> OrderItems { get; set; }
        public virtual int OrderItemId { get; set; }
        public virtual List<TestClient> Clients { get; set; }
        public int CurrentClientId { get; set; }
        public string BaseCurrencySearch { get; set; }
    }
    
    public class TestInvalidOrder
    {
        public TestInvalidOrder()
        {
            OrderItems = new HashSet<TestOrderItem>();
        }

        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ExecuteDate { get; set; }
        public int ClientId { get; set; }

        public virtual TestClient Client { get; set; }
        public virtual ICollection<TestOrderItem> OrderItems { get; set; }
        public virtual int OrderItemId { get; set; }
        public virtual List<TestClient> Clients { get; set; }
        public int CurrentClientId { get; set; }
        public string BaseCurrencySearch { get; set; }
    }

    public partial class TestClient
    {
        public TestClient()
        {
            Order = new HashSet<TestOrder>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }

        public virtual ICollection<TestOrder> Order { get; set; }
    }

    public partial class TestOrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public bool? IsMajor { get; set; }
        public decimal Price { get; set; }

        public virtual TestOrder Order { get; set; }
    }

    public class TestCustAddrCountModel : FlowModelBase
    {
        public virtual List<TestCustAddrCount> Data { get; set; }
    }

    public class TestCustAddrCount
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
        public Int32 AddrCount { get; set; }
    }

    public class TestProjectListModel : FlowModelBase
    {
        public virtual List<TestProjectDetails> Projects { get; set; }
    }

    public class TestProjectDetails
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int MemberCount { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual decimal DayHours { get; set; }
        public virtual int? CalendarId { get; set; }
    }
}
