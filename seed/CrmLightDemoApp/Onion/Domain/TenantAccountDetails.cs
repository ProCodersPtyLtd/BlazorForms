using CrmLightDemoApp.Onion.Domain.Entities;

namespace CrmLightDemoApp.Onion.Domain
{
	public class TenantAccountDetails : TenantAccount
	{
		public Company Company { get; set; }
	}
}
