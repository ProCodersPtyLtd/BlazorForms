using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared.DataStructures
{
    public class FileDetails
    {
        public virtual string Name { get; set; } 
        public virtual byte[] Content { get; set; } 
    }
}
