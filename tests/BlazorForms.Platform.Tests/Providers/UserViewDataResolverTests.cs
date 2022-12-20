using BlazorForms.Forms;
using BlazorForms.Platform.Definitions.Shared;
using BlazorForms.Platform.Tests.FluentForms;
using BlazorForms.Shared;
using BlazorForms.Shared.FastReflection;
using BlazorForms.Tests.Framework.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Tests.Providers
{
    public class UserViewDataResolverTests
    {
        private IFlowRunProvider _provider;
        private ServiceProvider _serviceProvider;
        private IModelBindingNavigator _modelBindingNavigator;
        private IJsonPathNavigator _jsonPathNavigator;

        public UserViewDataResolverTests()
        {
            var creator = new FlowRunProviderCreator();
            _provider = creator.GetFlowRunProvider();
            _serviceProvider = creator.ServiceProvider;
            _modelBindingNavigator = _serviceProvider.GetRequiredService<IModelBindingNavigator>();
            _jsonPathNavigator = _serviceProvider.GetRequiredService<IJsonPathNavigator>();
        }

        [Fact]
        public void OldResolverTest()
        {
            var resolver = new UserViewDataResolver();
            TestResolveData(resolver);
        }

        [Fact]
        public void NewResolverWithoutFormaTest()
        {
            var resolver = new UserViewDataResolverJsonPath(_jsonPathNavigator, _modelBindingNavigator);
            TestResolveData(resolver);
        }

        [Fact]
        public void NewResolverFormaTest()
        {
            var resolver = new UserViewDataResolverJsonPath(_jsonPathNavigator, _modelBindingNavigator);
            var data = GetResolvedData(resolver);
            Assert.Equal("5/12/1987", data[0, 4]);
        }

        string[,] GetResolvedData(IUserViewDataResolver resolver) 
        { 
            var parser = new FormDefinitionParser(_serviceProvider);
            var details = parser.Parse(typeof(TestFormList2));
            var model = new TestCustAddrCountModel { Data = new List<TestCustAddrCount>() };
            model.Data.Add(new TestCustAddrCount { CustomerId = 101, FirstName = "Ben", LastName = "Jones", AddrCount = 4, DOB = new DateTime(1987,12,5) });
            model.Data.Add(new TestCustAddrCount { CustomerId = 102, FirstName = "Bengal", LastName = "Tiger", AddrCount = 12 });

            var data = resolver.ResolveData(details, model, null);
            return data;
        }

        private void TestResolveData(IUserViewDataResolver resolver)
        {
            var data = GetResolvedData(resolver);

            Assert.Equal(2, data.GetLength(0));
            Assert.Equal(8, data.GetLength(1));

            Assert.Equal("101", data[0,0]);
            Assert.Equal("Ben", data[0,1]);
            Assert.Equal("Jones", data[0,2]);
            Assert.Equal("4", data[0,3]);

            Assert.Equal("102", data[1,0]);
            Assert.Equal("Bengal", data[1,1]);
            Assert.Equal("Tiger", data[1,2]);
            Assert.Equal("12", data[1,3]);
        }
    }
}
