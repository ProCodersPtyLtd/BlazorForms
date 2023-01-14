using BlazorForms.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public class ReflectionProvider : IReflectionProvider
    {
        private readonly ILogger _logger;
        private readonly IKnownTypesBinder _knownTypesBinder;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public ReflectionProvider(IKnownTypesBinder knownTypesBinder, ILogger<ReflectionProvider> logger)
        {
            _logger = logger;
            _knownTypesBinder = knownTypesBinder;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = _knownTypesBinder
            };
        }

        public T CloneObject<T>(T source)
        {
            var json = JsonConvert.SerializeObject(source, _jsonSerializerSettings);
            var result = JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
            return result;
        }
    }
}
