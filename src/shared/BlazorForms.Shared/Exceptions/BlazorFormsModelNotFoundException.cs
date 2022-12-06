using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared.Exceptions
{
    public class BlazorFormsModelNotFoundException : Exception
    {
        public BlazorFormsModelNotFoundException() : base()
        {
        }

        public BlazorFormsModelNotFoundException(string message) : base(message)
        {
        }
    }
}
