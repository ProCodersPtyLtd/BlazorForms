using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public class InvalidDependencyException : Exception
    {
        public InvalidDependencyException() : base()
        {

        }

        public InvalidDependencyException(string message) : base(message)
        {

        }
    }
}
