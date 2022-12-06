using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.Shared.Exceptions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException() : base()
        { }

        public ConfigurationException(string message) : base(message)
        { }
    }
}
