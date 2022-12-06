using BlazorForms.Flows.Definitions;
using System.Threading.Tasks;

namespace BlazorFormsDemoFlows
{
    public class MockTenantedScope : ITenantedScope
    {
        public async Task<string> GetTenantId()
        {
            return "1";
        }
    }
}
