using BlazorForms.Storage;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class LeadSourceTypeModel : IEntity
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public virtual string? Name { get; set; }
    }
}
