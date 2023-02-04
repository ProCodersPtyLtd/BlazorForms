using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;

namespace CrmLightDemoApp.Onion.Services.Model
{
	public class TenantAccountModel : TenantAccountDetails
	{
		public static TenantAccountModel FromDetails(TenantAccountDetails val)
		{
			var model = new TenantAccountModel();
			val.ReflectionCopyTo(model);
			return model;
		}
	}
}
