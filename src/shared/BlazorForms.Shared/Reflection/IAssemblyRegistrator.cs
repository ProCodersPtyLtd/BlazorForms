using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BlazorForms.Shared
{
    public interface IAssemblyRegistrator
    {
        IEnumerable<Assembly> GetConsideredAssemblies();
        void InjectAssembly(Assembly assembly);
        void RemoveAssembly(Assembly assembly);
    }
}
