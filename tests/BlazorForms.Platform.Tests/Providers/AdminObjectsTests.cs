using BlazorForms.Flows.Definitions;
using BlazorForms.Tests.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlazorForms.Platform.Tests.Providers
{
    public class AdminObjectsTests
    {
        private IFlowRunProvider _provider;

        public AdminObjectsTests()
        {
            _provider = new FlowRunProviderCreator().GetFlowRunProvider();
        }

        // ToDo: add some return values to MockFlowRunStorage.GetFlowContexts
        //[Fact]
        public async Task ReadStoredFlowsTest()
        {
            var ps = new FlowParamsGeneric();
            await _provider.GetListFlowUserView("BlazorForms.Admin.BusinessObjects.StorageFlows.StoredListFlow", ps, null);
        }
    }

    
}
