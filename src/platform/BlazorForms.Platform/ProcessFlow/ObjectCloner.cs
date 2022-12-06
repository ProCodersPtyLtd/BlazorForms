using Newtonsoft.Json;
using BlazorForms.Flows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.ProcessFlow
{
    public class ObjectCloner : IObjectCloner
    {
        private readonly IKnownTypesBinder _knownTypesBinder;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public ObjectCloner(IKnownTypesBinder knownTypesBinder)
        {
            _knownTypesBinder = knownTypesBinder;

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = _knownTypesBinder
            };
        }

        public async Task<T> CloneObject<T>(T source)
        {
            Task<T> task = Task.Run(() =>
            {
                var json = JsonConvert.SerializeObject(source, _jsonSerializerSettings);
                var result = JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
                return result;
            });

            return await task;
        }
    }
}
