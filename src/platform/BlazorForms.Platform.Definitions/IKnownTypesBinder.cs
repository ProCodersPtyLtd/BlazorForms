using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public interface IKnownTypesBinder : ISerializationBinder
    {
        JsonSerializerSettings JsonSerializerSettings { get; }
        IEnumerable<Type> KnownTypes { get; }
        Dictionary<string, Type> KnownTypesDict { get; }
    }
}
