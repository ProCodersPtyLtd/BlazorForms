using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.Shared.Exceptions
{
    public class AuthStateException : Exception
    {
        public AuthStateException() : base()
        { }

        public AuthStateException(string message) : base(message)
        { }
    }
}
