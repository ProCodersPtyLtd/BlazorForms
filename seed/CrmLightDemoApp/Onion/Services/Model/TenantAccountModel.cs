using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;

namespace CrmLightDemoApp.Onion.Services.Model
{
	public class TenantAccountModel : TenantAccountDetails, IFlowModel
	{
		public static TenantAccountModel FromDetails(TenantAccountDetails val)
		{
			var model = new TenantAccountModel();
			val.ReflectionCopyTo(model);
			return model;
		}
	}
}
