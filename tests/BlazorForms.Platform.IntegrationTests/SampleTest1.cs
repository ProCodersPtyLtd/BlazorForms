using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorForms.Platform.Integration.Tests.TestApi;
using BlazorForms.Integration.Tests.Server;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Integration.Tests
{
    [TestClass]
    public class SampleTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            await using var application = new TestApiApplication();

            var client = application.CreateClient();
            var data = await client.GetFromJsonAsync<List<WeatherForecast>>("/api/WeatherForecast/getWeatherForecast/{1}");
        }
    }
}