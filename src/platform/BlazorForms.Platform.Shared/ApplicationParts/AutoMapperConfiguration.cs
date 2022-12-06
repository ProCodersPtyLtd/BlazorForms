using AutoMapper;
using BlazorForms.Platform.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("BlazorForms.Tests.Framework")]
[assembly: InternalsVisibleTo("BlazorForms.Integration.Tests")]
[assembly: InternalsVisibleTo("BlazorApp2.Server")]
[assembly: InternalsVisibleTo("BlazorForms.IntegrationTests.Server")]
namespace BlazorForms.Platform.Shared.ApplicationParts
{
    public class AutoMapperConfiguration : IAutoMapperConfiguration
    {
        private static object _lock = new object();
        private static MapperConfiguration _configuration;

        public AutoMapperConfiguration()
        {
            
        }

        public MapperConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        internal static void InitializeMapperConfiguration(IEnumerable<AutoMapper.Profile> profiles)
        {
            lock (_lock)
            {
                if (_configuration == null)
                {
                    _configuration = new MapperConfiguration(cfg =>
                    {
                    //cfg.AddProfile<CrmAutoMapperProfile>();
                    cfg.AddProfiles(profiles);
                    });
                }
            }

        }
    }
}
