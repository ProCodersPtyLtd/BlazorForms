using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlazorForms.Platform.Shared.ApplicationParts
{
    public class PlatformAssemblyRegistrator : IAssemblyRegistrator
    {
        private static object _lock = new object();

        private static IEnumerable<Assembly> _assemblies;

        public IEnumerable<Assembly> GetConsideredAssemblies()
        {
            return _assemblies;
        }

        internal static void InitializeConfiguration(IEnumerable<Assembly> assemblies)
        {
            lock (_lock)
            {
                if (_assemblies == null)
                {
                    _assemblies = assemblies;
                }
                else
                {
                    var list = new List<Assembly>();
                    list.AddRange(_assemblies);
                    list.AddRange(assemblies);
                    _assemblies = list.Distinct();
                }
            }
        }

        public void InjectAssembly(Assembly assembly)
        {
            lock (_lock)
            {
                if (!_assemblies.Contains(assembly))
                {
                    var list = _assemblies.ToList();
                    list.Add(assembly);
                    _assemblies = list;
                }
            }
        }

        public void RemoveAssembly(Assembly assembly)
        {
            lock (_lock)
            {
                if (_assemblies.Contains(assembly))
                {
                    var list = _assemblies.ToList();
                    list.Remove(assembly);
                    _assemblies = list;
                }
            }
        }
    }
}
