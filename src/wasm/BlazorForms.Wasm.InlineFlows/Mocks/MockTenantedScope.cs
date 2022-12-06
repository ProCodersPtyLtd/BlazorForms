using BlazorForms.Flows.Definitions;
using System.Threading.Tasks;

namespace BlazorForms.Wasm.InlineFlows
{
    public class MockTenantedScope : ITenantedScope
    {
        public async Task<string> GetTenantId()
        {
            return "1";
        }
    }
}
