using BlazorForms.Flows.Definitions;
using BlazorForms.Storage.Interfaces;

namespace BlazorForms.Platform.Tests.HighStore
{
    public class UserRoleLink : IEntity, IFlowModel
    {
        public virtual int Id { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual int UserId { get; set; }
        public virtual int RoleId { get; set; }
    }
}
