using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Storage;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
	public class TenantAccountModel : IEntity, IFlowModel
	{
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CompanyId { get; set; }
        public string Bio { get; set; }

        // FK
        public List<UserModel> RefUser { get; } = new();

        public CompanyModel Company { get; set; }

  //      public static TenantAccountModel FromDetails(TenantAccountDetails val)
		//{
		//	var model = new TenantAccountModel();
		//	val.ReflectionCopyTo(model);
		//	return model;
		//}
	}
}
