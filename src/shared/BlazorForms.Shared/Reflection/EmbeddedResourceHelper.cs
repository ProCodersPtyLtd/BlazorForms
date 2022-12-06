using BlazorForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace BlazorForms.Shared
{
    public static class EmbeddedResourceHelper
    {
        public static string GetApiRequestFile(string namespaceAndFileName, Assembly assembly, ILogStreamer logStreamer)
        {
            try
            {
                var resourceStream = assembly.GetManifestResourceStream(namespaceAndFileName);

                using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }

            catch (Exception exception)
            {
                logStreamer.TrackException(new Exception($"Failed to read Embedded Resource {namespaceAndFileName}", exception.InnerException));
                throw new Exception($"Failed to read Embedded Resource {namespaceAndFileName}");
            }
        }

        public static string GetResourceFile(Assembly assembly, string namespaceAndFileName)
        {
            try
            {
                var resourceStream = assembly.GetManifestResourceStream(namespaceAndFileName);

                using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }

            catch (Exception exception)
            {
                throw new Exception($"Failed to read Embedded Resource {namespaceAndFileName}");
            }
        }
    }
}
