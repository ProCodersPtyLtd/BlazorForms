using BlazorForms.Flows.Definitions;
using BlazorForms.Flows;
using BlazorForms.Tests.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Shared.Extensions;
using Microsoft.SqlServer.Server;
using static BlazorForms.Shared.Extensions.QueryOptions;

namespace BlazorForms.Platform.Tests.Flows
{
    public class FlowExecutionTests
    {
        private IFlowRunProvider _provider;

        public FlowExecutionTests()
        {
            _provider = new FlowRunProviderCreator().GetFlowRunProvider();
        }

        [Fact]
        public async Task DemoClientTableFlowTest()
        {
            var pars = new FlowParamsGeneric { };
            var options = new QueryOptions { PageIndex = 0, PageSize = 100 };

            var data = await _provider.GetListFlowUserView(typeof(DemoClientTableFlow).FullName, pars, options);
            var formData = data.UserViewDetails;
            var model = data.GetModel() as DemoClientTableModel;
            Assert.Equal(100, model.Companies.Count);
        }
    }
}
