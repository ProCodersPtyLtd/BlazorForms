using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BlazorForms.Shared
{
    public static class AssemblyHelper
    {
        public static IEnumerable<Assembly> GetAssembliesStartsWith(string searchPattern, string startsWith)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = assemblies.Where(a => !a.IsDynamic).Select(a => a.Location).ToArray();

            var referencedPaths =
                Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, searchPattern, SearchOption.AllDirectories)
                    .ToList();
            var toLoad =
                referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            foreach (var path in toLoad)
            {
                try
                {
                    var assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path));
                    assemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    //Ignore; Perhaps catch BadImageFormatException and log the rest
                    var message = ex.Message;
                }
            }

            assemblies = assemblies.Where(a => a.GetName().Name.StartsWith(startsWith)).ToList();
            return assemblies;
        }

        public static IEnumerable<Assembly> GetAssemblies(string searchPattern)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = assemblies.Where(a => !a.IsDynamic).Select(a => a.Location).ToArray();

            var referencedPaths =
                Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, searchPattern, SearchOption.AllDirectories)
                    .ToList();
            var toLoad =
                referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            foreach (var path in toLoad)
            {
                try
                {
                    var assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path));
                    assemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    //Ignore; Perhaps catch BadImageFormatException and log the rest
                    var message = ex.Message;
                }
            }
            return assemblies;
        }



        public static Assembly GetAssembly(string searchPattern, string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = assemblies.Where(a => !a.IsDynamic).Select(a => a.Location).ToArray();

            var referencedPaths =
                Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, searchPattern, SearchOption.AllDirectories)
                    .ToList();
            var toLoad =
                referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            foreach (var path in toLoad)
            {
                try
                {
                    var assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path));
                    assemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    //Ignore; Perhaps catch BadImageFormatException and log the rest
                    var message = ex.Message;
                }
            }

            //var aaa = assemblies.Where(a => a.GetName().Name.Contains("BlazorForms."));

            return assemblies.FirstOrDefault(a => a.GetName().Name == name);
        }

    }
}
