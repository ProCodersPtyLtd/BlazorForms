using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public interface IApplicationPart
    {
        void Register(RegistrationContext context);
        void Initialize();
    }
}
