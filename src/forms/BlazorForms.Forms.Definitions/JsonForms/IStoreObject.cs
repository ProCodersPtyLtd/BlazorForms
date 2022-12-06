using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Forms
{
    public interface IStoreObject
    {
        public string Name { get; set; }
        public bool Validated { get; set; }
    }

    
}
