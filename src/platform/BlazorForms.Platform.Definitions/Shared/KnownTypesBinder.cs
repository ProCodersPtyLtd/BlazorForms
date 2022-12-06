using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorForms.Platform
{
    public class KnownTypesBinder : IKnownTypesBinder
    {
        private static readonly Dictionary<string, string> RenamedTypesMap = new Dictionary<string, string>
        {
            { "Platform.Crm.Business.Timesheets.Model.TimesheetsModel", "Platform.Crm.Domain.Models.Timesheets.TimesheetsModel" }
            , { "System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.String, mscorlib]]", "System.Collections.Generic.Dictionary`2[System.String,System.String]" }
            , { "System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib],[BlazorForms.Shared.DynamicRecordset, BlazorForms.Shared]]", "System.Collections.Generic.Dictionary`2[System.String,BlazorForms.Shared.DynamicRecordset]" }
            , { "System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib],[System.String, System.Private.CoreLib]]", "System.Collections.Generic.Dictionary`2[System.String,System.String]" }
            , { "BlazorForms.Flows.Engine.FlowContext", "BlazorForms.Flows.FlowContext" }
        };

        private static readonly object _lock = new object();

        private static List<Type> _knownTypes { get; set; }

        public JsonSerializerSettings JsonSerializerSettings { get { return CreateJsonSerializerSettings(); } }

        public IEnumerable<Type> KnownTypes => _knownTypes;

        private Dictionary<string, Type> _knownTypesDict;

        public Dictionary<string, Type> KnownTypesDict
        {
            get
            { 
                if (_knownTypesDict == null)
                {
                    _knownTypesDict = KnownTypes.GroupBy(t => t.FullName).ToDictionary(g => g.Key, g => g.FirstOrDefault());
                }

                return _knownTypesDict;
            }
        }

        private JsonSerializerSettings CreateJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = this
            };

            return settings;
        }

        public List<Type> GetTypes()
        {
            return _knownTypes;
        }

        public static void InitializeConfiguration(IEnumerable<Type> types)
        {
            lock (_lock)
            {
                if (_knownTypes == null)
                {
                    _knownTypes = types.ToList();
                }
                else
                {
                    var list = new List<Type>();
                    list.AddRange(_knownTypes);
                    list.AddRange(types);
                    _knownTypes = list.Distinct().ToList();
                }
            }
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            typeName = RenamedTypesMap.TryGetValue(typeName, out string renamedType) ? renamedType : typeName;
            
            // ToDo: _knownTypes is List<> and is much slower than Dictionary - switch to using _knownTypesDict
            var foundTypes = _knownTypes.Where(t => t.UnderlyingSystemType.ToString() == typeName);
            var type = foundTypes.SingleOrDefault(t => t.UnderlyingSystemType.ToString() == typeName);

            if (type == null)
            {
                var dict1 = typeof(Dictionary<string, DynamicRecordset>).UnderlyingSystemType.ToString();
                var dict2 = typeof(Dictionary<string, string>).UnderlyingSystemType.ToString();
                throw new Exception($"Type '{typeName}' is not found");
            }
            // var data = KnownTypes.Where(t => t.UnderlyingSystemType.ToString().Contains("Dictionary")).ToList();

            return type;
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.UnderlyingSystemType.ToString();
        }
    }

}
