using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.DynamicCode.Engine
{
    internal class DynamicCodeEngine : IDynamicCodeEngine, IDisposable
    {
        private static Assembly SystemRuntime = Assembly.Load(new AssemblyName("System.Runtime"));

        private readonly CollectibleAssemblyLoadContext _context;
        private readonly MemoryStream _ms;
        private Assembly _assembly;

        public EmitResult CompilationResult { get; private set; }

        public Assembly Assembly { get { return _assembly; } }

        public DynamicCodeEngine()
        { 
            _ms = new MemoryStream();
            _context = new CollectibleAssemblyLoadContext();
        }

        public DynamicCodeEngine(Compilation compilation) : this()
        {
            CreateCodeEngine(compilation);
        }

        public DynamicCodeEngine(string assemblyName, string codeText, List<MetadataReference> metadataReferences) : this()
        {
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(SystemRuntime.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Expressions.Expression<>).GetTypeInfo().Assembly.Location),
            };

            references.AddRange(metadataReferences);

            var compilation = CSharpCompilation.Create(assemblyName, new[] { CSharpSyntaxTree.ParseText(codeText) }, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            CreateCodeEngine(compilation);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void CreateCodeEngine(Compilation compilation)
        {
            try
            {
                CompilationResult = compilation.Emit(_ms);

                if (CompilationResult.Success)
                {
                    _ms.Seek(0, SeekOrigin.Begin);
                    _assembly = _context.LoadFromStream(_ms);
                }
            }
            catch(Exception exc)
            {

            }

            //var greetMethod = type.GetMethod("Hello");
            //var result = (int)greetMethod.Invoke(instance, new object[] { i });
            //Console.WriteLine(result);
        }

        public Type GetType(string typeName)
        {
            var type = _assembly.GetType(typeName);
            return type;
        }

        public object CreateInstance(string typeName)
        {
            var type = _assembly.GetType(typeName);

            var instance = Activator.CreateInstance(type);
            return instance;
        }

        public void Dispose()
        {
            _ms.Dispose();
            _context.Unload();
        }

        public static Compilation SimpleCompilation(int i)
        {
            var compilation = CSharpCompilation.Create("DynamicAssembly", new[] { CSharpSyntaxTree.ParseText(@"
                public class Greeter
                {
                    public int Hello(int iteration)
                    {
                        return iteration + 3;
                    }
                }") },
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(SystemRuntime.Location),
                },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            return compilation;
        }
    }
}
