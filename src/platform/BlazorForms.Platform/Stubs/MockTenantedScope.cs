using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Stubs
{
    public class MockTenantedScope : ITenantedScope
    {
        public async Task<string> GetTenantId()
        {
            return "1";
        }
    }
}
