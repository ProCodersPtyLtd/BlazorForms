using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Proxyma.Tests.Models
{
    public class Model1 
    {
        public virtual Addr[] AddrArray { get; set; }
        public virtual List<Addr> AddrList { get; set; }
        public virtual Tuple<string, int, Addr> TripleTuple { get; set; }
        public virtual List<string> StringList { get; set; }
        public virtual string[] StringArray { get; set; }
        public virtual int? ClientId { get; set; }

        public virtual string Title { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual Addr ResidentialAddress { get; set; }
    }

    public class Addr 
    {
        //public virtual int? AddressId { get; set; }
        public virtual string StreetLine1 { get; set; }
        public virtual string StreetLine2 { get; set; }
        public virtual string Suburb { get; set; }
        public virtual string State { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string Country { get; set; }
    }
}
