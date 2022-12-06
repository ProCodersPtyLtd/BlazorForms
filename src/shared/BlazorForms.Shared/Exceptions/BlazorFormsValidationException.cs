using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared.Exceptions
{
    public class BlazorFormsValidationException : Exception
    {
        public BlazorFormsValidationException() : base()
        {
        }

        public BlazorFormsValidationException(string message) : base(message)
        {
        }
    }
}
