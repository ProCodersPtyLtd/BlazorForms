using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.DynamicCode.Engine;
using BlazorForms.DynamicCode.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.DynamicCode
{
    public class DynamicFlowProvider : IDynamicFlowProvider
    {
        private readonly Dictionary<string, DynamicCodeContext> _cache = new Dictionary<string, DynamicCodeContext>();
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFlowParser _flowParser;
        private readonly IAssemblyRegistrator _assemblyRegistrator;
        private readonly IDynamicCodeValidationEngine _codeValidationEngine;

        public DynamicFlowProvider(ILogger<DynamicFlowProvider> logger, IServiceProvider serviceProvider, IFlowParser flowParser, 
            IAssemblyRegistrator assemblyRegistrator, IDynamicCodeValidationEngine codeValidationEngine)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _flowParser = flowParser;
            _assemblyRegistrator = assemblyRegistrator;
            _codeValidationEngine = codeValidationEngine;
        }

        public DynamicCodeContext GetFlow(string fullName)
        {
            return _cache[fullName];
        }

        public void RemoveFlow(string fullName)
        {
            if (_cache.ContainsKey(fullName))
            {
                _assemblyRegistrator.RemoveAssembly(_cache[fullName].Assembly);
                _flowParser.SetConsideredAssemblies(_assemblyRegistrator.GetConsideredAssemblies());
                _cache[fullName].Engine.Dispose();
                _cache.Remove(fullName);
            }
        }

        public DynamicCodeContext CompileFlow(DynamicCodeParameters ps)
        {
            var result = new DynamicCodeContext 
            {
                AssemblyName = ps.AssemblyName,
                References = new List<MetadataReference>(ps.References),
                Code = ps.Code,
                Namespace = ps.Namespace,
                ClassName = ps.ClassName,
                FullName = $"{ps.Namespace}.{ps.ClassName}",
            };

            RemoveFlow(result.FullName);
            result.References.AddRange(GetDefaultReferences());
            result.Engine = new DynamicCodeEngine(result.AssemblyName, result.Code, result.References);
            result.CompilationResult = result.Engine.CompilationResult;
            result.Success = result.Engine.CompilationResult.Success;

            if (result.Success)
            {
                result.Assembly = result.Engine.Assembly;

                _assemblyRegistrator.InjectAssembly(result.Engine.Assembly);
                _flowParser.SetConsideredAssemblies(_assemblyRegistrator.GetConsideredAssemblies());
                result.ClassType = result.Engine.Assembly.GetType(result.FullName);
                _cache[result.FullName] = result;
                result.Success = ValidateFlow(result);
            }

            if (!result.Success)
            {
                RemoveFlow(result.FullName);
                //result.Engine.Dispose();
            }
                
            return result;
        }

        /// <summary>
        /// Executes all flow validation rules on code text and assembly
        /// </summary>
        /// <param name="result"></param>
        public bool ValidateFlow(DynamicCodeContext ctx)
        {
            _codeValidationEngine.Validate(typeof(IDynamicCodeFlowValidationRule), ctx);

            if (ctx.ValidationIssues.Any(v => v.IsError))
            {
                return false;
            }

            return true;
        }

        private IEnumerable<MetadataReference> GetDefaultReferences()
        {
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(StateFlowBase).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ShortGuid).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IFlowModel).GetTypeInfo().Assembly.Location),
            };

            return references;
        }
    }
}
