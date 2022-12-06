using Newtonsoft.Json;
using BlazorForms.ItemStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace BlazorForms.Platform.Tests.Store
{
    public class StoreSchemaTests
    {
        StoreSchemaEngine _schemaEngine;

        public StoreSchemaTests()
        {
            _schemaEngine = new StoreSchemaEngine();
        }

        [Fact]
        public void SchemaReadTest()
        {
            string json = EmbeddedResource.GetApiRequestFile("BlazorForms.Platform.Tests.Store.TestSchema1.json");
            var data = _schemaEngine.ReadSchema(json);

            Assert.NotNull(data);
            Assert.Single(data.Definitions);
            Assert.Equal("basicAddress", data.Definitions.First().Value.Name);
        }

        [Fact]
        public void QueryReadTest()
        {
            string json = EmbeddedResource.GetApiRequestFile("BlazorForms.Platform.Tests.Store.TestQuery1.json");
            var data = _schemaEngine.ReadQuery(json);

            Assert.NotNull(data);
            Assert.Equal(2, data.Query.Fields.Count());
            Assert.Single(data.Query.Tables);
            Assert.Equal("address", data.Query.Tables.First().Value.ObjectAlias);
        }

        [Fact]
        public void DynamicRecordsetTest()
        {
            var d = new Dictionary<string, object>();
            d["id"] = 10;
            d["val"] = "my \"horse\" name";

            var text = JsonConvert.SerializeObject(d);
        }

        #region TestData





        #endregion
    }

    public static class EmbeddedResource
    {
        public static string GetApiRequestFile(string namespaceAndFileName)
        {
            try
            {
                var assembly = typeof(EmbeddedResource).GetTypeInfo().Assembly;
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
