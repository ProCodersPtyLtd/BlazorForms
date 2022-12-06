using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
    public interface ITenantedScope
    {
        Task<string> GetTenantId();
    }
}
