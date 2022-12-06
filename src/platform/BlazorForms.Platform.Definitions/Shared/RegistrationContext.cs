using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace BlazorForms.Platform
{
    public class RegistrationContext
    {
        public RegistrationContext(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
            AutoMapperProfiles = new Collection<AutoMapper.Profile>();
            DeserializerKnownTypes = new List<Type>();
            ProxyScopeModelTypes = new List<Type>();
            RegisteredAssemblies = new List<Assembly>();
        }

        public IServiceCollection ServiceCollection { get; }
        public Collection<AutoMapper.Profile> AutoMapperProfiles { get; }
        public List<Type> DeserializerKnownTypes { get; }
        public List<Type> ProxyScopeModelTypes { get; }
        public List<Assembly> RegisteredAssemblies { get; }
    }
}
