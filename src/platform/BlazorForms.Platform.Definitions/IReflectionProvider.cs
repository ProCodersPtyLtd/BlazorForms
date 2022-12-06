using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public interface IReflectionProvider
    {
        T CloneObject<T>(T source);
    }
}
