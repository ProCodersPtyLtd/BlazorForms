using BlazorForms.Flows.Definitions;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class UserListModel : IFlowModel
    {
        public virtual List<UserModel>? Data { get; set; }
    }
}
