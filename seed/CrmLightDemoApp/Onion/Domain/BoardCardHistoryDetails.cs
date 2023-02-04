using CrmLightDemoApp.Onion.Domain.Entities;

namespace CrmLightDemoApp.Onion.Domain
{
    public class BoardCardHistoryDetails : BoardCardHistory
    {
        public virtual string? PersonFullName { get; set; }
    }
}
