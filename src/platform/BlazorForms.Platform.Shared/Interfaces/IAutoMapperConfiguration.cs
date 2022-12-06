using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.Shared.Interfaces
{
    public interface IAutoMapperConfiguration
    {
        MapperConfiguration Configuration { get; }
    }
}
