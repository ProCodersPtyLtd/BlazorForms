using BlazorForms.Flows.Definitions;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class UserListModel : IFlowModel
    {
        public virtual List<UserModel>? Data { get; set; }
    }
}
